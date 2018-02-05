namespace NetCoreSample.Service.Models.HealthCheck
{
    /// <summary>
    /// DTO holds check result of the running service's self check
    /// </summary>
    public class SelfLiveCheckResult
    {
        /// <summary>
        /// Message provided by the self check
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Version information of the running service
        /// </summary>
        public string Version { get; set; }
    }
}
