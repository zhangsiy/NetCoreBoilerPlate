using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NetCoreSample.Extensions.Http;
using Serilog;
using Serilog.Events;

namespace NetCoreSample.Middlewares
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
            "Action={ActionPath} Request=[HTTP {RequestMethod} {RequestPath} {QueryString} responded {StatusCode} in {Elapsed:0.0000} ms]";

        static readonly ILogger Logger = Serilog.Log.ForContext<RequestLoggingMiddleware>();

        readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                await _next.Invoke(httpContext);
                sw.Stop();

                // For now, assume it is expected to provide verbose logging for any HTTP 5XX 
                // responses. 
                // Implementers should decide whether this behavior should be changed based on
                // specific use cases. 
                // E.g. whether 4XX should log verbose context as well. 
                int? statusCode = httpContext.Response?.StatusCode;
                LogEventLevel level = statusCode >= 500 ? LogEventLevel.Error : LogEventLevel.Information;

                ILogger logger = level >= LogEventLevel.Error ? await GetLoggerWithVerboseRequestContext(httpContext) : Logger;
                logger.Write(level, MessageTemplate,
                    httpContext.GetActionPath(),
                    httpContext.Request.Method, httpContext.Request.Path, httpContext.Request.QueryString.ToString(), statusCode, sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                await LogException(httpContext, sw, ex);
                throw;
            }
        }

        /// <summary>
        /// Log an exception originated from handling the given web request context.
        /// </summary>
        private async static Task<bool> LogException(HttpContext httpContext, Stopwatch sw, Exception ex)
        {
            sw.Stop();

            ILogger logger = await GetLoggerWithVerboseRequestContext(httpContext);
            logger.Error(ex, MessageTemplate,
                httpContext.GetActionPath(),
                httpContext.Request.Method, httpContext.Request.Path, httpContext.Request.QueryString.ToString(), 500, sw.Elapsed.TotalMilliseconds);

            return false;
        }

        /// <summary>
        /// Get a (Serilog) logger instance with verbose web request context data attached
        /// </summary>
        private async static Task<ILogger> GetLoggerWithVerboseRequestContext(HttpContext httpContext)
        {
            dynamic requestContext = await GetRequestContext(httpContext);
            return Log.ForContext("RequestContext", requestContext, true);
        }

        /// <summary>
        /// Construct a structure of data that represent verbose information
        /// from the request context.
        /// </summary>
        private async static Task<dynamic> GetRequestContext(HttpContext httpContext)
        {
            HttpRequest request = httpContext.Request;

            string requestBodyString = null;

            if (request.Body != null && request.Body.CanSeek)
            {
                long currentPosition = request.Body.Position;

                request.Body.Seek(0, SeekOrigin.Begin);

                // Hacky
                // StreamReader takes over ownership of stream passed in.
                // So if we wrap it in a "using" statement, it'll dispose
                // the body stream here, which is not something we want 
                // this piece of code to do.
                // So leave the StreamReader below to garbage collector,
                // and not invoke Dispose on it.
                var bodyReader = new StreamReader(request.Body);

                requestBodyString = await bodyReader.ReadToEndAsync();
                request.Body.Seek(currentPosition, SeekOrigin.Begin);
            }

            var result = new
            {
                RequestHeader = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                RequestHost = request.Host,
                RequestRemoteIp = httpContext.Connection.RemoteIpAddress,
                RequestProtocol = request.Protocol,
                RequestContentType = request.ContentType,
                RequestContentLength = request.ContentLength,
                RequestQueryString = request.QueryString,
                RequestBodyString = requestBodyString,
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
            HttpResponse response = httpContext.Response;

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
