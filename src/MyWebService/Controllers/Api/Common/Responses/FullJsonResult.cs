using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyWebService.Controllers.Api.Common.Responses;

namespace MyWebService.Controllers.Api.Common.ResponseBuilder
{
    /// <summary>
    /// ActionResult to represent Json-coded 200 OK
    /// </summary>
    public class FullJsonResult : JsonResult
    {
        /// <summary>
        /// ETag to build into the response header
        /// </summary>
        private string ETag { get; set; }

        /// <summary>
        /// Last-Modified to build into the response header
        /// </summary>
        private DateTime? LastModified { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The value to return as the response body</param>
        public FullJsonResult(object value) 
            : base(value)
        {
        }

        /// <summary>
        /// Attach ETag to build into the response header
        /// </summary>
        public FullJsonResult WithETag(string eTag)
        {
            ETag = eTag;
            return this;
        }

        /// <summary>
        /// Attach Last-Modified to build into the response header
        /// </summary>
        public FullJsonResult WithLastModified(DateTime lastModified)
        {
            LastModified = lastModified;
            return this;
        }

        /// <summary>
        /// Attach all the response headers to the ActionContext
        /// </summary>
        private void AttachHeaders(ActionContext context)
        {
            context.PutETag(ETag);
            context.PutLastModified(LastModified);
        }

        /// <summary>
        /// Execute the result logic async
        /// </summary>
        public override Task ExecuteResultAsync(ActionContext context)
        {
            AttachHeaders(context);
            return base.ExecuteResultAsync(context);
        }

        /// <summary>
        /// Execute the result logic
        /// </summary>
        public override void ExecuteResult(ActionContext context)
        {
            AttachHeaders(context);
            base.ExecuteResult(context);
        }
    }
}
