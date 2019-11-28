import random

import aiohttp
import asyncio
from Crypto.Cipher import AES
from Crypto.Util.Padding import unpad

from ciphers.bb84 import BB84
from protos import logins_pb2, response_pb2, request_pb2, utils_pb2 
from utils import Gate, H, Qubit

def send(bits, gates):
    qubits = []
    for bit, gate in zip(bits, gates):
        qubit = Qubit(bit, 1-bit)
        if gate.value == '+':
            qubits.append(qubit)
        else:
            qubits.append(H(qubit))
    return qubits

async def get_logins():
    bb84 = BB84()

    request = request_pb2.Request()

    bits, client_gates = [], []
    for _ in range(1, 280):
        bits.append(float(random.randint(0, 1)))
        client_gates.append(Gate(random.choice('+x')))

    qubits = [x.to_protobuf() for x in send(bits, client_gates)]
    request.qubits.extend(qubits)
    request.gates.extend([x.to_protobuf() for x in client_gates])
    data = request.SerializeToString()

    async with aiohttp.ClientSession() as session:
        async with session.post('http://localhost:16962/get_logins', data=data) as r:
            data = await r.read()
            data = response_pb2.Response().FromString(data)
    logins = data.option.Extensions[logins_pb2.LOGINS]
    server_gates = list(map(Gate.from_protobuf, data.gates[:]))
    key = bb84.generate_key(server_gates, client_gates, bits)
    key, iv = key[:16], key[16:]

    for name in logins.name[:]:
        print(unpad(AES.new(key, iv=iv, mode=AES.MODE_CBC).decrypt(name), 16))


if __name__ == '__main__':
    asyncio.run(get_logins())