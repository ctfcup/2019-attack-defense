import random

from ciphers.bb84 import BB84
from utils import Gate, Qubit


if __name__ == '__main__':
    alice_bits = []
    alice_gates = []
    bob_gates = []

    for _ in range(1, 280):
        x = float(random.randint(0, 1))
        alice_bits.append(x)
        alice_gates.append(Gate(random.choice('+x')))
        bob_gates.append(Gate(random.choice('+x')))

    bb84 = BB84()
    alice_qubits = bb84.send(alice_bits, alice_gates)
    bob_bits = bb84.recieve(alice_qubits, bob_gates)

    alice_key = bb84.generate_key(bob_gates, alice_gates, alice_bits)
    bob_key = bb84.generate_key(bob_gates, alice_gates, bob_bits)
    
    print(alice_key)
    print(bob_key)