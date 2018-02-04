using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreSample.Service.Models.HealthCheck
{
    /// <summary>
    /// A check item class holds logic for checking an (external) web service
    /// </summary>
    internal class WebServiceLiveCheckItem : ILiveCheckItem
    {
        private string ServiceUrl { get; set; }

        public WebServiceLiveCheckItem(string serviceUrl)
        {
            ServiceUrl = serviceUrl;
        }

        public async Task<dynamic> ExecuteAsync()
        {
            var result = new WebServiceLiveCheckResult
            {
                ServiceUrl = ServiceUrl
            };

            // HTTPClient can throw on things like DNS resolution, so wrapping into
            // a try catch on top of the status code logic
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(ServiceUrl);
                    result.CanConnect = response.IsSuccessStatusCode;
                    result.StatusCode = response.StatusCode;

                    if (!response.IsSuccessStatusCode)
                    {
                        result.Details = response;
                    }
                }
            }
            catch (Exception ex)
            {
                result.CanConnect = false;
                result.Details = ex;
            }

            return result;
        }
    }
}
