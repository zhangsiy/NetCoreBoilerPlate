using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebService.Controllers.Api.Common.Requests;
using MyWebService.Controllers.Api.Common.ResponseBuilder;
using MyWebService.Controllers.Api.Common.Responses;

namespace MyWebService.Controllers.Api
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
        /// <remarks>This over-shadows the method provided by MVC out of box</remarks>
        protected FullJsonResult Json(object value)
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
        /// Constructs a 409 Conflict response
        /// </summary>
        protected ConflictResult Conflict(object conflictDescription)
        {
            return new ConflictResult(conflictDescription);
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
