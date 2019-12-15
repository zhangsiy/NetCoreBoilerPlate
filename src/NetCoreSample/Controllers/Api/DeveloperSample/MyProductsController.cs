using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NetCoreSample.Controllers.Api.Common;
using NetCoreSample.Data.DeveloperSample;
using NetCoreSample.Models.DeveloperSample;

namespace NetCoreSample.Controllers.Api.DeveloperSample
{
    /// <summary>
    /// Sample Controller for basic CRUD operations
    /// </summary>
    [Route("api/v1/[controller]")]
    [ResponseCache(CacheProfileName = "PublicOneHour")]
    public class MyProductsController : ControllerBase
    {
        private IMyProductRepository MyProductRepository { get; }

        public MyProductsController(IMyProductRepository myProductRepository)
        {
            MyProductRepository = myProductRepository;
        }

        /// <summary>
        /// Get all entities 
        /// </summary>
        /// <returns>The list of entities</returns>
        [Route(""), HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MyProduct>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            // Example of using the search request builder
            return Json(await MyProductRepository.GetAllAsync());
        }

        /// <summary>
        /// Get entities by ID
        /// </summary>
        /// <param name="myProductId">The ID to find</param>
        /// <returns>The entity matches the given ID</returns>
        [Route("{myProductId}"), HttpGet]
        [EnableCors("ReadOnlyDefault")]
        [ProducesResponseType(typeof(MyProduct), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById(string myProductId)
        {
            // Example of using the query builder
            var result = await MyProductRepository.GetAsync(myProductId);

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
        [ProducesResponseType(typeof(MyProduct), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Post([FromBody]MyProduct value)
        {
            // Sample simple sanity check
            // In reality, validation should be in the model layer and not controller
            if (!string.IsNullOrEmpty(value.MyProductId))
            {
                return BadRequest("MyProductId should not be provided for a POST!");
            }

            var createdValue = await MyProductRepository.CreateAsync(value);

            // Construct the response
            return Created(Url.Action("GetById", new {myProductId = createdValue.MyProductId}), createdValue);
        }

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <returns>The updated entity</returns>
        [Route(""), HttpPut]
        [ProducesResponseType(typeof(MyProduct), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Put([FromBody]MyProduct value)
        {
            // Sample validation on the model
            // In reality, validation should be in the model layer and not controller
            if (string.IsNullOrEmpty(value.MyProductId))
            {
                return BadRequest("MyProductId is required!");
            }

            // First try get the resource to update
            var toUpdate = await MyProductRepository.GetAsync(value.MyProductId);
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

            // Now update the resource
            var updatedValue = await MyProductRepository.UpdateAsync(value);

            // Generate a etag for the new entity
            var eTag = new ETagBuilder()
                .WithToken(updatedValue.MyProductId)
                .WithToken(updatedValue.Name)
                .WithToken(updatedValue.Description)
                .Build();

            // Construct the response as 200 OK (or 204 No Content), with right headers attached
            return Json(value).WithETag(eTag).WithLastModified(updatedValue.Modified);
        }
    }
}
