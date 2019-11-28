using System;
using Microsoft.AspNetCore.Mvc;

namespace medlink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignInController : ControllerBase
    {
        private readonly IAuthProvider _authProvider;
        private readonly IVendorTokenSource _vendorTokenSource;
        private readonly ISessionProvider _sessionProvider;

        public SignInController(IAuthProvider authProvider, ISessionProvider sessionProvider, IVendorTokenSource vendorTokenSource)
        {
            _authProvider = authProvider;
            _sessionProvider = sessionProvider;
            _vendorTokenSource = vendorTokenSource;
        }

        [HttpPost]
        public string SignIn()
        {
            var formCollection = Request.Form;
            formCollection.TryGetValue("login", out var login);
            formCollection.TryGetValue("password", out var password);

            if (_authProvider.AddUserOrCheckPass(login, password))
            {
                if ( formCollection.TryGetValue("vendorToken", out var token))
                    _vendorTokenSource.SetToken(login, token);
                
                return _sessionProvider.CreateSession(login);
            }
            
            throw new Exception("Users already exist");
        }
    }
}