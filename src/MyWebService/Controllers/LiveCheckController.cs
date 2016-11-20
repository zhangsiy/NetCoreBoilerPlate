using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyWebService.Models.HealthCheck;

namespace MyWebService.Controllers
{
    /// <summary>
    /// A end point to allow external pings to detect the up status of the service
    /// </summary>
    [Route("/[controller]")]
    public class LiveCheckController : Controller
    {
        /// <summary>
        /// Ping to get a live check response
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get()
        {
            return await new LiveCheckBuilder()
                .RegisterSelfCheck()
                .Run();
        }

    }
}
