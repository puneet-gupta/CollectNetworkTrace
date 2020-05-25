using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Newtonsoft.Json;

namespace CollectNetworkTrace
{
    internal class ArmService
    {

        const string ArmEndpoint = "management.azure.com";
        private readonly HttpClient _client = new HttpClient();
        private readonly NetworkTraceConfiguation _config = null;

        internal ArmService(NetworkTraceConfiguation config, string authToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _config = config;
        }

        public void StartNetworkTrace(int duration)
        {
            string url = $"https://{ArmEndpoint}/subscriptions/{_config.SubscriptionId}/resourceGroups/{_config.ResourceGroup}/providers/Microsoft.Web/sites/{_config.SiteName}/networkTrace/start?durationInSeconds={duration}&api-version=2015-08-01";

            Utility.Trace($"Invoking {url}");
            var response = _client.PostAsync(url, null).Result;

            if (response.IsSuccessStatusCode)
            {
                if (response.Headers.TryGetValues("Location", out IEnumerable<string> values))
                {
                    string armOperationStatus = values.FirstOrDefault();
                    Utility.Trace($"Sleeping for {duration} seconds");
                    Thread.Sleep(duration * 1000);
                    var networkTraceFile = GetTraceFile(armOperationStatus);
                    Utility.Trace($"Trace File captured at {networkTraceFile} hence exiting...");
                }
            }
            else
            {
                Utility.Trace($"Failed with HTTP {response.StatusCode} while starting the trace. Response message is '{response.Content.ReadAsStringAsync().Result}");
            }
        }

        private string GetTraceFile(string armOperationStatus)
        {
            var traceFile = "";
            bool isRunning = true;
            while (isRunning)
            {
                var response = _client.GetAsync(armOperationStatus).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    var results = JsonConvert.DeserializeObject<NetworkTraceResult[]>(responseString);

                    if (results.Any())
                    {
                        isRunning = !(string.Equals(results.FirstOrDefault().Status, "Succeeded", StringComparison.OrdinalIgnoreCase));
                        if (isRunning)
                        {
                            Thread.Sleep(15 * 1000);
                        }
                        else
                        {
                            traceFile = results.FirstOrDefault().Path;
                        }
                    }
                }
            }
            return traceFile;
        }
    }
}
