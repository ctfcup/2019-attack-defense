import random
from os import urandom

import aiohttp
import asyncio
from Crypto.Cipher import AES
from Crypto.Util.Padding import unpad

from ciphers import aes, xor
from ciphers.bb84 import BB84
from protos import get_msg_pb2, logins_pb2, response_pb2, request_pb2, set_msg_pb2, utils_pb2
from utils import Gate, H, Qubit


ALGOS = {
    utils_pb2.Algo.AES: aes,
    utils_pb2.Algo.XOR: xor
}


def send(bits, gates):
    qubits = []
    for bit, gate in zip(bits, gates):
        qubit = Qubit(bit, 1-bit)
        if gate.value == '+':
            qubits.append(qubit)
        else:
            qubits.append(H(qubit))
    return qubits


def generate_request():
    request = request_pb2.Request()

    bits, client_gates = [], []
    for _ in range(1, 280):
        bits.append(float(random.randint(0, 1)))
        client_gates.append(Gate(random.choice('+x')))

    qubits = [x.to_protobuf() for x in send(bits, client_gates)]
    request.qubits.extend(qubits)
    request.gates.extend([x.to_protobuf() for x in client_gates])

    return request, bits, client_gates


async def get_logins(algo):
    request, bits, client_gates = generate_request()
    request.algo = algo
    request = request.SerializeToString()

    async with aiohttp.ClientSession() as session:
        async with session.post('http://localhost:16962/get_logins', data=request) as r:
            data = response_pb2.Response().FromString(await r.read())
            server_gates = list(map(Gate.from_protobuf, data.gates[:]))
            logins = data.option.Extensions[logins_pb2.LOGINS]

    bb84 = BB84()
    key = bb84.generate_key(server_gates, client_gates, bits)

    algo = ALGOS[algo]
    decrypted_logins = []
    for name in logins.name[:]:
        decrypted_logins.append(algo.decrypt(key, name))
    return decrypted_logins


async def set_msg(login, msg, algo):
    request, bits, client_gates = generate_request()

    request.algo = algo
    request.option.Extensions[set_msg_pb2.SET_MSG_REQUEST].msg = msg
    request.option.Extensions[set_msg_pb2.SET_MSG_REQUEST].login = login
    request = request.SerializeToString()

    async with aiohttp.ClientSession() as session:
        async with session.post('http://localhost:16962/set_msg', data=request) as r:
            response = response_pb2.Response().FromString(await r.read())
            server_gates = list(map(Gate.from_protobuf, response.gates[:]))

    bb84 = BB84()
    key = bb84.generate_key(server_gates, client_gates, bits)

    algo = ALGOS[algo]
    set_msg_proto = response.option.Extensions[set_msg_pb2.SET_MSG_RESPONSE]
    decrypted_msg = algo.decrypt(key, set_msg_proto.msg)

    print(decrypted_msg == msg)

    return key


async def get_msg(login, prev_key, algo):
    request, bits, client_gates = generate_request()

    request.algo = algo
    request.option.Extensions[get_msg_pb2.GET_MSG_REQUEST].login = login
    request = request.SerializeToString()

    async with aiohttp.ClientSession() as session:
        async with session.post('http://localhost:16962/get_msg', data=request) as r:
            response = response_pb2.Response().FromString(await r.read())
            server_gates = list(map(Gate.from_protobuf, response.gates[:]))

    bb84 = BB84()
    key = bb84.generate_key(server_gates, client_gates, bits)

    algo = ALGOS[algo]
    get_msg_proto = response.option.Extensions[get_msg_pb2.GET_MSG_RESPONSE]
    decrypted_msg = algo.decrypt(key, get_msg_proto.msg)

    return algo.decrypt(prev_key, decrypted_msg)


async def check_all():
    login, msg, algo = urandom(10), urandom(100), utils_pb2.Algo.XOR
    key = await set_msg(login, msg, algo)
    msg_form_db = await get_msg(login, key, algo)
    print(msg_form_db == msg)
    logins = await get_logins(algo)
    print(logins)
    print(login in logins)


if __name__ == '__main__':
    asyncio.run(check_all())