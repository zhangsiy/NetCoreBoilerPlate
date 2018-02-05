using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreSample.Service.Controllers
{
    /// <summary>
    /// A end point to allow external pings to detect the up status of the service
    /// </summary>
    [Route("/[controller]")]
    public class ExceptionController : Controller
    {
        /// <summary>
        /// Ping to get a live check response
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get()
        {
            throw new InvalidOperationException("This is an exception thrown for testing!");
        }
    }
}
