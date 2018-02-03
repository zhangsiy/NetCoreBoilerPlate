using Microsoft.AspNetCore.Mvc;

namespace NetCoreSample.Controllers.Api.Common.Responses
{
    /// <summary>
    /// ActionResult to represent 301 Moved Permanently
    /// </summary>
    public class MovedPermanentlyResult : RedirectResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">URL to redirect to.</param>
        public MovedPermanentlyResult(string url) 
            : base(url, true)
        {
        }
    }
}
