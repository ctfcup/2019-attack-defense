import random
import numpy as np
from math import sqrt

from Crypto.Util.number import long_to_bytes

from utils import Gate, H, Qubit


class BB84:
    def __init__(self, key_size=32):
        self._key_size = key_size

    def generate_basis(self):
        return [Gate(random.choice('+x')) for _ in range(self._key_size*10)]

    def recieve(self, qubits, gates):
        bits = []
        for qubit, gate in zip(qubits, gates):
            if gate.value == '+':
                bits.append(qubit.measure())
            else:
                bits.append(H(qubit).measure())
        return bits

    def generate_key(self, server_gates, client_gates, bits):
        key = ''
        for server_gate, client_gate, bit in zip(server_gates, client_gates, bits):
            if server_gate == client_gate:
                key += str(int(bit))
        return int(key, 2)
