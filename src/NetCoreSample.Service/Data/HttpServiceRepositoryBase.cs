using System;
using System.Net.Http;

namespace NetCoreSample.Service.Data
{
    internal abstract class HttpServiceRepositoryBase
    {
        protected HttpClient HttpClient { get; }

        protected HttpServiceRepositoryBase(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        protected abstract Uri BaseUri { get; }

        protected Uri GetFullUri(string partialUrl)
        {
            return new Uri(BaseUri, partialUrl);
        }
    }
}
