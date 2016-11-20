using System.Net;

namespace MyWebService.Models.HealthCheck
{
    /// <summary>
    /// DTO to hold check result of (external) web service
    /// </summary>
    public class WebServiceLiveCheckResult
    {
        /// <summary>
        /// URL checked
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Indicates whether the check was able to connect to the service
        /// </summary>
        public bool CanConnect { get; set; }

        /// <summary>
        /// The status code read from the connection
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Additional details from the context
        /// </summary>
        public dynamic Details { get; set; }
    }
}
