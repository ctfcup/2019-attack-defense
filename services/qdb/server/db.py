import aioredis

EXPIRE_TIME = 15*60*1000

class DB:
    def __init__(self):
        self.addr = 'redis://redis:6379/'
        self.db = None

    async def get_db(self):
        if self.db is None:
            self.db = await aioredis.create_redis(self.addr)
        return self.db

    async def get_logins(self):
        db = await self.get_db()
        return await db.keys('*')

    async def check_login(self, login):
        db = await self.get_db()
        return await db.exists(login)

    async def set_msg(self, login, msg):
        db = await self.get_db()
        await db.set(login, msg)
        await db.pexpire(login, EXPIRE_TIME)

    async def get_msg(self, login):
        db = await self.get_db()
        return await db.get(login)