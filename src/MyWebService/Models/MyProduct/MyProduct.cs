using System;
using Nest;

namespace MyWebService.Models.MyProduct
{
    /// <summary>
    /// Example model definition that supports annotations to guide 
    /// Elastic Search mapping. 
    /// </summary>
    [ElasticsearchType(Name = "MyProducts", IdProperty = "MyProductId")]
    public class MyProduct
    {
        [String(Index = FieldIndexOption.NotAnalyzed)]
        public string MyProductId { get; set; }

        [String]
        public string Name { get; set; }

        [String]
        public string Description { get; set; }

        [Date]
        public DateTime Created { get; set; }

        [Date]
        public DateTime Modified { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MyProduct()
        {
        }
    }
}
