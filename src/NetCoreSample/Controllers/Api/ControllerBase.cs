﻿using Microsoft.AspNetCore.Mvc;
using NetCoreSample.Controllers.Api.Common.Requests;
using NetCoreSample.Controllers.Api.Common.Responses;

namespace NetCoreSample.Controllers.Api
{
    /// <summary>
    /// A base for all controllers. 
    /// 
    /// This is mainly to provide a hook-in point of utilities needed
    /// by all controllers
    /// </summary>
    public class ControllerBase : Controller
    {
        /// <summary>
        /// Constructs a 200 OK response with Json output
        /// </summary>
        /// <remarks>This hides the method provided by MVC out of box</remarks>
        protected new FullJsonResult Json(object value)
        {
            return new FullJsonResult(value);
        }

        /// <summary>
        /// Constructs a 301 Moved Permanently response
        /// </summary>
        protected MovedPermanentlyResult MovedPermanently(string url)
        {
            return new MovedPermanentlyResult(url);
        }

        /// <summary>
        /// Constructs a 304 Not Modified response
        /// </summary>
        protected NotModifiedResult NotModified()
        {
            return new NotModifiedResult();
        }

        /// <summary>
        /// Constructs a 412 Precondition Failed response
        /// </summary>
        protected PreconditionFailedResult PreconditionFailed()
        {
            return new PreconditionFailedResult();
        }

        /// <summary>
        /// Create a (configurable) check for preconditions of a request.
        /// 
        /// Such as check for cases where 304 or 412 should be returned and short-circuit
        /// the rest of the request handling.
        /// </summary>
        protected RequestPreconditionCheck RequestPreconditionCheck()
        {
            return new RequestPreconditionCheck(HttpContext.Request);
        }
    }
}
