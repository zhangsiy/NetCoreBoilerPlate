using Microsoft.AspNetCore.Mvc;

namespace NetCoreSample.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Redirect default route to swagger
        /// </summary>
        [HttpGet]
        [Route("")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public RedirectResult Get()
        {
            return new RedirectResult("/swagger/index.html");
        }
    }
}