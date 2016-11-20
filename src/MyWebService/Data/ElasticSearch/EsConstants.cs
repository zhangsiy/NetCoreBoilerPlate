using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebService.Data.ElasticSearch
{
    /// <summary>
    /// Constants for Elastic Search
    /// </summary>
    public static class EsConstants
    {
        /// <summary>
        /// Limit on the number of results per batch for a search
        /// </summary>
        public const int DefaultSearchResultNumPerBatch = 1000;
    }
}
