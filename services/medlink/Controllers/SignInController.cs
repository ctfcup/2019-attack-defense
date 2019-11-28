using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace medlink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignInController : ControllerBase
    {
        private readonly IUsers _users;
        private readonly ISessions _sessions;
        private readonly IVendorTokens _vendorTokens;
        private readonly ISessionSource _sessionSource;

        public SignInController(IUsers users, ISessions sessions,
            IVendorTokens vendorTokens, ISessionSource sessionSource)
        {
            _users = users;
            _sessions = sessions;
            _vendorTokens = vendorTokens;
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
                if (formCollection.TryGetValue("vendorToken", out var token))
                    _vendorTokens.Add(login, token);

                var session = _sessionSource.GetSession();
                _sessions.Add(session, login);
                return session;
            }

            throw new Exception("Users already exist");
        }
        
        public bool AddUserOrCheckPass(string login, string pass)
        {
            if (_users.Contains(login)) 
                return _users.Get(login).Equals(GetHash(pass));
            
            _users.Add(login, GetHash(pass));
            return true;

        }

        private string GetHash(string source)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(source));

                return Encoding.Default.GetString(bytes);
            }
        }
    }
}