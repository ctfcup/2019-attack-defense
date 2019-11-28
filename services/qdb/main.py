from os import urandom
from Crypto.Cipher import AES

def xor_bytes(a, b):
    return bytes([x^y for x, y in zip(a, b)])

if __name__ == '__main__':
    key = b'\0'*13 + b'\x80' + urandom(2)
    iv = urandom(16)
    pt = urandom(16*3)

    ct = AES.new(key, AES.MODE_CBC, iv).encrypt(pt)
    print(key.hex())

    while True:
        _pt = AES.new(b'\0'*13 + b'\x80' + urandom(2), AES.MODE_CBC, b'\0'*16).decrypt(ct)
        if pt[16:] == _pt[16:]:
            print(key.hex())
            exit()