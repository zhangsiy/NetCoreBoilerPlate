using System;

namespace NetCoreSample.Service.Common.AwsDynamoDB
{
    /// <summary>
    /// Exception thrown where a key is expected to be found but is not in the 
    /// DB
    /// </summary>
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException()
            : base("The key carried by the item does not exist in Database.")
        {
        }
    }
}
