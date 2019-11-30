from sys import stderr
import requests
import utils
import json

from gornilo import Verdict


def close(code, public="", private=""):
    if public:
        print(public)
    if private:
        print(private, file=stderr)
    print('Exit with code %d' % code, file=stderr)
    exit(code)


def register_user(addr):
    url = 'http://%s:%s/api/signin' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    user = {"login": utils.get_name(), "password": utils.get_password()}
    r = ensure_success(lambda: requests.post(url, headers=headers, data=user, verify=False))

    user["session"] = r.content.decode("UTF-8")
    return user


def register_vendor(addr):
    url = 'http://%s:%s/api/signin' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    user = {
        "login": utils.get_name(),
        "password": utils.get_password(),
        "vendorToken": utils.get_token()
    }
    r = ensure_success(lambda: requests.post(url, headers=headers, data=user, verify=False))

    user["session"] = r.content.decode("UTF-8")
    return user


def put_body(addr, session, body_model, token):
    url = 'http://%s:%s/api/bodymodel' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}
    payload = {'vendorToken': token}
    r = ensure_success(
        lambda: requests.put(url, headers=headers, json=body_model, params=payload, cookies=cookies, verify=False))


def get_body(addr, session, model, revision, token):
    url = 'http://%s:%s/api/bodymodel' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}
    payload = {'modelSeries': model, 'revision': revision, 'vendorToken': token}
    r = ensure_success(lambda: requests.get(url, headers=headers, params=payload, cookies=cookies, verify=False))

    return json.loads(r.content.decode("UTF-8"))


def get_supported_bodies(addr, session):
    url = 'http://%s:%s/api/bodymodels' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}
    r = ensure_success(lambda: requests.get(url, headers=headers, cookies=cookies, verify=False))

    return json.loads(r.content.decode("UTF-8"))


def get_model_template(addr, session, model, revision):
    url = 'http://%s:%s/api/template' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}
    payload = {'modelSeries': model, 'revision': revision}

    i = 0

    while i < 3:
        r = ensure_success(lambda: requests.get(url, headers=headers, params=payload, cookies=cookies, verify=False))
        conent = r.content.decode("UTF-8")
        try:
            if conent != None:
                return json.loads(conent)
        except Exception:
            print(i)

        i += 1
        print(r)

        return Verdict.CHECKER_ERROR("Check api/template or cleanup folder and restart container :)")


def put_telemetry(addr, session, telemetry):
    url = 'http://%s:%s/api/telemetry' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}
    ensure_success(lambda: requests.put(url, headers=headers, cookies=cookies, json=telemetry, verify=False))


def get_telemetry(addr, session):
    url = 'http://%s:%s/api/telemetry' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}

    r = ensure_success(lambda: requests.get(url, headers=headers, cookies=cookies, verify=False))

    return json.loads(r.content.decode("UTF-8"))


def health_check(addr, session):
    url = 'http://%s:%s/api/healthcheck' % (addr, utils.get_port())
    headers = {'User-Agent': utils.get_user_agent()}
    cookies = {'medlinkToken': session}

    r = ensure_success(lambda: requests.get(url, headers=headers, cookies=cookies, verify=False))
    return json.loads(r.content.decode("UTF-8"))


def ensure_success(request):
    try:
        r = request()
    except Exception as e:
        raise HTTPException(Verdict.DOWN("HTTP error"))
    if r.status_code != 200:
        raise HTTPException(Verdict.MUMBLE("Invalid status code: %s" % r.status_code))
    return r


class HTTPException(Exception):
    def __init__(self, verdict=None):
        self.verdict = verdict  # you could add more args

    def __str__(self):
        return str(self.verdict)
