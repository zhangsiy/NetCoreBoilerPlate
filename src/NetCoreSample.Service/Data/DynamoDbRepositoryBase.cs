using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;

namespace NetCoreSample.Service.Data
{
    internal class DynamoDbRepositoryBase<TObject> where TObject : class
    {
        protected IAmazonDynamoDB DynamoDb { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dynamoDb">The AWS DynamoDb Client</param>
        public DynamoDbRepositoryBase(IAmazonDynamoDB dynamoDb)
        {
            DynamoDb = dynamoDb;
        }

        protected TObject GetModelObject(Document document)
        {
            return document != null
                ? JsonConvert.DeserializeObject<TObject>(document.ToJson())
                : null;
        }

        protected Document GetDocument(TObject modelObject)
        {
            return modelObject != null
                ? Document.FromJson(JsonConvert.SerializeObject(modelObject))
                : null;
        }
    }
}
