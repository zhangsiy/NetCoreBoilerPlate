using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreSample.Controllers
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
#pragma warning disable 1998
        public async Task<object> Get()
#pragma warning restore 1998
        {
            throw new InvalidOperationException("This is an exception thrown for testing!");
        }
    }
}
