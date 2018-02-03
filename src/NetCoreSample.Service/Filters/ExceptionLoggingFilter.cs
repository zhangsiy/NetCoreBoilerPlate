using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace NetCoreSample.Filters
{
    /// <summary>
    /// Filter for exception logging
    /// </summary>
    public class ExceptionLoggingFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            // Log the exception
            Log.Error(context.Exception.ToString());

            // Hand over the control to base
            base.OnException(context);
        }
    }
}
