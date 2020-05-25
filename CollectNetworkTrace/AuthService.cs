using System;
using Microsoft.Identity.Client;


namespace CollectNetworkTrace
{
    class AuthService
    {
        const string ScopeEndpoint = "https://management.core.windows.net//.default";
        public static string GetAcccessToken(NetworkTraceConfiguation config)
        {
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                                                      .WithClientSecret(config.Secret)
                                                      .WithTenantId(config.TenantId)
                                                      .Build();

            string[] scopes = new string[] { ScopeEndpoint };

            var result = app.AcquireTokenForClient(scopes)
                .ExecuteAsync()
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Utility.Trace("Token acquired");
            return result.AccessToken;
        }
    }
}
