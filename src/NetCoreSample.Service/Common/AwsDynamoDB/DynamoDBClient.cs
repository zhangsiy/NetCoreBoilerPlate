using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace NetCoreSample.Service.Common.AwsDynamoDB
{
    /// <summary>
    /// 
    /// </summary>
    internal class DynamoDBClient : IDynamoDBClient
    {
        private IAmazonDynamoDB DynamoDb { get; }

        public DynamoDBClient(IAmazonDynamoDB dynamoDb)
        {
            DynamoDb = dynamoDb;
        }

        public IDynamoDBTable LoadTable(string tableName)
        {
            Table table = Table.LoadTable(DynamoDb, tableName);
            return new DynamoDBTable(table);
        }
    }
}
