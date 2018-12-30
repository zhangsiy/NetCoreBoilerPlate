using NetCoreSample.Service.Common;
using Microsoft.AspNetCore.Http;

namespace NetCoreSample.Service.Application.Authentication
{
    internal class Auth0Provider : IAuth0Provider
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }

        public Auth0Provider(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public string AccessToken
        {
            get
            {
                HttpContext context = HttpContextAccessor.HttpContext;

                return context?.Request.GetAuthorizationBearerToken();
            }
        }
    }
}
