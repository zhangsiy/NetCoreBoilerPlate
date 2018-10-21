using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreSample.Service.Common.AwsDynamoDB
{
    /// <summary>
    /// 
    /// </summary>
    internal class DynamoDBTable : IDynamoDBTable
    {
        private Table InternalTable { get; }

        public DynamoDBTable(Table table)
        {
            InternalTable = table;
        }

        public async Task<T> GetItemAsync<T>(string hashKey)
        {
            Document doc = await InternalTable.GetItemAsync(hashKey);
            return GetModelObject<T>(doc);
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(IList<string> hashKeys)
        {
            var batchGet = InternalTable.CreateBatchGet();
            hashKeys.ForEach(id => batchGet.AddKey(id));
            await batchGet.ExecuteAsync();

            return batchGet.Results.Select(GetModelObject<T>);
        }

        public async Task<IEnumerable<T>> GetAllItemsAsync<T>()
        {
            IEnumerable<Document> allDocuments = await InternalTable.Scan(new ScanFilter()).GetRemainingAsync();
            return allDocuments.Select(GetModelObject<T>);
        }

        public async Task DeleteItemsAsync<T>(List<string> hashKeys)
        {
            DocumentBatchWrite batchWrite = InternalTable.CreateBatchWrite();
            hashKeys.ForEach(key => batchWrite.AddKeyToDelete(key));
            await batchWrite.ExecuteAsync();
        }

        public async Task<T> DeleteItemAsync<T>(string hashKey)
        {
            Document resultDoc = await InternalTable.DeleteItemAsync(
                hashKey,
                new DeleteItemOperationConfig
                {
                    ReturnValues = ReturnValues.AllOldAttributes
                });
            return GetModelObject<T>(resultDoc);
        }

        public async Task<T> PutItemAsync<T>(T itemToPut)
        {
            await InternalTable.PutItemAsync(
                GetDocument(itemToPut));
            return itemToPut;
        }

        public async Task PutItemsAsync<T>(List<T> itemsToPut)
        {
            DocumentBatchWrite batchWrite = InternalTable.CreateBatchWrite();

            itemsToPut.ForEach(item =>
            {
                batchWrite.AddDocumentToPut(GetDocument(item));
            });

            await batchWrite.ExecuteAsync();
        }

        public async Task<IEnumerable<T>> ScanAsync<T>(ScanFilter filter)
        {
            Search search = InternalTable.Scan(filter);
            List<Document> matches = await search.GetRemainingAsync();
            return matches.Select(GetModelObject<T>);
        }

        private T GetModelObject<T>(Document document)
        {
            return document != null
                ? JsonConvert.DeserializeObject<T>(document.ToJson())
                : default(T);
        }

        private Document GetDocument<T>(T modelObject)
        {
            // This for now relies on Json serialize the model object
            // and then map the json attributes into the DynamoDb document
            // This makes the assumption that all DynamoDb attributes are
            // Pascal cased, and the same goes for corresponding properties
            // on the model object. 
            // If it is found that this assumption becomes problematic or
            // hard to enforce via convention, then we might need to resort
            // and consider asking for explicit attribute mapping. See
            // Document.FromAttributeMap
            return modelObject != null
                ? Document.FromJson(
                    JsonConvert.SerializeObject(
                        modelObject,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new DefaultContractResolver()
                        }))
                : null;
        }
    }
}
