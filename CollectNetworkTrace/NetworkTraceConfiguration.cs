using System;
using System.Configuration;

namespace CollectNetworkTrace
{
    public class NetworkTraceConfiguation
    {
        private string _siteName;

        public string TenantId { get; internal set; }
        public string ClientId { get; internal set; }
        public string Secret { get; internal set; }
        public string SiteName
        {
            get
            {
                string siteName = _siteName;
                if (!string.IsNullOrWhiteSpace(SlotName))
                {
                    siteName = $"{siteName}({SlotName})";
                }
                return siteName;
            }
            internal set
            {
                _siteName = value;
            }
        }
        public string ResourceGroup { get; internal set; }
        public string SubscriptionId { get; internal set; }
        public string SlotName { get; private set; }

        public static NetworkTraceConfiguation GetConfiguration()
        {
            var secret = new NetworkTraceConfiguation()
            {
                TenantId = GetConfigurationValue("NETWORKTRACE_TENANTID"),
                ClientId = GetConfigurationValue("NETWORKTRACE_CLIENTID"),
                Secret = GetConfigurationValue("NETWORKTRACE_CLIENTSECRET"),
                SiteName = GetEnvironmentVariable("WEBSITE_SITE_NAME"),
                ResourceGroup =   GetEnvironmentVariable("WEBSITE_RESOURCE_GROUP"),
                SubscriptionId = GetSubscriptionId(),
                SlotName = ConfigurationManager.AppSettings["NETWORKTRACE_SLOTNAME"]
            };

            return secret;
        }

        private static string GetSubscriptionId()
        {
            var webSiteOwner = GetEnvironmentVariable("WEBSITE_OWNER_NAME");
            return webSiteOwner.Split('+')[0];
        }

        private static string GetEnvironmentVariable(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ApplicationException($"Failed to read environment variable {name}");
            }
            return value;
        }

        private static string GetConfigurationValue(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            var envVar = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrEmpty(envVar))
            {
                value = envVar;
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ApplicationException($"Key {key} is not set in configuration");
            }
            if (value.StartsWith("your", StringComparison.OrdinalIgnoreCase))
            {
                throw new ApplicationException($"Please specify the setting {key} either in CollectNetworkTrace.exe.config or as an APP SETTING for the app");
            }
            return value;
        }
    }
}
