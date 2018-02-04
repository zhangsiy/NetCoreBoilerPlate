using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace NetCoreSample.Service.Controllers.Api.Common.Requests
{
    /// <summary>
    /// Extensions to HttpRequest to help performing all types of server side
    /// cache helping headers 
    /// </summary>
    internal static class RequestHeaderCheckExtensions
    {
        /// <summary>
        /// Check for header: If-Match
        /// 
        /// Value of a previous calls ETag response used with a PUT or DELETE. 
        /// Server should only act if nobody else has modified the resource since you last fetched it.
        /// Otherwise provides a 412.
        /// </summary>
        public static bool CheckIfMatch(this HttpRequest request, string eTag)
        {
            return CheckHeader(request, HeaderNames.IfMatch, headerValue => eTag == headerValue);
        }

        /// <summary>
        /// Check for header: If-Modified-Since
        /// 
        /// Value of a previous Last-Modified response used with a GET. 
        /// Server should only provide a response if the resource was modified since the timestamp submitted. 
        /// Use in conjunction with If-None-Match in case the change occurred within the same second.
        /// Otherwise provide a 304.
        /// </summary>
        public static bool CheckIfModifiedSince(this HttpRequest request, DateTime lastModified)
        {
            return CheckHeader(request, HeaderNames.IfModifiedSince, headerValue =>
            {
                // Don't just throw if the date in the header cannot be parsed.
                // Just treat it as if the resource is updated, and let client to 
                // get a new copy, and hence with a "healthy" set of headers 
                DateTime headerDate;
                if (!DateTime.TryParse(headerValue, out headerDate))
                {
                    return true;
                }
                return lastModified.ToUniversalTime() > headerDate.ToUniversalTime();
            });
        }

        /// <summary>
        /// Check for header: If-None-Match
        /// 
        /// Value of a previous calls ETag response used with a GET.
        /// Server should only provide a response if the ETag doesn’t match, i.e. the resource has been altered.
        /// Otherwise provide a 304.
        /// </summary>
        public static bool CheckIfNoneMatch(this HttpRequest request, string eTag)
        {
            return CheckHeader(request, HeaderNames.IfNoneMatch, headerValue => eTag != headerValue);
        }

        /// <summary>
        /// Check for header: If-Unmodified-Since
        /// 
        /// value of a previous Last-Modified response used with a PUT or DELETE.
        /// Server should only act if nobody else has modified the resource since you last fetched it.
        /// Otherwise provides a 412.
        /// </summary>
        public static bool CheckIfUnmodifiedSince(this HttpRequest request, DateTime lastModified)
        {
            return CheckHeader(request, HeaderNames.IfUnmodifiedSince, headerValue =>
            {
                // Don't just throw if the date in the header cannot be parsed.
                // Just treat it as if the resource is updated, and let client to 
                // get a new copy, and hence with a "healthy" set of headers 
                DateTime headerDate;
                if (!DateTime.TryParse(headerValue, out headerDate))
                {
                    return true;
                }
                return headerDate.ToUniversalTime() >= lastModified.ToUniversalTime();
            });
        }

        /// <summary>
        /// Helper to do header sanity checks
        /// </summary>
        private static bool CheckHeader(HttpRequest request, string headerName, Func<string, bool> processHeaderValue)
        {
            if (request.Headers.ContainsKey(headerName))
            {
                var headerValue = request.Headers[headerName];
                if (!string.IsNullOrEmpty(headerValue))
                {
                    return processHeaderValue(headerValue);
                }
            }

            // Default to say true, because request did not ask for check
            return true;
        } 
    }
}
