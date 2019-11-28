import random
import numpy

from math import sqrt
from Crypto.Util.number import long_to_bytes


class BB84:
    def __init__(self, key_size=32):
        self._key_size = key_size

    def generate_basis(self):
        return [random.choice('+x') for _ in range(self._key_size*10)]

    def measure(self, qubits, gates):
        for qubit, gate in zip(qubits, gates):
            if gate == 'x':
                qubit *= complex(1/sqrt(2), -1/sqrt(2))
            yield numpy.random.choice(numpy.arange(0, 2), p=[round(pow(qubit.real, 2), 1), round(pow(qubit.imag, 2), 1)])

    def _restore_key(self, server_gates, client_gates, measure):
        for bit, server_gate, client_gate in zip(measure, server_gates, client_gates):
            if server_gate == client_gate:
                yield bit
    
    def generate_key(self, server_gates, client_gates, measure):
        bits = ''.join(map(str, self._restore_key(server_gates, client_gates, measure)))
        return bytes.fromhex(hex(int(bits, 2))[2:].zfill(64))


if __name__ == '__main__':
    size = 32
    bb84 = BB84()

    server_basis = bb84.generate_basis()

    client_qubits = []
    for _ in range(size*8*4):
        x = random.randint(0, 1)
        client_qubits.append(complex(x, 1-x))
    client_gates = ['+'] * size*8*4
    measures = bb84.measure(client_qubits, client_gates)

    key = bb84.generate_key(server_basis, client_gates, measures)
    key, iv = key[:16], key[16:]
    print(key[:8].hex(), key.hex())