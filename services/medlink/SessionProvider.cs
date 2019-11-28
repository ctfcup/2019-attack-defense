using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using medlink.Helpers;
using medlink.Storage;

namespace medlink
{
    public interface ISessionProvider
    {
        string CreateSession(string login);
        bool CheckSession(string session, out string login);
    }

    public class SessionProvider : FileBasedIndex<Dictionary<string,string>>, ISessionProvider
    {
        private string seed = "";

        public SessionProvider(ISettings settings, IFileDumper fileDumper) : 
            base(fileDumper, new Dictionary<string, string>(), settings.UserSessionsFolder)
        {
        }

        public string CreateSession(string login)
        {
            var now = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            var sessionKey = Guid.NewGuid().ToByteArray();
            var loginPart = Encoding.UTF8.GetBytes(login);
            var data = new byte[now.Length + sessionKey.Length + loginPart.Length];

            Buffer.BlockCopy(now, 0, data, 0, now.Length);
            Buffer.BlockCopy(sessionKey, 0, data, now.Length, sessionKey.Length);
            Buffer.BlockCopy(loginPart, 0, data, now.Length + sessionKey.Length , loginPart.Length);

            var session = Convert.ToBase64String(data.ToArray());
            Index[session] = login;
            return session;
        }

        public bool CheckSession(string session, out string login)
        {
            if (Index.ContainsKey(session))
            {
                login = Index[session];
                return true;
            }

            login = null;
            return false;
        }

        public TokenValidation ValidateToken(string session)
        {
            var result = new TokenValidation();
            var data = Convert.FromBase64String(session);
            var createdAt = data.Take(8).ToArray();
            var sessionKey = Encoding.UTF8.GetString( data.Skip(8).Take(16).ToArray());
            var login= Encoding.UTF8.GetString( data.Skip(28).ToArray());

            var when = DateTime.FromBinary(BitConverter.ToInt64(createdAt, 0));
            if (when < DateTime.UtcNow.AddHours(-24))
            {
                result.Errors.Add(TokenValidationStatus.Expired);
                return result;
            }

            if (Index.ContainsKey(sessionKey))
            {
                result.Errors.Add(TokenValidationStatus.WrongUser);
                return result;
            }

            if (Index[sessionKey].Equals(login))
            {
                result.Errors.Add(TokenValidationStatus.WrongToken);
                return result;
            }
            

            return result;
        }

        public class TokenValidation
        {
            public bool Validated { get { return Errors.Count == 0; } }
            public readonly List<TokenValidationStatus> Errors = new List<TokenValidationStatus>();
        }

        public enum TokenValidationStatus
        {
            Expired,
            WrongUser,
            WrongToken,
        }

    }
}