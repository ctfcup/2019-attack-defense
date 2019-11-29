import aiohttp
from aiohttp.client import ClientTimeout

from networking.masking_connector import get_agent
from protos import response_pb2


PORT = 16962


class API:
    def __init__(self, hostname):
        self.hostname = hostname
        self.session = aiohttp.ClientSession(timeout=ClientTimeout(total=100), headers={"User-Agent": get_agent()})

    async def __aenter__(self):
        return self

    async def __aexit__(self, exc_type, exc_val, exc_tb):
        await self.session.close()

    async def ping(self):
        async with aiohttp.ClientSession() as session:
            async with session.get('http://{}:{}/ping'.format(self.hostname, PORT)) as r:
                return r.status == 200

    async def get_logins(self, request):
        async with aiohttp.ClientSession() as session:
            async with session.post('http://{}:{}/get_logins'.format(self.hostname, PORT), data=request) as r:
                return response_pb2.Response().FromString(await r.read())

    async def set_msg(self, request):
        async with aiohttp.ClientSession() as session:
            async with session.post('http://{}:{}/set_msg'.format(self.hostname, PORT), data=request) as r:
                return response_pb2.Response().FromString(await r.read())

    async def get_msg(self, request):
        async with aiohttp.ClientSession() as session:
            async with session.post('http://{}:{}/get_msg'.format(self.hostname, PORT), data=request) as r:
                return response_pb2.Response().FromString(await r.read())
