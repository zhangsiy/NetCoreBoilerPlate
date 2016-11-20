using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWebService.Logging;
using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using Microsoft.Extensions.Options;
using MyWebService.Models.MyProduct;
using Nest;
using NLog;

namespace MyWebService.Data.ElasticSearch
{
    /// <summary>
    /// A simple repository providing access to ElasticSearch indices 
    /// </summary>
    internal class EsRepository : IEsRepository
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly IEsApiCallDetailsLogger EsApiLog = new EsApiCallDetailsLogger();

        private IElasticClient Client { get; set; }

        public EsRepository(IOptions<EsRepoConfiguration> esConfiguration)
        {
            var esConfig = esConfiguration.Value;

            var httpConnection = new AwsHttpConnection(new AwsSettings
            {
                AccessKey = esConfig.AwsConfiguration.AccessKey,
                SecretKey = esConfig.AwsConfiguration.SecretKey,
                Region = esConfig.AwsConfiguration.Region
            });

            var pool = new SingleNodeConnectionPool(new Uri(esConfig.ServerUrl));

            // Configure the connection
            //  - Also hookup asyn logging of all API call details (EnableTrace not avaialble in NEST 2.x, yet.)
            var config = new ConnectionSettings(pool, httpConnection)
                .DisableDirectStreaming()
                .OnRequestCompleted(r => EsApiLog.LogEsApiCallDetailsAsync(r));

            // Default index mappings to types...
            // To bad there is no attribute based mapping for this
            // [TO_FILL register ElasticSearch index mapping here]
            config.MapDefaultTypeIndices(m => m
                .Add(typeof(MyProduct), "my_product_index")
            );

            // Create the client
            Client = new ElasticClient(config);
        }

        /// <summary>
        /// Search entities with the search request built by the specified building function
        /// </summary>
        /// <typeparam name="T">The type of the entities to search</typeparam>
        /// <param name="buildRequest">
        ///     The function provides a descriptive way to build search request.
        ///     <see href="https://www.elastic.co/guide/en/elasticsearch/client/net-api/1.x/writing-queries.html"/>
        /// </param>
        /// <returns>The enumeration captures the entities found by the search</returns>
        public async Task<IEnumerable<T>> SearchEntitiesAsync<T>(Func<SearchDescriptor<T>, ISearchRequest> buildRequest) where T : class
        {
            var response = await Client.SearchAsync<T>(buildRequest);
            return response.Documents;
        }

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
        public async Task<IEnumerable<T>> SearchEntitiesAsyncWithQuery<T>(Func<QueryContainerDescriptor<T>, QueryContainer> buildQuery) where T : class
        {
            var response = await Client.SearchAsync<T>(s => s
                .Size(EsConstants.DefaultSearchResultNumPerBatch)
                .Query(buildQuery));
            return response.Documents;
        }
    }
}
