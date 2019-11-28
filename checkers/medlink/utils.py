import random
import uuid

UserAgents = None
Names = None


def get_user_agent():
    global UserAgents
    if UserAgents is None:
        with open('user-agents') as fin:
            UserAgents = [line.strip() for line in fin]
    return random.choice(UserAgents)


def get_name():
    global Names
    if Names is None:
        with open('user-agents') as fin:
            Names = [line.strip() for line in fin]
    return str(uuid.uuid4())
        #random.choice(Names)


def get_password():
    return str(uuid.uuid4())


def get_token():
    return str(uuid.uuid4());


def get_bodyid():
    return str(uuid.uuid4());


def get_port():
    return 44364


def get_body_model():
    return {
        "modelSeries": "AzazaTech_1",
        "bodySeriesFamilyToken": str(uuid.uuid4()),
        "referenceValues": {
            "heart tem.": 50.0,
            "oil pressure": 30.0,
        }
    }
