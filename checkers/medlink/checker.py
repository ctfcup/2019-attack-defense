from __future__ import print_function
from sys import argv
import random
from client import *

SERVICE_NAME = "geoapi"
OK, CORRUPT, MUMBLE, DOWN, CHECKER_ERROR = 101, 102, 103, 104, 110


def close(code, public="", private=""):
    if public:
        print(public)
    if private:
        print(private, file=stderr)
    print('Exit with code %d' % code, file=stderr)
    exit(code)


def check(*args):
    addr = args[0]

    vendor = register_vendor(addr)

    vendor_session = vendor["session"]
    v_token = vendor["vendorToken"]
    body_model = utils.get_body_model()
    put_body(addr, vendor_session, body_model, v_token)
    bodies = get_supported_bodies(addr, vendor_session)

    if body_model["modelSeries"] not in bodies:
        close(MUMBLE)

    received = get_body(addr, vendor_session, body_model["modelSeries"], v_token)

    if body_model["modelSeries"] != received["modelSeries"]:
        close(CORRUPT)

    for key in body_model["referenceValues"].keys():
        if key not in received["referenceValues"] or body_model["referenceValues"][key] != received["referenceValues"][key]:
            close(CORRUPT)

    template = get_model_template(addr, vendor_session, body_model["modelSeries"])

    lower_temp = {v.lower(): v for v in template}

    for key in body_model["referenceValues"].keys():
        if key not in lower_temp:
            close(CORRUPT)

    user = register_user(addr)
    user_session = user["session"]
    user_model = random.choice(bodies)
    template = get_model_template(addr, user_session, user_model)

    telemetry = {
        "bodyID": utils.get_bodyid(),
        "bodyModelSeries":user_model,
        "hardwareTelemetry": {}
    }

    for parameter in template:
        telemetry["hardwareTelemetry"][parameter] = random.randrange(0, 100)

    put_telemetry(addr, user_session, telemetry)
    r_telemetry = get_telemetry(addr, user_session)

    if telemetry["bodyID"] != r_telemetry["bodyID"]:
        close(CORRUPT)

    for key in r_telemetry["hardwareTelemetry"].keys():
        if r_telemetry["hardwareTelemetry"][key] != telemetry["hardwareTelemetry"][key]:
            close(CORRUPT)

    r_check = health_check(addr, user_session)

    for key in r_check["checkResults"].keys():
        if key not in template:
            close(CORRUPT)

    close(OK)


def put(*args):
    addr = args[0]
    flag = args[2]
    vuln = args[3]

    if vuln == 1:
        put_first_flag(addr, flag)

    if vuln == 2:
        put_second_flag(addr, flag)


def get(*args):
    addr = args[0]
    flag_id = args[1]
    flag = args[2]
    vuln = args[3]

    if vuln == 1:
        get_first_flag(addr, flag_id, flag)

    if vuln == 2:
        get_second_flag(addr, flag_id, flag)

    close(OK)


def put_first_flag(addr, flag):
    user = register_user(addr)
    user_session = user["session"]
    bodies = get_supported_bodies(addr, user_session)
    user_model = random.choice(bodies)
    template = get_model_template(addr, user_session, user_model)

    telemetry = {
        "bodyID": flag,
        "bodyModelSeries": user_model,
        "hardwareTelemetry": {}
    }

    for parameter in template:
        telemetry["hardwareTelemetry"][parameter] = random.randrange(0, 100)

    put_telemetry(addr, user_session, telemetry)
    close(OK, user_session)


def put_second_flag(addr, flag):
    vendor = register_vendor(addr)
    vendor_session = vendor["session"]
    v_token = vendor["vendorToken"]
    body_model = utils.get_body_model()
    body_model["bodySeriesFamilyToken"] = flag
    put_body(addr, vendor_session, body_model, v_token)

    close(OK, json.dumps({"session":vendor_session, "vendorToken":v_token}))


def get_first_flag(addr, flag_id, flag):
    r_telemetry = get_telemetry(addr, flag_id)

    if r_telemetry["bodyID"] != flag:
        close(CORRUPT)


def get_second_flag(addr, flag_id, flag):
    user = json.load(flag_id)
    r_telemetry = get_body(addr, user["session"], user["vendorToken"])

    if r_telemetry["bodySeriesFamilyToken"] != flag:
        close(CORRUPT)


def info(*args):
    close(OK, "vulns: 2")


def not_found(*args):
    print("Unsupported command %s" % argv[1], file=stderr)
    return CHECKER_ERROR

COMMANDS = {'check': check, 'put': put, 'get': get, 'info': info}

check("localhost")

if __name__ == '__main__':
    try:
        COMMANDS.get(argv[1], not_found)(*argv[2:])
    except Exception as e:
        close(CHECKER_ERROR, "Evil checker", "INTERNAL ERROR: %s" % e)
