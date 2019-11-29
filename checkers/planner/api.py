import aiohttp
from aiohttp.client import ClientTimeout
from networking.masking_connector import get_agent


PORT = 1337

class API:
    def __init__(self, hostname):
        self.hostname = hostname
        self.url = f'http://{hostname}:{PORT}'
        self.session = aiohttp.ClientSession(timeout=ClientTimeout(total=100), 
                                                headers={"User-Agent": get_agent()},
                                                cookie_jar=aiohttp.CookieJar(unsafe=True))

    async def __aenter__(self):
        return self

    async def __aexit__(self, exc_type, exc_val, exc_tb):
        await self.session.close()
        
    async def get_json(self, resp, status=200):
        return await resp.json() if resp.status == status else {}
     
    async def registration(self, registration_creds):
        async with self.session.post(f'{self.url}/registration', data=registration_creds, allow_redirects=False) as r:
            return r.status
                
    async def update_user(self, update_fields):
        async with self.session.post(f'{self.url}/profile', data=update_fields, allow_redirects=False) as r:
            return r.status
                
    async def create_planner(self, planner):
        async with self.session.post(f'{self.url}/planners', json=planner, allow_redirects=False) as r:
            return r.status, await self.get_json(r, 201)

    async def create_task(self, planner_id, task):
        async with self.session.post(f'{self.url}/planners/{planner_id}', json=task, allow_redirects=False) as r:
            return r.status
    
    async def login(self, login, password):
        async with self.session.post(f'{self.url}/login', data={'login':login, 'password':password}, allow_redirects=False) as r:
            return r.status
            
    async def get_profile(self):
        async with self.session.get(f'{self.url}/profile', allow_redirects=False) as r:
            return r.status, await r.text()
            
    async def get_planners(self):
        async with self.session.get(f'{self.url}/planners', allow_redirects=False) as r:
            return r.status, await r.text()
    
    async def get_tasks(self, planner_id, week, year):
        async with self.session.get(f'{self.url}/planners/{planner_id}/tasks?week={week}&year={year}', allow_redirects=False) as r:
            return r.status, await self.get_json(r)
