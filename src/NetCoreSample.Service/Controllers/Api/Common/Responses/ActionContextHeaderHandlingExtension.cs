using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace NetCoreSample.Service.Controllers.Api.Common.Responses
{
    /// <summary>
    /// Extension utility class to attach additional response headers to the ActionContext,
    /// so they show up as part of the response
    /// </summary>
    public static class ActionContextHeaderHandlingExtension
    {
        /// <summary>
        /// Attach ETag header to the response
        /// </summary>
        public static void PutETag(this ActionContext context, string eTag)
        {
            if (context != null && context.HttpContext != null && context.HttpContext.Response != null)
            {
                if (eTag != null)
                {
                    context.HttpContext.Response.Headers.Add(HeaderNames.ETag, eTag);
                }
            }
        }

        /// <summary>
        /// Attach LastModified header to the response
        /// </summary>
        public static void PutLastModified(this ActionContext context, DateTime? lastModified)
        {
            if (context != null && context.HttpContext != null && context.HttpContext.Response != null)
            {
                if (lastModified != null)
                {
                    context.HttpContext.Response.Headers.Add(HeaderNames.LastModified, lastModified.Value.ToUniversalTime().ToString("R"));
                }
            }
        }
    }
}
