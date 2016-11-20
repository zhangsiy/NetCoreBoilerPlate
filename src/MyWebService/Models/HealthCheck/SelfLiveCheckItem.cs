using System.Reflection;
using System.Threading.Tasks;

namespace MyWebService.Models.HealthCheck
{
    /// <summary>
    /// A check item class holds logic for checking the running service itself
    /// </summary>
    internal class SelfLiveCheckItem : ILiveCheckItem
    {
        public async Task<dynamic> ExecuteAsync()
        {
            var result = new SelfLiveCheckResult
            {
                Message = "The Service Is Live!",
                Version = Assembly.GetEntryAssembly().GetName().Version.ToString(4)
            };

            // For now, this uses "fake" async to meet the interface needs. The expectation
            // is that we will want to collect some information, such as server states, 
            // asynchronosly soon
            return await Task.Run(() => result);
        }
    }
}
