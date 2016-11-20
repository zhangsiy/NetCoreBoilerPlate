using System.Threading.Tasks;

namespace MyWebService.Models.HealthCheck
{
    /// <summary>
    /// A common interface for all types of live check items
    /// </summary>
    public interface ILiveCheckItem
    {
        /// <summary>
        /// Execute the check item and return the result
        /// </summary>
        /// <returns>The task/promise to provide the check result</returns>
        Task<dynamic> ExecuteAsync();
    }
}
