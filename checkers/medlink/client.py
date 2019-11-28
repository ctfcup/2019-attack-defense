from sys import stderr
import requests
import utils
import json

OK, CORRUPT, MUMBLE, DOWN, CHECKER_ERROR = 101, 102, 103, 104, 110


def close(code, public="", private=""):
    if public:
        print(public)
    if private:
        print(private, file=stderr)
    print('Exit with code %d' % code, file=stderr)
    exit(code)


def register_user(addr):
    url = 'https://%s:%s/api/signin' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    user = {"login": utils.get_name(), "password": utils.get_password()}
    try:
        r = requests.post(url, headers=headers, data=user, verify=False)

        if r.status_code != 200:
            close(MUMBLE, "Submit error", "Invalid status code: %s %d" % (url, r.status_code))

        user["session"] = r.content.decode("UTF-8")
        return user

    except Exception as e:
        close(DOWN, "HTTP Error", "HTTP error: %s" % e)


def register_vendor(addr):
    url = 'https://%s:%s/api/signin' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    user = {
        "login": utils.get_name(),
        "password": utils.get_password(),
        "vendorToken": utils.get_token()
    }
    try:
        r = requests.post(url, headers=headers, data=user, verify=False)

        if r.status_code != 200:
            close(MUMBLE, "Submit error", "Invalid status code: %s %d" % (url, r.status_code))

        user["session"] = r.content.decode("UTF-8")
        return user;

    except Exception as e:
        close(DOWN, "HTTP Error", "HTTP error: %s" % e)


def put_body(addr, session, body_model, token):
    url = 'https://%s:%s/api/bodymodel' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}
    payload = {'vendorToken': token}
    try:
        r = requests.put(url, headers=headers, json=body_model, params=payload, cookies=cookies, verify=False)

        if r.status_code != 200:
            close(MUMBLE, "Submit error", "Invalid status code: %s %d" % (url, r.status_code))

    except Exception as e:
        close(DOWN, "HTTP Error", "HTTP error: %s" % e)


def get_body(addr, session, model, token):
    url = 'https://%s:%s/api/bodymodel' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}
    payload = {'modelSeries': model, 'vendorToken': token}
    try:
        r = requests.get(url, headers=headers, params=payload, cookies=cookies, verify=False)

        if r.status_code != 200:
            close(MUMBLE, "Submit error", "Invalid status code: %s %d" % (url, r.status_code))

        return json.loads(r.content.decode("UTF-8"))

    except Exception as e:
        close(DOWN, "HTTP Error", "HTTP error: %s" % e)


def get_supported_bodies(addr, session):
    url = 'https://%s:%s/api/bodymodels' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}
    try:
        r = requests.get(url, headers=headers, cookies=cookies, verify=False)

        if r.status_code != 200:
            close(MUMBLE, "Submit error", "Invalid status code: %s %d" % (url, r.status_code))

        return json.loads(r.content.decode("UTF-8"))

    except Exception as e:
        close(DOWN, "HTTP Error", "HTTP error: %s" % e)


def get_model_template(addr, session, model):
    url = 'https://%s:%s/api/template' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}
    payload = {'modelSeries': model}
    try:
        r = requests.get(url, headers=headers, params=payload, cookies=cookies, verify=False)

        if r.status_code != 200:
            close(MUMBLE, "Submit error", "Invalid status code: %s %d" % (url, r.status_code))

        return json.loads(r.content.decode("UTF-8"))

    except Exception as e:
        close(DOWN, "HTTP Error", "HTTP error: %s" % e)


def put_telemetry(addr, session, telemetry):
    url = 'https://%s:%s/api/telemetry' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}

    try:
        r = requests.put(url, headers=headers, cookies=cookies, json=telemetry, verify=False)

        if r.status_code != 200:
            close(MUMBLE, "Submit error", "Invalid status code: %s %d" % (url, r.status_code))

    except Exception as e:
        close(DOWN, "HTTP Error", "HTTP error: %s" % e)


def get_telemetry(addr, session):
    url = 'https://%s:%s/api/telemetry' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}

    try:
        r = requests.get(url, headers=headers, cookies=cookies, verify=False)

        if r.status_code != 200:
            close(MUMBLE, "Submit error", "Invalid status code: %s %d" % (url, r.status_code))

        return json.loads(r.content.decode("UTF-8"))

    except Exception as e:
        close(DOWN, "HTTP Error", "HTTP error: %s" % e)


def health_check(addr, session):
    url = 'https://%s:%s/api/healthcheck' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}

    try:
        r = requests.get(url, headers=headers, cookies=cookies, verify=False)

        if r.status_code != 200:
            close(MUMBLE, "Submit error", "Invalid status code: %s %d" % (url, r.status_code))

        return json.loads(r.content.decode("UTF-8"))

    except Exception as e:
        close(DOWN, "HTTP Error", "HTTP error: %s" % e)