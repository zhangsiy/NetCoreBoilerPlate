using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreSample.Service.Middlewares
{
    /// <summary>
    /// A middleware aimed at global level logging of service requests, and responses where 
    /// appropriate.
    /// This also handles the global level logging of exceptions occurred anywhere within the
    /// pipeline.
    /// Note: This is a revised version from the code copied from the reference below.
    /// 
    /// Reference: https://blog.getseq.net/smart-logging-middleware-for-asp-net-core/
    /// </summary>
    public class RequestLoggingMiddleware
    {
        /// <summary>
        /// Default request logging message template
        /// </summary>
        const string MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        static readonly ILogger Log = Serilog.Log.ForContext<RequestLoggingMiddleware>();

        readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var sw = Stopwatch.StartNew();
            try
            {
                await _next.Invoke(httpContext);
                sw.Stop();

                // For now, assume it is expected to provide verbose logging for any HTTP 5XX 
                // responses. 
                // Implementers should decide whether this behavior should be changed based on
                // specific use cases. 
                // E.g. whether 4XX should log verbose context as well. 
                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode >= 500 ? LogEventLevel.Error : LogEventLevel.Information;

                var log = level >= LogEventLevel.Error ? GetLoggerWithVerboseRequestContext(httpContext) : Log;
                log.Write(level, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, statusCode, sw.Elapsed.TotalMilliseconds);
            }
            // Never caught, because `LogException()` returns false.
            catch (Exception ex) when (LogException(httpContext, sw, ex)) { }
        }

        /// <summary>
        /// Log an exception originated from handling the given web request context.
        /// </summary>
        private static bool LogException(HttpContext httpContext, Stopwatch sw, Exception ex)
        {
            sw.Stop();

            GetLoggerWithVerboseRequestContext(httpContext)
                .Error(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500, sw.Elapsed.TotalMilliseconds);

            return false;
        }

        /// <summary>
        /// Get a (Serilog) logger instance with verbose web request context data attached
        /// </summary>
        private static ILogger GetLoggerWithVerboseRequestContext(HttpContext httpContext)
        {
            var requestContext = GetRequestContext(httpContext);
            return Log.ForContext("RequestContext", requestContext, true);
        }

        /// <summary>
        /// Construct a structure of data that represent verbose information
        /// from the request context.
        /// </summary>
        private static dynamic GetRequestContext(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var result = new
            {
                RequestHeader = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                RequestHost = request.Host,
                RequestRemoteIp = httpContext.Connection.RemoteIpAddress,
                RequestProtocol = request.Protocol,
                RequestContentType = request.ContentType,
                RequestContentLength = request.ContentLength,
                RequestQueryString = request.QueryString,
                RequestForm = request.HasFormContentType 
                            ? request.Form.ToDictionary(v => v.Key, v => v.Value.ToString())
                            : null
            };

            return result;
        }

        /// <summary>
        /// Construct a structure of data that represent verbose information
        /// from the request context.
        /// </summary>
        private static dynamic GetResponseContext(HttpContext httpContext)
        {
            var response = httpContext.Response;

            var result = new
            {
                ResponseHeader = response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                ResponseStatusCode = response.StatusCode,
                ResponseContentType = response.ContentType,
                ResponseContentLength = response.ContentLength
            };

            return result;
        }
    }
}
