from aiohttp import web

from handler import Handler
from ciphers.bb84 import BB84
from db import DB

bb84 = BB84()
db = DB()
handler = Handler(bb84, db)

app = web.Application()
app.add_routes([
    web.get('/ping', handler.ping),
    web.post('/get_logins', handler.get_logins),
    web.post('/set_msg', handler.set_msg),
    web.post('/get_msg', handler.get_msg)
])
