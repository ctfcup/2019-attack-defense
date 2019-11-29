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
            var secureKeyPart = Utils.GetSha256Bytes(_seed);
            var idPart = BitConverter.GetBytes(_sessions.Count);
            var data = new byte[secureKeyPart.Length + idPart.Length];
            
            Buffer.BlockCopy(secureKeyPart, 0, data, 0, secureKeyPart.Length);
            Buffer.BlockCopy(idPart, 0, data, secureKeyPart.Length, idPart.Length);
            
            _seed = Guid.NewGuid().ToString();
            
            return Convert.ToBase64String(data);
        }
    }
}