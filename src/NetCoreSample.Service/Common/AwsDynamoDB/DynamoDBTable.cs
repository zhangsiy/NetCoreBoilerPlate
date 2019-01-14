using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
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

        public async Task<T> GetItemAsync<T>(string partitionKey, string rangeKey)
        {
            Document doc = await InternalTable.GetItemAsync(partitionKey, rangeKey);
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

        public async Task<IEnumerable<T>> GetItemsByPartitionKey<T>(string partitionKey)
        {
            Search search = InternalTable.Query(partitionKey, new QueryFilter());
            IEnumerable<Document> documents = await search.GetRemainingAsync();
            return documents.Select(GetModelObject<T>);
        }

        public async Task<IEnumerable<T>> GetItemsFromSecondaryIndexByPartitionKey<T>(
            string indexName, string partitionKeyName, string partitionKey)
        {
            var queryConfig = new QueryOperationConfig
            {
                IndexName = indexName,
                // It seems like there should be a way to query the secondary index by its partition key 
                // without constructing a generic filter like this, but I could not find one :(
                Filter = new QueryFilter(partitionKeyName, QueryOperator.Equal, partitionKey)
            };
            Search search = InternalTable.Query(queryConfig);
            IEnumerable<Document> documents = await search.GetRemainingAsync();
            return documents.Select(GetModelObject<T>);
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

        public async Task<T> DeleteItemAsync<T>(string partitionKey, string rangeKey)
        {
            Document resultDoc = await InternalTable.DeleteItemAsync(
                partitionKey,
                rangeKey,
                new DeleteItemOperationConfig
                {
                    ReturnValues = ReturnValues.AllOldAttributes
                });
            return GetModelObject<T>(resultDoc);
        }

        public async Task<T> AddItemAsync<T>(T itemToAdd)
        {
            // Add an item does not allow overwrite
            // So add condition to the transaction to prevent it
            // And throw the higher level exception in case the condition fails
            try
            {
                await InternalTable.PutItemAsync(
                    GetDocument(itemToAdd),
                    new PutItemOperationConfig
                    {
                        ConditionalExpression = GetKeyDoesNotExistExpression(itemToAdd)
                    });
            }
            catch (ConditionalCheckFailedException)
            {
                // Condition failed, meaning that the key carried by the object already exists in the DB
                // at the time of the operation. 
                // Throw the proper exception
                throw new KeyAlreadyExistsException();
            }
            return itemToAdd;
        }

        public async Task<T> PutItemAsync<T>(T itemToPut)
        {
            await InternalTable.PutItemAsync(
                GetDocument(itemToPut));
            return itemToPut;
        }

        public async Task<T> PutExistingItemAsync<T>(T itemToPut)
        {
            try
            {
                await InternalTable.PutItemAsync(
                     GetDocument(itemToPut),
                     new PutItemOperationConfig
                     {
                         ConditionalExpression = GetKeyExistsExpression(itemToPut)
                     });
            }
            catch (ConditionalCheckFailedException)
            {
                // Condition failed, meaning that the key carried by the object does not
                // exist in the DB.
                throw new KeyNotFoundException();
            }

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

        /// <summary>
        /// Get a DynamoDB operation condition expression that checks to ensure
        /// the key of the modelObject does not exist in the DB. 
        /// A sample use would be to avoid item overwrite.
        /// </summary>
        private Expression GetKeyDoesNotExistExpression<T>(T modelObject)
        {
            string hashKeyAttributeName = InternalTable.HashKeys.First();
            string rangeKeyAttributeName = InternalTable.RangeKeys.FirstOrDefault();

            if (string.IsNullOrEmpty(rangeKeyAttributeName))
            {
                return new Expression
                {
                    ExpressionAttributeNames =
                    {
                        {"#hashKey", hashKeyAttributeName},
                    },
                    ExpressionAttributeValues =
                    {
                        {":hashKey", modelObject.GetPropertyValue(hashKeyAttributeName)},
                    },
                        ExpressionStatement = "attribute_not_exists(#hashKey) OR #hashKey <> :hashKey",
                };
            }
            else
            {
                return new Expression
                {
                    ExpressionAttributeNames =
                    {
                        {"#hashKey", hashKeyAttributeName},
                        {"#rangeKey", rangeKeyAttributeName }
                    },
                    ExpressionAttributeValues =
                    {
                        {":hashKey", modelObject.GetPropertyValue(hashKeyAttributeName)},
                        {":rangeKey", modelObject.GetPropertyValue(rangeKeyAttributeName)}
                    },
                    ExpressionStatement = "attribute_not_exists(#hashKey) OR #hashKey <> :hashKey OR attribute_not_exists(#rangeKey) OR #rangeKey <> :rangeKey",
                };
            }
        }

        /// <summary>
        /// Get a DynamoDB operation condition expression that checks to ensure
        /// the key of the modelObject indeed exists in the DB. 
        /// A sample use would be to avoid creation of item where the intent was 
        /// to update
        /// </summary>
        private Expression GetKeyExistsExpression<T>(T modelObject)
        {
            string hashKeyAttributeName = InternalTable.HashKeys.First();
            string rangeKeyAttributeName = InternalTable.RangeKeys.FirstOrDefault();

            if (string.IsNullOrEmpty(rangeKeyAttributeName))
            {
                return new Expression
                {
                    ExpressionAttributeNames =
                    {
                        {"#hashKey", hashKeyAttributeName},
                    },
                    ExpressionAttributeValues =
                    {
                        {":hashKey", modelObject.GetPropertyValue(hashKeyAttributeName)},
                    },
                    ExpressionStatement = "attribute_exists(#hashKey) AND #hashKey = :hashKey",
                };
            }
            else
            {
                return new Expression
                {
                    ExpressionAttributeNames =
                    {
                        {"#hashKey", hashKeyAttributeName},
                        {"#rangeKey", rangeKeyAttributeName }
                    },
                    ExpressionAttributeValues =
                    {
                        {":hashKey", modelObject.GetPropertyValue(hashKeyAttributeName)},
                        {":rangeKey", modelObject.GetPropertyValue(rangeKeyAttributeName)}
                    },
                    ExpressionStatement = "attribute_exists(#hashKey) AND #hashKey = :hashKey AND attribute_exists(#rangeKey) AND #rangeKey = :rangeKey",
                };
            }
        }
    }
}
