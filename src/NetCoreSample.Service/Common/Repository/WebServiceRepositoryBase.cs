using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreSample.Service.Common.Repository
{
    /// <summary>
    /// Wraps HttpClient to provide deserialization and response status code check
    /// (and, in the future, other common concerns like authentication and correlation id handling)
    /// </summary>
    public class WebServiceRepositoryBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly HttpClient HttpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        public WebServiceRepositoryBase(HttpClient httpClient)
        {
            HttpClient = httpClient;

            AddDefaultRequestHeaders(HttpClient.DefaultRequestHeaders);
        }

        /// <summary>
        /// Makes an HTTP GET request to the given path and returns the deserialized content from the response
        /// or throws HttpRequestException if call was not successful.
        /// </summary>
        /// <typeparam name="T">Type that response content should be deserialized into</typeparam>
        /// <param name="path">Path for request</param>
        /// <returns>Response content, deserialized into given type</returns>
        public async Task<T> GetAsync<T>(string path)
        {
            return await SendAsync<T>(path, HttpMethod.Get);
        }

        /// <summary>
        /// Makes an HTTP POST request to the given path with the given value, serialized as JSON, as the request body
        /// and returns the deserialized content from the response, or throws HttpRequestException if call was not successful.
        /// </summary>
        /// <typeparam name="TRequest">Type of the "value" parameter (request body)</typeparam>
        /// <typeparam name="TResponse">Type that response content should be deserialized into</typeparam>
        /// <param name="path">Path for request</param>
        /// <param name="value">Content to be sent as request body</param>
        /// <returns>Response content, deserialized into given type</returns
        public async Task<TResponse> PostAsJsonAsync<TRequest, TResponse>(string path, TRequest value)
        {
            return await SendAsync<TResponse>(path, HttpMethod.Post, ConstructJsonHttpContent<TRequest, TResponse>(value));
        }

        /// <summary>
        /// Makes an HTTP POST request to the given path with the given value, serialized as JSON, as the request body
        /// and returns the deserialized content from the response, or throws HttpRequestException if call was not successful.
        /// (Overload for when TRequest and TResponse are the same type). 
        /// </summary>
        /// <typeparam name="T">Type of both the "value" parameter (request body) and that response content should be deserialized into</typeparam>
        /// <param name="path">Path for request</param>
        /// <param name="value">Content to be sent as request body</param>
        /// <returns>Response content, deserialized into given type</returns>
        public async Task<T> PostAsJsonAsync<T>(string path, T value)
        {
            return await PostAsJsonAsync<T, T>(path, value);
        }

        /// <summary>
        /// Makes an HTTP PUT request to the given path with the given value, serialized as JSON, as the request body
        /// and returns the deserialized content from the response, or throws HttpRequestException if call was not successful.
        /// </summary>
        /// <typeparam name="TRequest">Type of the "value" parameter (request body)</typeparam>
        /// <typeparam name="TResponse">Type that response content should be deserialized into</typeparam>
        /// <param name="path">Path for request</param>
        /// <param name="value">Content to be sent as request body</param>
        /// <returns>Response content, deserialized into given type</returns>
        public async Task<TResponse> PutAsJsonAsync<TRequest, TResponse>(string path, TRequest value)
        {
            return await SendAsync<TResponse>(path, HttpMethod.Put, ConstructJsonHttpContent<TRequest, TResponse>(value));
        }

        /// <summary>
        /// Makes an HTTP PUT request to the given path with the given value, serialized as JSON, as the request body
        /// and returns the deserialized content from the response, or throws HttpRequestException if call was not successful.
        /// (Overload for when TRequest and TResponse are the same type). 
        /// </summary>
        /// <typeparam name="T">Type of both the "value" parameter (request body) and that response content should be deserialized into</typeparam>
        /// <param name="path">Path for request</param>
        /// <param name="value">Content to be sent as request body</param>
        /// <returns>Response content, deserialized into given type</returns>
        public async Task<T> PutAsJsonAsync<T>(string path, T value)
        {
            return await PutAsJsonAsync<T, T>(path, value);
        }

        /// <summary>
        /// Makes an HTTP DELETE request to the given path and returns the deserialized content from the response
        /// or throws HttpRequestException if call was not successful.
        /// </summary>
        /// <typeparam name="T">Type that response content should be deserialized into</typeparam>
        /// <param name="path">Path for request</param>
        /// <returns>Response content, deserialized into given type</returns>
        public async Task<T> Delete<T>(string path)
        {
            return await SendAsync<T>(path, HttpMethod.Delete);
        }

        /// <summary>
        /// Construct <see cref="HttpContent"/> by serializing specified value into Json
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected HttpContent ConstructJsonHttpContent<TRequest, TResponse>(TRequest value)
        {
            string serializedValue = JsonConvert.SerializeObject(value); // warning, not optimized for large objects
            HttpContent content = new StringContent(serializedValue, Encoding.UTF8, "application/json");
            return content;
        }

        /// <summary>
        /// Dispatch 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected async Task<T> SendAsync<T>(string path, HttpMethod method, HttpContent content = null)
        {
            var request = BuildRequest(path, method, content);

            return await SendRequestAsync<T>(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpRequestMessage"></param>
        /// <returns></returns>
        protected async Task<T> SendRequestAsync<T>(HttpRequestMessage httpRequestMessage)
        {
            HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        /// <summary>
        /// Build <see cref="HttpRequestMessage"/> from the provided component
        /// </summary>
        /// <param name="path"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected virtual HttpRequestMessage BuildRequest(string path, HttpMethod method, HttpContent content = null)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(HttpClient.BaseAddress, path),
                Method = method
            };

            AddDefaultRequestHeaders(request.Headers);

            if (content != null)
            {
                request.Content = content;
            }

            return request;
        }

        private static void AddDefaultRequestHeaders(HttpRequestHeaders headers)
        {
            headers.Add("Accept", "application/json");
        }
    }
}