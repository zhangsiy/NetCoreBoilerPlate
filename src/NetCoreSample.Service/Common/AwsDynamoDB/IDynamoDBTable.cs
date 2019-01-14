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
        /// <param name="partitionKey"></param>
        /// <param name="rangeKey"></param>
        /// <returns></returns>
        Task<T> GetItemAsync<T>(string partitionKey, string rangeKey);

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
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetItemsByPartitionKey<T>(string partitionKey);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="partitionKeyName"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetItemsFromSecondaryIndexByPartitionKey<T>(string indexName, string partitionKeyName, string partitionKey);

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
        /// <param name="partitionKey"></param>
        /// <param name="rangeKey"></param>
        /// <returns></returns>
        Task<T> DeleteItemAsync<T>(string partitionKey, string rangeKey);

        /// <summary>
        /// Add an item to the table, if the primary key carried by the item
        /// does not yet exist in the table.
        /// </summary>
        /// <exception cref="KeyAlreadyExistsException">If the key already exists in the table</exception>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemToAdd"></param>
        /// <returns></returns>
        Task<T> AddItemAsync<T>(T itemToAdd);

        /// <summary>
        /// Put an item into the table.
        /// If an item with the same key already exists, this will overwrite it.
        /// Otherwise, this would create the item.
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
        /// Put an item into the table, if the item carries a key already existing
        /// in the table. i.e. this performs an overwrite only.
        /// </summary>
        /// <exception cref="KeyNotFoundException">If the key is not found in the table</exception>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemToPut"></param>
        /// <returns></returns>
        Task<T> PutExistingItemAsync<T>(T itemToPut);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> ScanAsync<T>(ScanFilter filter);
    }
}
