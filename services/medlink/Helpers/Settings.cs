using System;

namespace medlink.Helpers
{
    public class Settings : ISettings
    {
        public int ApiPort { get; set; } = 9007;
        public string BodyModelsFolder { get; set; } = "./data/user";
        public string TelemetryFolder { get; set; } = "./data/telemetry";
        public string UserSessionsFolder { get; set; }= "./data/sessions";
        public string UsersFolder { get; set; }= "./data/users";
        public string VendorTokensFolder { get; set; } = "./data/tokens";
        public TimeSpan Ttl { get; set; } = new TimeSpan(1, 30, 0);
        public TimeSpan DumpInterval { get; set; } = TimeSpan.FromMilliseconds(100);
        public string SessionKey { get; set; } = "medlinkToken";
    }
}