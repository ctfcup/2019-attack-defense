from itertools import cycle

def _xor_bytes(a, b):
    return bytes([x^y for x,y in zip(a, cycle(b))])

def encrypt(key, pt):
    key = bytes.fromhex(hex(key)[2:].zfill(16)[:16])
    return _xor_bytes(pt, key)

def decrypt(key, ct):
    key = bytes.fromhex(hex(key)[2:].zfill(16)[:16])
    return _xor_bytes(ct, key)
