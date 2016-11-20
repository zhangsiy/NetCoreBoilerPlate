namespace MyWebService.Data.ElasticSearch
{
    /// <summary>
    /// Data envelope to hold consiguration options to drive the ElasticRepository's behavior
    /// </summary>
    public class EsRepoConfiguration
    {
        /// <summary>
        /// The full qualified URL pointing at the Elastic Search cluster 
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// Configuration for AWS access
        /// </summary>
        public AwsConfiguration AwsConfiguration { get; set; }
    }
}
