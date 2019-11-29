using System;
using medlink.Helpers;

namespace medlink
{
    public class SessionSource : ISessionSource
    {
        private readonly ISessions _sessions;
        private string _seed = "";

        public SessionSource(ISessions sessions)
        {
            _sessions = sessions;
        }

        public string GetSession()
        {
            var timePart = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            var secureKeyPart = Utils.GetSha256Bytes(_seed);
            var idPart = BitConverter.GetBytes(_sessions.Count);
            var data = new byte[timePart.Length + secureKeyPart.Length + idPart.Length];
            
            Buffer.BlockCopy(timePart, 0, data, 0, timePart.Length);
            Buffer.BlockCopy(secureKeyPart, 0, data, timePart.Length, secureKeyPart.Length);
            Buffer.BlockCopy(idPart, 0, data, timePart.Length + secureKeyPart.Length, idPart.Length);
            
            _seed = Guid.NewGuid().ToString();
            
            return Convert.ToBase64String(data);
        }
        
        public enum TokenValidationStatus
        {
            Expired,
            WrongUser,
            WrongToken
        }

        // public string CreateSession(string login)
        // {
        //     var now = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
        //     var sessionKey = Guid.NewGuid().ToByteArray();
        //     var loginPart = Encoding.UTF8.GetBytes(login);
        //     var data = new byte[now.Length + sessionKey.Length + loginPart.Length];
        //
        //     Buffer.BlockCopy(now, 0, data, 0, now.Length);
        //     Buffer.BlockCopy(sessionKey, 0, data, now.Length, sessionKey.Length);
        //     Buffer.BlockCopy(loginPart, 0, data, now.Length + sessionKey.Length, loginPart.Length);
        //
        //     var session = Convert.ToBase64String(data.ToArray());
        //     Index[session] = login;
        //     return session;
        // }
        //
        // public bool CheckSession(string session, out string login)
        // {
        //     if (Index.ContainsKey(session))
        //     {
        //         login = Index[session];
        //         return true;
        //     }
        //
        //     login = null;
        //     return false;
        // }
        //
        // public TokenValidation ValidateToken(string session)
        // {
        //     var result = new TokenValidation();
        //     var data = Convert.FromBase64String(session);
        //     var createdAt = data.Take(8).ToArray();
        //     var sessionKey = Encoding.UTF8.GetString(data.Skip(8).Take(16).ToArray());
        //     var login = Encoding.UTF8.GetString(data.Skip(28).ToArray());
        //
        //     var when = DateTime.FromBinary(BitConverter.ToInt64(createdAt, 0));
        //     if (when < DateTime.UtcNow.AddHours(-24))
        //     {
        //         result.Errors.Add(TokenValidationStatus.Expired);
        //         return result;
        //     }
        //
        //     if (Index.ContainsKey(sessionKey))
        //     {
        //         result.Errors.Add(TokenValidationStatus.WrongUser);
        //         return result;
        //     }
        //
        //     if (Index[sessionKey].Equals(login))
        //     {
        //         result.Errors.Add(TokenValidationStatus.WrongToken);
        //         return result;
        //     }
        //
        //
        //     return result;
        // }
        //
        // public class TokenValidation
        // {
        //     public readonly List<TokenValidationStatus> Errors = new List<TokenValidationStatus>();
        //     public bool Validated => Errors.Count == 0;
        // }
    }
}