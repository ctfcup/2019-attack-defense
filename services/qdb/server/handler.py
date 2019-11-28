from aiohttp import web
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad

from protos import logins_pb2, response_pb2, request_pb2, utils_pb2 
from utils import Gate, Qubit


class Handler:
    def __init__(self, bb84):
        self._bb84 = bb84

    async def get_logins(self, request):
        data = await request.read()
        data = request_pb2.Request().FromString(data)

        client_qubits = list(map(Qubit.from_protobuf, data.qubits[:]))
        client_gates = list(map(Gate.from_protobuf, data.gates[:]))
        server_gates = self._bb84.generate_basis()

        bits = self._bb84.recieve(client_qubits, client_gates)
        key = self._bb84.generate_key(server_gates, client_gates, bits)
        key, iv = key[:16], key[16:]

        response = response_pb2.Response()
        response.gates.extend([x.to_protobuf() for x in server_gates])
        logins = response.option.Extensions[logins_pb2.LOGINS]
        aes = AES.new(key, iv=iv, mode=AES.MODE_CBC)
        logins.name = aes.encrypt(pad(b'123', 16))
        return web.Response(body=response.SerializeToString())
