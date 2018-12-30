using System.Threading.Tasks;
using NetCoreSample.Service.Common;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NetCoreSample.Service.ActionFilters
{
    /// <summary>
    /// This action filter is intended to solicit information from action contexts
    /// and prepare them for logging
    /// </summary>
    public class ActionLoggingContextFilter : IAsyncActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Here extract the action path and attach to the http context for logging
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            string controllerName = controllerActionDescriptor?.ControllerName;
            string actionName = controllerActionDescriptor?.ActionName;
            string actionPath = $"{controllerName}/{actionName}";

            context.HttpContext.SetActionPath(actionPath);

            // And just let the pipeline proceed
            await next.Invoke();
        }
    }
}
