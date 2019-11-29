#!/usr/bin/env python3.7
import os
import random
import traceback
import datetime
from api import API
from gornilo import Checker, CheckRequest, GetRequest, PutRequest, Verdict
from string import ascii_lowercase
from random import choices, choice
import re

FLAG_PATTERN = re.compile(r'[A-Za-z0-9]{31}=')
WEEK = 48
YEAR = 2019
checker = Checker()

def grs(k):
    return ''.join(choices(ascii_lowercase, k=k))

def get_reg_creds():
    login = grs(10)
    return {'login': login, 'email': f'{login}@{grs(5)}.{grs(2)}', 'password': grs(25), 'fname': grs(7), 'sname': grs(7)}

def get_current_time():
    return datetime.datetime.now().strftime('%d-%m-%G %H:%M')
    
def get_planner():
    return {'name': grs(10), 'description': grs(15), 'creationTime': get_current_time()}
    
def get_random_day():
    return choice(['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'])

def get_random_time():
    return choice([f'{str(i).zfill(2)}:00' for i in range(6,24,2)])
    
def get_task():
    return {'week': WEEK, "year": YEAR, "day": get_random_day(), 
                "time": get_random_time(), "name":grs(10),"description": grs(25)}
    
@checker.define_check
async def check_service(request: CheckRequest) -> Verdict:
    async with API(request.hostname) as api:
        try:
            creds = get_reg_creds()
            status = await api.registration(creds)
            if status != 302:
                print(f'REGISTRATION: Wait 302, but return {status}')
                return Verdict.MUMBLE("Registration error")
            status = await api.update_user({"login": creds['login'], "additionalInfo":grs(25)})
            if status != 200:
                print(f'UPDATE USER: Wait 200, but return {status}')
                return Verdict.MUMBLE("Can't update user")
            status, res_json = await api.create_planner(get_planner())
            if status != 201 and len(res_json) != 0:
                print(f'CREATE PANNER: Wait 201 with not empty json, but return {status} with {res_json}')
                return Verdict.MUMBLE("Can't create planner")
            planner_id = res_json['id']
            status = await api.create_task(planner_id, get_task())
            if status != 201:
                print(f'CREATE TASK: Wait 201, but return {status}')
                Verdict.MUMBLE("Can't create task")
        except Exception as ex:
            print(traceback.format_exc())
            return Verdict.DOWN('DOWN')
    return Verdict.OK()
    
@checker.define_put(vuln_num=1, vuln_rate=2)
async def put_flag_into_the_service(request: PutRequest) -> Verdict:
    async with API(request.hostname) as api:
        try:
            creds = get_reg_creds()
            status = await api.registration(creds)
            if status != 302:
                print(f'ON PUT 1 REGISTRATION: Wait 302, but return {status}')
                return Verdict.MUMBLE("Registration error")

            status = await api.update_user({"login": creds['login'], "additionalInfo":request.flag})
            if status != 200:
                print(f'ON PUT 1 UPDATE USER: Wait 200, but return {status}')
                return Verdict.MUMBLE("Can't update user")
        except Exception as ex:
            print(traceback.format_exc())
            return Verdict.DOWN('DOWN')
    return Verdict.OK(f"{creds['login']}:{creds['password']}")
    
@checker.define_get(vuln_num=1)
async def get_flag_from_the_service(request: GetRequest) -> Verdict:
    async with API(request.hostname) as api:
        try:
            login, password = request.flag_id.split(":")
            status = await api.login(login, password)
            if status != 200:
                print(f'ON GET 1 LOG IN: Wait 200, but return {status}')
                return Verdict.MUMBLE("Can't log in")
            status,text = await api.get_profile()
            if status != 200:
                print(f'ON GET 1 GET PROFILE: Wait 200, but return {status}')
                return Verdict.MUMBLE("Can't get profile")
            if request.flag not in FLAG_PATTERN.findall(text):
                print(f'ON GET 1 FLAG ERROR: not found')
                return Verdict.CORRUPT("Can't find flag")
        except Exception as ex:
            print(traceback.format_exc())
            return Verdict.DOWN('DOWN')
    return Verdict.OK()
    
@checker.define_put(vuln_num=2, vuln_rate=1)
async def put_second_flag_into_the_service(request: PutRequest) -> Verdict:
    async with API(request.hostname) as api:
        try:
            creds = get_reg_creds()
            status = await api.registration(creds)
            if status != 302:
                print(f'ON PUT 2 REGISTRATION: Wait 302, but return {status}')
                return Verdict.MUMBLE("Registration error")
                
            planner = get_planner()
            status, planner = await api.create_planner(planner)
            if status != 201 or len(planner) == 0:
                print(f'ON GET 2 GET PLANNER: Wait 201, but return {status}')
                return Verdict.MUMBLE("Can't create planner")
            
            status,text = await api.get_planners()
            planner_id = planner["id"]
            
            if status != 200 or len(text.split(f'<tr id="{planner_id}">')) < 1:
                print(f'ON GET 2 GET PLANNERS: Wait 200, but return {status}')
                return Verdict.MUMBLE("Can't get planner")
            
            task = get_task()
            task['description'] = request.flag
            status = await api.create_task(planner_id, task)
            if status != 201:
                print(f'ON GET 2 CREATE TASK: Wait 201, but return {status}')
                Verdict.MUMBLE("Can't create task")          
            
        except Exception as ex:
            print(traceback.format_exc())
            return Verdict.DOWN('DOWN')
    return Verdict.OK(f"{creds['login']}:{creds['password']}:{planner['id']}")
    
@checker.define_get(vuln_num=2)
async def get_flag_from_the_service(request: GetRequest) -> Verdict:
    async with API(request.hostname) as api:
        try:
            login, password, planner_id = request.flag_id.split(":")
            status = await api.login(login, password)
            if status != 200:
                print(f'ON PUT 2 LOG IN:Wait 200, but return {status}')
                return Verdict.MUMBLE("Can't log in")
            
            status, tasks = await api.get_tasks(planner_id, WEEK, YEAR)
            if status != 200 or len(tasks) == 0:
                print(f'ON PUT 2 GET TASKS: Wait 200, but return {status} without tasks')
                return Verdict.MUMBLE("Can't get tasks")

            for task in tasks:
                if request.flag in task['description']:
                    return Verdict.OK()
        except Exception as ex:
            print(traceback.format_exc())
            return Verdict.DOWN('DOWN')
    print(f'ON GET 2 flag not found')
    return Verdict.CORRUPT("Can't find flag")

if __name__ == "__main__":
    checker.run()