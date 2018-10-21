using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreSample.Service.Common.AwsDynamoDB
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDynamoDBTable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        Task<T> GetItemAsync<T>(string hashKey);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKeys"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetItemsAsync<T>(IList<string> hashKeys);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllItemsAsync<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKeys"></param>
        /// <returns></returns>
        Task DeleteItemsAsync<T>(List<string> hashKeys);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        Task<T> DeleteItemAsync<T>(string hashKey);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsToPut"></param>
        /// <returns></returns>
        Task PutItemsAsync<T>(List<T> itemsToPut);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemToPut"></param>
        /// <returns></returns>
        Task<T> PutItemAsync<T>(T itemToPut);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> ScanAsync<T>(ScanFilter filter);
    }
}
