namespace NetCoreSample.Service.Models.HealthCheck
{
    /// <summary>
    /// Model to hold Database live check result
    /// </summary>
    public class DatabaseLiveCheckResult
    {
        /// <summary>
        /// The database connection string checked
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Was the connection successful
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// Additional details from context
        /// </summary>
        public dynamic Details { get; set; }
    }
}
