using System.Threading.Tasks;
using Elasticsearch.Net;

namespace MyWebService.Logging
{
    /// <summary>
    /// Custom logger to log ElasticSearch API calls
    /// </summary>
    public interface IEsApiCallDetailsLogger
    {
        /// <summary>
        /// Synchronus logging 
        /// </summary>
        /// <param name="esApiCallDetails">The API call details to log</param>
        void LogEsApiCallDetails(IApiCallDetails esApiCallDetails);

        /// <summary>
        /// Asynchronus logging 
        /// </summary>
        /// <param name="esApiCallDetails">The API call details to log</param>
        /// <returns>The task handle for the asynchronous logging action</returns>
        Task LogEsApiCallDetailsAsync(IApiCallDetails esApiCallDetails);
    }
}
