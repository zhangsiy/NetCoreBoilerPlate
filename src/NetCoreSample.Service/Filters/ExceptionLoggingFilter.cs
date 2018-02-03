using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace NetCoreSample.Filters
{
    /// <summary>
    /// Filter for exception logging
    /// </summary>
    public class ExceptionLoggingFilter : ExceptionFilterAttribute
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            // Log the exception
            _logger.Error(context.Exception.ToString());

            // Hand over the control to base
            base.OnException(context);
        }
    }
}
