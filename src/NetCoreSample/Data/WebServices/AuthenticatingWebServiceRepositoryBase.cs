using System.Net.Http;
using System.Threading.Tasks;
using NetCoreSample.Application.Authentication;

namespace NetCoreSample.Data.WebServices
{
    /// <summary>
    /// An extension base of Web Service backed repository, that also provides options to
    /// attach authentication to service calls.
    /// </summary>
    public class AuthenticatingWebServiceRepositoryBase : WebServiceRepositoryBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly IAuth0Provider Auth0Provider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="auth0Provider"></param>
        public AuthenticatingWebServiceRepositoryBase(
            HttpClient httpClient,
            IAuth0Provider auth0Provider)
        : base(httpClient)
        {
            Auth0Provider = auth0Provider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<T> GetAsyncWithAuthentication<T>(string path)
        {
            return await SendAsyncWithAuthentication<T>(path, HttpMethod.Get);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<TResponse> PostAsJsonAsyncWithAuthentication<TRequest, TResponse>(string path, TRequest value)
        {
            return await SendAsyncWithAuthentication<TResponse>(path, HttpMethod.Post, ConstructJsonHttpContent<TRequest, TResponse>(value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<TResponse> PutAsJsonAsyncWithAuthentication<TRequest, TResponse>(string path, TRequest value)
        {
            return await SendAsyncWithAuthentication<TResponse>(path, HttpMethod.Put, ConstructJsonHttpContent<TRequest, TResponse>(value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<T> DeleteAsyncWithAuthentication<T>(string path)
        {
            return await SendAsyncWithAuthentication<T>(path, HttpMethod.Delete);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task<T> SendAsyncWithAuthentication<T>(string path, HttpMethod method, HttpContent content = null)
        {
            var request = base.BuildRequest(path, method, content);

            var token = Auth0Provider.AccessToken;
            request.Headers.Add("Authorization", $"Bearer {token}"); // Specific to Cimpress Auth0 implementation

            return await SendRequestAsync<T>(request);
        }
    }
}