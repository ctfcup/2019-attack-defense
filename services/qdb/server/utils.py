import random
import numpy as np
from math import pow, sqrt

from protos import utils_pb2


def H(qubit):
    state = ((1 / sqrt(2)) * np.matrix([[1,1],[1,-1]])) * qubit.state
    return Qubit(state[0,0], state[1,0])

class Qubit:
    def __init__(self, real, image):
        self.state = np.matrix([[real], [image]])

    def __eq__(self, other):
        return np.allclose(self.state, other.state)

    def measure(self):
        M = 1e6
        m = random.randint(0,M)
        return int(m < round(pow(np.matrix([1, 0])*self.state, 2), 2)*M)

    def to_protobuf(self):
        if self == Qubit(1, 0):
            return utils_pb2.Qubit.QUBIT_0
        if self == H(Qubit(1, 0)):
            return utils_pb2.Qubit.QUBIT_45
        if self == Qubit(0, 1):
            return utils_pb2.Qubit.QUBIT_90
        if self == H(Qubit(0, 1)):
            return utils_pb2.Qubit.QUBIT_135


    @staticmethod
    def from_protobuf(qubit_proto):
        if qubit_proto == utils_pb2.Qubit.QUBIT_0:
            return Qubit(1, 0)
        if qubit_proto == utils_pb2.Qubit.QUBIT_45:
            return H(Qubit(1, 0))
        if qubit_proto == utils_pb2.Qubit.QUBIT_90:
            return Qubit(0, 1)
        if qubit_proto == utils_pb2.Qubit.QUBIT_135:
            return H(Qubit(0, 1))

class Gate:
    def __init__(self, value):
        self.value = value

    def __eq__(self, other):
        return self.value == other.value

    def to_protobuf(self):
        if self.value == '+':
            return utils_pb2.Gate.PLUS
        else:
            return utils_pb2.Gate.CROSS

    @staticmethod
    def from_protobuf(gate_proto):
        return Gate('+' if gate_proto is utils_pb2.Gate.PLUS else 'x')