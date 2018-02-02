using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace NetCoreSample.Service.Controllers
{
    /// <summary>
    /// End point to allow external pings to detect the up status of the service
    /// A common use case being 
    /// </summary>
    [Route("/[controller]")]
    public class LiveCheckController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public async Task<object> Get()
        {
            return Json(new
            {
                Message = "The service is live!",
                Version = Assembly.GetEntryAssembly().GetName().Version.ToString(4)
            });
        }
    }
}
