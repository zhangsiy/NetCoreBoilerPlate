using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreSample.Service.Controllers
{
    /// <summary>
    /// An endpoint to trigger a server side exception, for testing
    /// </summary>
    [Route("/[controller]")]
    public class ExceptionController : Controller
    {
        /// <summary>
        /// Trigger a server side exception. (For Testing)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get()
        {
            throw new InvalidOperationException("This is an exception thrown for testing!");
        }
    }
}
