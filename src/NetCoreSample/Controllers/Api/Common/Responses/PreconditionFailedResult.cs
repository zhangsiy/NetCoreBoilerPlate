using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreSample.Controllers.Api.Common.Responses
{
    /// <summary>
    /// ActionResult to represent 412 Precondition Failed
    /// </summary>
    public class PreconditionFailedResult : StatusCodeResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PreconditionFailedResult()
            : base(StatusCodes.Status412PreconditionFailed)
        {
        }
    }
}
