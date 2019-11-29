from itertools import cycle

from Crypto.Util.number import long_to_bytes


def _xor_bytes(a, b):
    return bytes([x^y for x,y in zip(a, cycle(b))])

def encrypt(key, pt):
    key = long_to_bytes(key)
    return _xor_bytes(pt, key)

def decrypt(key, ct):
    key = long_to_bytes(key)
    return _xor_bytes(ct, key)
