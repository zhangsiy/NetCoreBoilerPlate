using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog;

namespace NetCoreSample.Middlewares
{
    /// <summary>
    /// Middleware to ensure correlation ID set in context for every request scope
    /// </summary>
    public class CorrelationIdAttachMiddelware
    {
        private const string CorrelationIdHeaderKey = "x-correlation-id";

        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">The handle for the next link in the pipeline</param>
        public CorrelationIdAttachMiddelware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// The default required method for middlewares, entry point for the logic 
        /// of the middleware.
        /// </summary>
        /// <param name="context">HTTP Context</param>
        /// <returns>Task facilitating the async operation</returns>
        public async Task Invoke(HttpContext context)
        {
            // Setup correslation ID in the context
            var correlationId = GetCorrelationId(context);
            MappedDiagnosticsLogicalContext.Set("correlationId", correlationId);

            // Attach the correlation ID to reponse header
            context.Response.Headers[CorrelationIdHeaderKey] = correlationId;

            await _next.Invoke(context);
        }

        /// <summary>
        /// Get or generate a correlation ID based on the context
        /// </summary>
        /// <param name="context">HTTP Context</param>
        /// <returns>The correlation ID to use</returns>
        private static string GetCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey(CorrelationIdHeaderKey)
                && !string.IsNullOrEmpty(context.Request.Headers[CorrelationIdHeaderKey]))
            {
                return context.Request.Headers[CorrelationIdHeaderKey];
            }
            else
            {
                return context.TraceIdentifier;
            }
        }
    }
}
