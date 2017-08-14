using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyWebService.Controllers.Api.Common.Responses
{
    /// <summary>
    /// ActionResult to represent 304 Not Modified
    /// </summary>
    public class NotModifiedResult : StatusCodeResult 
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
        public NotModifiedResult() 
            : base(StatusCodes.Status304NotModified)
        {
        }

        /// <summary>
        /// Attach ETag to build into the response header
        /// </summary>
        public NotModifiedResult WithETag(string eTag)
        {
            ETag = eTag;
            return this;
        }

        /// <summary>
        /// Attach Last-Modified to build into the response header
        /// </summary>
        public NotModifiedResult WithLastModified(DateTime lastModified)
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
