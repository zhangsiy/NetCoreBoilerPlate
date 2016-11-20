namespace MyWebService.Data.ElasticSearch
{
    /// <summary>
    /// Data structure to hold AWS configurations
    /// </summary>
    public class AwsConfiguration
    {
        /// <summary>
        /// Access key for the IAM account
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Secret key for the IAM account
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// AWS region
        /// </summary>
        public string Region { get; set; }
    }
}
