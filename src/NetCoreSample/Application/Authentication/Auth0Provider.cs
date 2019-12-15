using Microsoft.AspNetCore.Http;
using NetCoreSample.Extensions.Http;

namespace NetCoreSample.Application.Authentication
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
