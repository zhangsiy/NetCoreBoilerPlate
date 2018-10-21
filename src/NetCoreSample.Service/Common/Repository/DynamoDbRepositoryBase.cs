using NetCoreSample.Service.Common.AwsDynamoDB;

namespace NetCoreSample.Service.Common.Repository
{
    internal class DynamoDbRepositoryBase
    {
        protected IDynamoDBClient DynamoDb { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dynamoDb">The AWS DynamoDb Client</param>
        public DynamoDbRepositoryBase(IDynamoDBClient dynamoDb)
        {
            DynamoDb = dynamoDb;
        }        
    }
}
