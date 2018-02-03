using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreSample.Controllers.Api.Common.Responses;

namespace NetCoreSample.Controllers.Api.Common.Requests
{
    /// <summary>
    /// Builder to build check for pre-conditions of a request.
    /// It also provides access to recommendation of the action
    /// to take based on the check result.
    /// </summary>
    public class RequestPreconditionCheck
    {
        /// <summary>
        /// The Web Request
        /// </summary>
        private HttpRequest Request { get; set; }

        /// <summary>
        /// E-Tag to check against
        /// </summary>
        private string ETag { get; set; }

        /// <summary>
        /// Last Modified time stamp to check against
        /// </summary>
        private DateTime? LastModified { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RequestPreconditionCheck(HttpRequest request)
        {
            Request = request;
        }

        /// <summary>
        /// Specify the e-tag to check against
        /// </summary>
        public RequestPreconditionCheck WithETag(string eTag)
        {
            ETag = eTag;
            return this;
        }

        /// <summary>
        /// Specify the last modified time stamp to check against
        /// </summary>
        public RequestPreconditionCheck WithLastModified(DateTime lastModified)
        {
            LastModified = lastModified;
            return this;
        }

        /// <summary>
        /// Evaluate the check(s) and produce a recommended action
        /// </summary>
        /// <returns>
        /// The recommended action to take based on the check results. 
        /// null if no deviating action to take. 
        /// </returns>
        public IActionResult GetRecommendedAction()
        {
            switch (Request.Method)
            {
                // 304 Not Modified conditions
                case "GET":
                case "HEAD":
                    if (LastModified != null)
                    {
                        if (!Request.CheckIfModifiedSince(LastModified.Value))
                        {
                            return new NotModifiedResult();
                        }
                    }

                    if (!string.IsNullOrEmpty(ETag))
                    {
                        if (!Request.CheckIfNoneMatch(ETag))
                        {
                            return new NotModifiedResult();
                        }
                    }
                    break;

                // 412 Precondition Failed conditions
                case "PUT":
                case "POST":
                    if (LastModified != null)
                    {
                        if (!Request.CheckIfMatch(ETag))
                        {
                            return new PreconditionFailedResult();
                        }
                    }

                    if (!string.IsNullOrEmpty(ETag))
                    {
                        if (!Request.CheckIfUnmodifiedSince(LastModified.Value))
                        {
                            return new PreconditionFailedResult();
                        }
                    }
                    break;

                default:
                    // For now, don't expect the usage of PATCH, and just bypass the checks
                    break;
            }

            return null;
        }
    }
}
