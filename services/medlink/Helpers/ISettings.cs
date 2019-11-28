using System;

namespace medlink.Helpers
{
    public interface ISettings
    {
        string BodyDiagnosticFolder { get; }
        string TelemetryFolder { get; }
        string UserSessionsFolder { get; }
        public string UsersFolder { get; set; }
        string VendorTokensFolder { get; }
        TimeSpan Ttl { get; set; }
        TimeSpan DumpInterval { get; }
        string SessionKey { get; }
    }
}