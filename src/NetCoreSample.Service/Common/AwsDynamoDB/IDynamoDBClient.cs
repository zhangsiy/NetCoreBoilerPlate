namespace NetCoreSample.Service.Common.AwsDynamoDB
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDynamoDBClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IDynamoDBTable LoadTable(string tableName);
    }
}
