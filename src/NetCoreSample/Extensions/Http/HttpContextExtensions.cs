using Microsoft.AspNetCore.Http;

namespace NetCoreSample.Extensions.Http
{
    internal static class HttpContextExtensions
    {
        private const string ActionPathKey = "ActionPath";

        public static void SetActionPath(this HttpContext httpContext, string actionPath)
        {
            httpContext.SetItem(ActionPathKey, actionPath);
        }
        
        public static string GetActionPath(this HttpContext httpContext)
        {
            return httpContext.GetItem(ActionPathKey);
        }
        
        private static void SetItem(this HttpContext httpContext, string key, string value)
        {
            httpContext.Items[key] = value;
        }

        private static string GetItem(this HttpContext httpContext, string key)
        {
            if (httpContext.Items.ContainsKey(key))
            {
                return httpContext.Items[key].ToString();
            }
            return null;
        }
    }
}
