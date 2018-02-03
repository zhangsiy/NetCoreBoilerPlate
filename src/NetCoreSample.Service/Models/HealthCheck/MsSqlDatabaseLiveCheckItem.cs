using System;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace NetCoreSample.Models.HealthCheck
{
    /// <summary>
    /// A check item class holds logic for checking a SqlServer connection
    /// </summary>
    internal class MsSqlDatabaseLiveCheckItem : ILiveCheckItem
    {
        /// <summary>
        /// The connection string to check
        /// </summary>
        private string ConnectionString { get; set; }

        public MsSqlDatabaseLiveCheckItem(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<dynamic> ExecuteAsync()
        {
            var result = new DatabaseLiveCheckResult
            {
                ConnectionString = ConnectionString
            };

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    var command = new SqlCommand("Select 1", conn);
                    await command.ExecuteScalarAsync();

                    // At this point we can decalre the connection is up
                    result.Connected = true;
                }
            }
            catch (Exception ex)
            {
                result.Connected = false;
                result.Details = ex;
            }

            return result;
        }
    }
}
