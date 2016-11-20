using System;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using NLog;

namespace MyWebService.Logging
{
    public class EsApiCallDetailsLogger : IEsApiCallDetailsLogger
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public Task LogEsApiCallDetailsAsync(IApiCallDetails esApiCallDetails)
        {
            return Task.Run(() => LogEsApiCallDetails(esApiCallDetails));
        }

        public void LogEsApiCallDetails(IApiCallDetails esApiCallDetails)
        {
            // log out the requests
            var requestBody = esApiCallDetails.RequestBodyInBytes != null
                ? string.Format("Request Body: {0}{1}", Encoding.UTF8.GetString(esApiCallDetails.RequestBodyInBytes), Environment.NewLine)
                : string.Empty;

            var responseBody = esApiCallDetails.ResponseBodyInBytes != null
                ? string.Format("Response Body: {0}{1}", Encoding.UTF8.GetString(esApiCallDetails.ResponseBodyInBytes), Environment.NewLine)
                : string.Empty;

            Logger.Trace("{0} {1} {2} {3}{4}{5}",
                esApiCallDetails.HttpMethod.ToString().ToUpperInvariant(),
                esApiCallDetails.Uri,
                esApiCallDetails.HttpStatusCode.ToString().ToUpperInvariant(),
                Environment.NewLine,
                requestBody,
                responseBody);
        }
    }
}