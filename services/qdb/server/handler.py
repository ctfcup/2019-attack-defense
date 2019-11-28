from aiohttp import web

from protos import request_pb2, utils_pb2, logins_pb2, response_pb2


BITS_TO_GATES = {0: '+', 1: 'x'}
GATES_TO_BITS = {'+': utils_pb2.Gate.PLUS, 'x': utils_pb2.Gate.CROSS}

class Handler:
    def __init__(self, bb84):
        self._bb84 = bb84

    async def get_logins(self, request):
        data = await request.read()
        data = request_pb2.Request().FromString(data)

        client_qubits = [complex(x.real, x.image) for x in data.qubits[:]]
        client_gates = [BITS_TO_GATES[x] for x in data.gates[:]]
        server_gates = self._bb84.generate_basis()
        
        measures = self._bb84.measure(client_qubits, client_gates)
        key = self._bb84.generate_key(server_gates, client_gates, measures)
        key, iv = key[:16], key[16:]
        print(key.hex(), iv.hex())

        response = response_pb2.Response()
        gates = [GATES_TO_BITS[x] for x in server_gates]
        response.gates.extend(gates)
        logins = response.option.Extensions[logins_pb2.LOGINS]
        logins.name = '123'
        return web.Response(body=response.SerializeToString())