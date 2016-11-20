using Microsoft.AspNetCore.Builder;

namespace MyWebService.Middlewares
{
    /// <summary>
    /// Extension to register request logger to the application
    /// </summary>
    public static class MiddlewareRegistrationExtensions
    {
        /// <summary>
        /// Builder hook to register the middleware
        /// </summary>
        /// <param name="builder">The application builder</param>
        /// <returns>The resultant application builder object for fluent calls</returns>
        public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }

        /// <summary>
        /// Builder hook to register the middleware
        /// </summary>
        /// <param name="builder">The application builder</param>
        /// <returns>The resultant application builder object for fluent calls</returns>
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationIdAttachMiddelware>();
        }
    }
}
