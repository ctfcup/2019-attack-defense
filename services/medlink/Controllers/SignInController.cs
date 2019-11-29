using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using medlink.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace medlink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignInController : ControllerBase
    {
        private readonly IPasswords _passwords;
        private readonly ISessions _sessions;
        private readonly IVendorInfos _vendorInfos;
        private readonly ISessionSource _sessionSource;

        public SignInController(IPasswords passwords, ISessions sessions,
            IVendorInfos vendorInfos, ISessionSource sessionSource)
        {
            _passwords = passwords;
            _sessions = sessions;
            _vendorInfos = vendorInfos;
            _sessionSource = sessionSource;
        }

        [HttpPost]
        public string SignIn()
        {
            var formCollection = Request.Form;
            formCollection.TryGetValue("login", out var login);
            formCollection.TryGetValue("password", out var password);

            if (AddUserOrCheckPass(login, password))
            {
                if (formCollection.TryGetValue("vendorToken", out var token) && !string.IsNullOrEmpty(token))
                    _vendorInfos.Add(login, new VendorInfo
                    {
                        Token = token,
                        ModelSeries = new HashSet<string>()
                    });

                var session = _sessionSource.GetSession();
                _sessions.Add(session, login);
                return session;
            }

            throw new Exception("Users already exist");
        }
        
        public bool AddUserOrCheckPass(string login, string pass)
        {
            if (_passwords.Contains(login)) 
                return _passwords.Get(login).Equals(Utils.GetSha256(pass));
            
            _passwords.Add(login, Utils.GetSha256(pass));
            return true;

        }
    }
}