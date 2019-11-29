#!/usr/bin/env python3.7
import os
import random
import traceback

import asyncio

from api import API
from gornilo import Checker, CheckRequest, GetRequest, PutRequest, Verdict

from ciphers import aes, xor
from ciphers.bb84 import BB84
from protos import get_msg_pb2, logins_pb2, response_pb2, request_pb2, set_msg_pb2, utils_pb2
from utils import Gate, H, Qubit


ALGOS = {
    utils_pb2.Algo.AES: aes,
    utils_pb2.Algo.XOR: xor
}

checker = Checker()


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


async def get_logins(api, algo, hostname):
    request, bits, client_gates = generate_request()
    request.algo = algo
    request = request.SerializeToString()

    async with API(hostname) as api:
        response = await api.get_logins(request)
    server_gates = list(map(Gate.from_protobuf, response.gates[:]))
    logins = response.option.Extensions[logins_pb2.LOGINS]

    bb84 = BB84()
    key = bb84.generate_key(server_gates, client_gates, bits)

    algo = ALGOS[algo]
    decrypted_logins = []
    for name in logins.name[:]:
        decrypted_logins.append(algo.decrypt(key, name))
    return decrypted_logins


async def set_msg(login, msg, algo, hostname):
    request, bits, client_gates = generate_request()

    request.algo = algo
    request.option.Extensions[set_msg_pb2.SET_MSG_REQUEST].msg = msg
    request.option.Extensions[set_msg_pb2.SET_MSG_REQUEST].login = login
    request = request.SerializeToString()

    async with API(hostname) as api:
        response = await api.set_msg(request)
    server_gates = list(map(Gate.from_protobuf, response.gates[:]))

    bb84 = BB84()
    key = bb84.generate_key(server_gates, client_gates, bits)

    algo = ALGOS[algo]
    set_msg_proto = response.option.Extensions[set_msg_pb2.SET_MSG_RESPONSE]
    decrypted_msg = algo.decrypt(key, set_msg_proto.msg)

    return key, decrypted_msg


async def get_msg(login, prev_key, algo, hostname):
    request, bits, client_gates = generate_request()

    request.algo = algo
    request.option.Extensions[get_msg_pb2.GET_MSG_REQUEST].login = login
    request = request.SerializeToString()

    async with API(hostname) as api:
        response = await api.get_msg(request)
    server_gates = list(map(Gate.from_protobuf, response.gates[:]))

    bb84 = BB84()
    key = bb84.generate_key(server_gates, client_gates, bits)

    algo = ALGOS[algo]
    get_msg_proto = response.option.Extensions[get_msg_pb2.GET_MSG_RESPONSE]
    decrypted_msg = algo.decrypt(key, get_msg_proto.msg)

    return algo.decrypt(prev_key, decrypted_msg)


@checker.define_check
async def check_service(request: CheckRequest) -> Verdict:
    async with API(request.hostname) as api:
        try:
            await api.ping()
        except Exception as ex:
            return Verdict.DOWN('DOWN', traceback.format_exc())
    return Verdict.OK()


@checker.define_put(vuln_num=1, vuln_rate=1)
async def put_flag_aes(request: PutRequest) -> Verdict:
    algo = utils_pb2.Algo.AES
    login, msg = os.urandom(12), request.flag.encode()

    key, decrypted_msg = await set_msg(login, msg, algo, request.hostname)

    if decrypted_msg == msg:
        return Verdict.OK("{}:{}".format(login.hex(), key))
    else:
        return Verdict.MUMBLE("AES doesn't work", "AES doesn't work")


@checker.define_get(vuln_num=1)
async def get_flag_aes(request: GetRequest) -> Verdict:
    algo = utils_pb2.Algo.AES
    login, key = request.flag_id.split(':')
    login, key = bytes.fromhex(login), int(key)

    flag = await get_msg(login, key, algo, request.hostname)
    if flag == request.flag.encode():
        return Verdict.OK()
    else:
        return Verdict.MUMBLE("AES doesn't work", "AES doesn't work")


@checker.define_put(vuln_num=2, vuln_rate=1)
async def put_flag_xor(request: PutRequest) -> Verdict:
    algo = utils_pb2.Algo.XOR
    login, msg = os.urandom(12), request.flag.encode()

    key, decrypted_msg = await set_msg(login, msg, algo, request.hostname)

    if decrypted_msg == msg:
        return Verdict.OK("{}:{}".format(login.hex(), key))
    else:
        return Verdict.MUMBLE("XOR doesn't work", "XOR doesn't work")


@checker.define_get(vuln_num=2)
async def get_flag_xor(request: GetRequest) -> Verdict:
    algo = utils_pb2.Algo.XOR
    login, key = request.flag_id.split(':')
    login, key = bytes.fromhex(login), int(key)

    flag = await get_msg(login, key, algo, request.hostname)
    if flag == request.flag.encode():
        return Verdict.OK()
    else:
        return Verdict.MUMBLE("XOR doesn't work", "XOR doesn't work")


if __name__ == "__main__":
    checker.run()