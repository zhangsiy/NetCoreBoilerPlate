using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreSample.Service.Controllers.Api.Common.Responses
{
    /// <summary>
    /// ActionResult to represent 409 Conflict
    /// </summary>
    public class ConflictResult : ObjectResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conflictDescription">Description object for the conflict</param>
        public ConflictResult(object conflictDescription) 
            : base(conflictDescription)
        {
            StatusCode = StatusCodes.Status409Conflict;
        }
    }
}
