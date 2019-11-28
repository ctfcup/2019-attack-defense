from aiohttp import web

from handler import Handler
from ciphers.bb84 import BB84

handler = Handler(BB84())

app = web.Application()
app.add_routes([
    web.post('/get_logins', handler.get_logins)
])
web.run_app(app)