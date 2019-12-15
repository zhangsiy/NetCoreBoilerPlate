using System;
using Microsoft.AspNetCore.Http;

namespace NetCoreSample.Extensions.Http
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Get the authorization bearer token if it exists on the HttpRequest
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns>The bearer token if exists on the request. Otherwise, null. </returns>
        public static string GetAuthorizationBearerToken(this HttpRequest httpRequest)
        {
            string authHeader = httpRequest.Headers["Authorization"].ToString().Trim();
            if (authHeader.StartsWith("bearer ", StringComparison.InvariantCultureIgnoreCase))
            {
                string authToken = authHeader.Substring(7);
                return authToken;
            }

            return null; 
        }
    }
}
