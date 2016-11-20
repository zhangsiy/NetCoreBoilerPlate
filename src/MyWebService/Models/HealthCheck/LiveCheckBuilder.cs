using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MyWebService.Models.HealthCheck
{
    internal class LiveCheckBuilder : ILiveCheckBuilder
    {
        private IList<ILiveCheckItem> LiveCheckItems { get; set; } 

        public LiveCheckBuilder()
        {
            LiveCheckItems = new List<ILiveCheckItem>();
        }

        public ILiveCheckBuilder RegisterSelfCheck()
        {
            LiveCheckItems.Add(new SelfLiveCheckItem());
            return this;
        }

        public ILiveCheckBuilder RegisterSqlServerDatabaseCheck(string connectionString)
        {
            LiveCheckItems.Add(new MsSqlDatabaseLiveCheckItem(connectionString));
            return this;
        }

        public ILiveCheckBuilder RegisterWebServiceCheck(string serviceUrl)
        {
            LiveCheckItems.Add(new WebServiceLiveCheckItem(serviceUrl));
            return this;
        }

        public async Task<dynamic> Run()
        {
            // Start executing all checks in parallel
            var tasks = LiveCheckItems.Select(item => item.ExecuteAsync()).ToList();

            // Batch wait for all tasks to complete
            // Although we should be able to achieve the same result without this 
            // statement, as we don't expect any of the task to fail. But it is better
            // to not make assumptions.
            await Task.WhenAll(tasks);

            // Now await on every task and package up the check result
            // We know that we don't expect blocking while awaiting the task, because
            // the above has guaranteed they have all finished.
            var aggregatedResult = new List<dynamic>();
            tasks.ForEach(async task =>
            {
                aggregatedResult.Add(await task);
            });

            return aggregatedResult;
        }
    }
}
