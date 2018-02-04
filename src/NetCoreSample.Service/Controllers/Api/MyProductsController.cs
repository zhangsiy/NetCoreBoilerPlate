using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCoreSample.Service.Controllers.Api.Common;
using NetCoreSample.Service.Models.MyProduct;

namespace NetCoreSample.Service.Controllers.Api
{
    /// <summary>
    /// Sample Controller for basic CRUD operations
    /// </summary>
    [Route("api/v1/[controller]")]
    [ResponseCache(CacheProfileName = "Default")]
    public class MyProductsController : ControllerBase
    {
        private List<MyProduct> AllMyProducts { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MyProductsController()
        {
            AllMyProducts = new List<MyProduct> {
                new MyProduct
                {
                    MyProductId = "AAA-001",
                    Name = "My Product 1",
                    Description = "First test MyProduct",
                    Created = new DateTime(2015, 10, 22, 10, 10, 10),
                    Modified = new DateTime(2015, 10, 22, 10, 10, 10)
                },
                new MyProduct
                {
                    MyProductId = "AAA-002",
                    Name = "My Product 2",
                    Description = "Second test MyProduct",
                    Created = new DateTime(2016, 2, 3, 4, 30, 30),
                    Modified = new DateTime(2016, 3, 12, 7, 8, 9)
                },
                new MyProduct
                {
                    MyProductId = "AAA-003",
                    Name = "My Product 3",
                    Description = "Third test MyProduct",
                    Created = new DateTime(2015, 03, 15, 15, 22, 11),
                    Modified = new DateTime(2016, 05, 27, 19, 6, 3)
                }
            };
        }

        /// <summary>
        /// Get all entities 
        /// </summary>
        /// <returns>The list of entities</returns>
        [Route(""), HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Example of using the search request builder
            return Json(AllMyProducts);
        }

        /// <summary>
        /// Get entities by ID
        /// </summary>
        /// <param name="myProductId">The ID to find</param>
        /// <returns>The entity matches the given ID</returns>
        [Route("{myProductId}"), HttpGet]
        public async Task<IActionResult> GetById(string myProductId)
        {
            // Example of using the query builder
            var result = AllMyProducts.FirstOrDefault(p => p.MyProductId == myProductId);

            if (result == null)
            {
                return NotFound();
            }

            // Generate a etag for the entity to allow client side caching based on this.
            var eTag = new ETagBuilder()
                .WithToken(result.MyProductId)
                .WithToken(result.Name)
                .WithToken(result.Description)
                .Build();

            // Check for conditions from request headers
            var action = RequestPreconditionCheck()
                .WithETag(eTag)
                .WithLastModified(result.Modified)
                .GetRecommendedAction();
            if (action != null)
            {
                return action;
            }

            return Json(result).WithETag(eTag).WithLastModified(result.Modified);
        }

        /// <summary>
        /// Create an entity
        /// </summary>
        /// <returns>The created entity</returns>
        [Route(""), HttpPost]
        public async Task<IActionResult> Post([FromBody]MyProduct value)
        {
            // Sample simple sanity check
            // In reality, validation should be in the model layer and not controller
            if (!string.IsNullOrEmpty(value.MyProductId))
            {
                return BadRequest("MyProductId should not be provided for a POST!");
            }

            // "Create" the new Entity
            // Generate a GUID to serve as the object ID
            var newId = Guid.NewGuid().ToString();
            value.MyProductId = newId;
            AllMyProducts.Add(value);

            // Query the store for the "real" state of the entity
            var createdValue = AllMyProducts.FirstOrDefault(p => p.MyProductId == newId);

            // Construct the response
            return Created(Url.Action("GetById", new {myProductId = newId}), createdValue);
        }

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <returns>The updated entity</returns>
        [Route(""), HttpPut]
        public async Task<IActionResult> Put([FromBody]MyProduct value)
        {
            // Sample validation on the model
            // In reality, validation should be in the model layer and not controller
            if (string.IsNullOrEmpty(value.MyProductId))
            {
                return BadRequest("MyProductId is required!");
            }

            // First try get the resource to update
            var toUpdate = AllMyProducts.FirstOrDefault(p => p.MyProductId == value.MyProductId);
            if (toUpdate == null)
            {
                return NotFound();
            }

            // Generate a etag for the entity to update
            var originalETag = new ETagBuilder()
                .WithToken(toUpdate.MyProductId)
                .WithToken(toUpdate.Name)
                .WithToken(toUpdate.Description)
                .Build();

            // Check for conditions from request headers
            var action = RequestPreconditionCheck()
                .WithETag(originalETag)
                .WithLastModified(toUpdate.Modified)
                .GetRecommendedAction();
            if (action != null)
            {
                return action;
            }

            // Now update the resource (fake it here by removing and adding) 
            AllMyProducts.Remove(toUpdate);
            AllMyProducts.Add(value);

            // Generate a etag for the new entity
            var eTag = new ETagBuilder()
                .WithToken(value.MyProductId)
                .WithToken(value.Name)
                .WithToken(value.Description)
                .Build();

            // Construct the response as 200 OK (or 204 No Content), with right headers attached
            return Json(value).WithETag(eTag).WithLastModified(value.Modified);
        }
    }
}
