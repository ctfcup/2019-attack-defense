from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad


def encrypt(key, pt):
    key = bytes.fromhex(hex(key)[2:].zfill(64)[:64])
    key, iv = key[:16], key[16:]
    return AES.new(key, iv=iv, mode=AES.MODE_CBC).encrypt(pad(pt, 16))

def decrypt(key, ct):
    key = bytes.fromhex(hex(key)[2:].zfill(64)[:64])
    key, iv = key[:16], key[16:]
    return unpad(AES.new(key, iv=iv, mode=AES.MODE_CBC).decrypt(ct), 16)
