using System.Threading.Tasks;

namespace NetCoreSample.Models.HealthCheck
{ 
    /// <summary>
    /// A builder to build and run live/connection check on various 
    /// dependencies including the service itself
    /// </summary>
    public interface ILiveCheckBuilder
    {
        /// <summary>
        /// Register a check for the running service itself
        /// </summary>
        /// <returns>The builder itself for fluent code structure</returns>
        ILiveCheckBuilder RegisterSelfCheck();

        /// <summary>
        /// Register a check for SqlServer database connection
        /// </summary>
        /// <param name="connectionString">The connection string to check</param>
        /// <returns>The builder itself for fluent code structure</returns>
        ILiveCheckBuilder RegisterSqlServerDatabaseCheck(string connectionString);

        /// <summary>
        /// Register a check for (external) web service
        /// </summary>
        /// <param name="serviceUrl">The URL identifying the web service</param>
        /// <returns>The builder itself for fluent code structure</returns>
        ILiveCheckBuilder RegisterWebServiceCheck(string serviceUrl);

        /// <summary>
        /// Execute the built live checks. 
        /// This executes all checks in non-blocking fashion
        /// </summary>
        /// <returns>The task/promise to provide the aggregated check result</returns>
        Task<dynamic> Run();
    }
}
