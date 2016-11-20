using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace MyWebService.Data.ElasticSearch
{
    /// <summary>
    /// A simple repository providing access to ElasticSearch indices 
    /// </summary>
    public interface IEsRepository
    {
        /// <summary>
        /// Search entities with the search request built by the specified building function
        /// </summary>
        /// <typeparam name="T">The type of the entities to search</typeparam>
        /// <param name="buildRequest">
        ///     The function provides a descriptive way to build search request.
        ///     <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/1.x/writing-queries.html"/>
        /// </param>
        /// <returns>The enumeration captures the entities found by the search</returns>
        Task<IEnumerable<T>> SearchEntitiesAsync<T>(Func<SearchDescriptor<T>, ISearchRequest> buildRequest) where T : class;

        /// <summary>
        /// Search entities with the query (container) built by the specified building function.
        /// This will use system default values for specifications in the search request but outside of the query. 
        /// E.g. limit on number of results.
        ///     <seealso cref="EsConstants"/> 
        /// </summary>
        /// <typeparam name="T">The type of the entities to search</typeparam>
        /// <param name="buildQuery">
        ///     The function provides a descriptive way to build query.
        ///     <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/1.x/writing-queries.html"/>
        /// </param>
        /// <returns>The enumeration captures the entities found by the search</returns>
        Task<IEnumerable<T>> SearchEntitiesAsyncWithQuery<T>(Func<QueryContainerDescriptor<T>, QueryContainer> buildQuery) where T : class;
    }
}
