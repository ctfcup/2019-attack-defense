using System;
using System.Threading.Tasks;
using medlink.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace medlink.Controllers
{
    public abstract class SessionControllerBase<TValue> : ControllerBase
    {
        private readonly ISessionProvider _sessionProvider;
        private readonly ISettings _settings;

        public SessionControllerBase(ISettings settings, ISessionProvider sessionProvider)
        {
            _settings = settings;
            _sessionProvider = sessionProvider;
        }

        protected string Session => Request.Cookies[_settings.SessionKey];

        protected bool IsAuthorized()
        {
            return Request.Cookies.ContainsKey(_settings.SessionKey);
        }

        protected async Task<TValue> HandleAuthorizedRequest(Func<string, Task<TValue>> handler)
        {
            if (!IsAuthorized() || !_sessionProvider.CheckSession(Session, out var login))
            {
                Response.StatusCode = 403;
                return default;
            }

            return await handler(login);
        }

        protected async Task HandleAuthorizedRequest(Func<string, Task> handler)
        {
            if (!IsAuthorized() || !_sessionProvider.CheckSession(Session, out var login))
            {
                Response.StatusCode = 403;
                return;
            }

            await handler(login);
        }
    }
}