using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyWebService.Models.MyProduct;

namespace MyWebService.Controllers.Api
{
    /// <summary>
    /// Sample Controller for basic CRUD operations
    /// </summary>
    [Route("api/v1/[controller]")]
    [ResponseCache(CacheProfileName = "Default")]
    [Produces("application/json")]
    public class MyProductsController : Controller
    {
        private List<MyProduct> AllMyProducts { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MyProductsController()
        {
            AllMyProducts = new List<MyProduct> {
                new MyProduct { MyProductId = "AAA-001", Name = "My Product 1", Description = "First test MyProduct"},
                new MyProduct { MyProductId = "AAA-002", Name = "My Product 2", Description = "Second test MyProduct" },
                new MyProduct { MyProductId = "AAA-003", Name = "My Product 3", Description = "Third test MyProduct" }
            };
        }

        /// <summary>
        /// Get all entities 
        /// </summary>
        /// <returns>The list of entities</returns>
        [Route(""), HttpGet]
        public async Task<IEnumerable<MyProduct>> GetAll()
        {
            // Example of using the search request builder
            return AllMyProducts;
        }

        /// <summary>
        /// Get entities by ID
        /// </summary>
        /// <param name="myProductId">The ID to find</param>
        /// <returns>The entity matches the given ID</returns>
        [Route("{myProductId}"), HttpGet]
        public async Task<MyProduct> GetById(string myProductId)
        {
            // Example of using the query builder
            return AllMyProducts.FirstOrDefault(p => p.MyProductId == myProductId);
        }
    }
}
