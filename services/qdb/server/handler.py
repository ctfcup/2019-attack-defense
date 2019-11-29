from aiohttp import web

from ciphers import aes, xor
from protos import get_msg_pb2, logins_pb2, response_pb2, request_pb2, set_msg_pb2, utils_pb2 
from utils import Gate, Qubit


ALGOS = {
    utils_pb2.Algo.AES: aes,
    utils_pb2.Algo.XOR: xor
}


class Handler:
    def __init__(self, bb84, db):
        self._bb84 = bb84
        self._db = db

    def _process_request(self, request):
        client_qubits = list(map(Qubit.from_protobuf, request.qubits[:]))
        client_gates = list(map(Gate.from_protobuf, request.gates[:]))
        server_gates = self._bb84.generate_basis()

        bits = self._bb84.recieve(client_qubits, client_gates)
        key = self._bb84.generate_key(server_gates, client_gates, bits)

        return server_gates, key


    async def get_logins(self, request):
        request = request_pb2.Request().FromString(await request.read())
        server_gates, key = self._process_request(request)
        algo = ALGOS[request.algo]

        response = response_pb2.Response()
        response.gates.extend([x.to_protobuf() for x in server_gates])
        logins = []
        for login in await self._db.get_logins():
            logins.append(algo.encrypt(key, login))

        response.option.Extensions[logins_pb2.LOGINS].name.extend(logins)
        return web.Response(body=response.SerializeToString())

    async def set_msg(self, request):
        request = request_pb2.Request().FromString(await request.read())
        server_gates, key = self._process_request(request)
        algo = ALGOS[request.algo]

        login = request.option.Extensions[set_msg_pb2.SET_MSG_REQUEST].login
        msg = request.option.Extensions[set_msg_pb2.SET_MSG_REQUEST].msg
        encrypted_msg = algo.encrypt(key, msg)

        if not await self._db.check_login(login):
            await self._db.set_msg(login, encrypted_msg)

            response = response_pb2.Response()
            response.gates.extend([x.to_protobuf() for x in server_gates])
            set_msg_proto = response.option.Extensions[set_msg_pb2.SET_MSG_RESPONSE]
            set_msg_proto.msg = encrypted_msg
            return web.Response(body=response.SerializeToString())
        else:
            return web.Response(status=400)
    
    async def get_msg(self, request):
        request = request_pb2.Request().FromString(await request.read())
        server_gates, key = self._process_request(request)
        algo = ALGOS[request.algo]

        login = request.option.Extensions[get_msg_pb2.GET_MSG_REQUEST].login

        if await self._db.check_login(login):
            msg = await self._db.get_msg(login)
            encrypted_msg = algo.encrypt(key, msg)

            response = response_pb2.Response()
            response.gates.extend([x.to_protobuf() for x in server_gates])
            get_msg_proto = response.option.Extensions[get_msg_pb2.GET_MSG_RESPONSE]
            get_msg_proto.msg = encrypted_msg
            return web.Response(body=response.SerializeToString())
        else:
            return web.Response(status=400)