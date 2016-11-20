using System.Collections.Generic;
using System.Threading.Tasks;
using MyWebService.Data.ElasticSearch;
using Microsoft.AspNetCore.Mvc;

namespace MyWebService.Controllers.Api
{
    /// <summary>
    /// A base class implementation for API controllers that require AWS ElasticSearch support
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity</typeparam>
    public class ApiWithEsControllerBase<TEntity> : Controller where TEntity : class
    {
        /// <summary>
        /// The (injected) Elastic Search repository instance
        /// </summary>
        protected IEsRepository EsRepository { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="esRepository">The (injected) Elastic Search repository instance</param>
        public ApiWithEsControllerBase(IEsRepository esRepository)
        {
            EsRepository = esRepository;
        }

        /// <summary>
        /// A default action to attach onto all extending controllers.
        /// To perform query/search with raw queries.
        /// </summary>
        /// <param name="rawQuery">The raw Elastic Search query to use</param>
        /// <returns>The collection of entities found via the query.</returns>
        [Route("~/api/v1/[controller]/raw/{rawQuery}"), HttpGet]
        public async Task<IEnumerable<TEntity>> GetByRawQuery(string rawQuery)
        {
            // Example of using the query builder
            return await EsRepository.SearchEntitiesAsyncWithQuery<TEntity>(q => q.Raw(rawQuery));
        }
    }
}
