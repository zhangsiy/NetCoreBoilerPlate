using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using NetCoreSample.Data.Exceptions;
using Newtonsoft.Json;

namespace NetCoreSample.Data.Persistence
{
    internal class S3RepositoryBase<TObject> where TObject : class
    {
        protected IAmazonS3 S3Client { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s3Client">The AWS S3 Client</param>
        public S3RepositoryBase(IAmazonS3 s3Client)
        {
            S3Client = s3Client;
        }

        /// <summary>
        /// Get data from S3 based on the given bucket and key. And also deserialize it into
        /// the given domain model by type.
        /// </summary>
        /// <returns>The model object deserialized from the data</returns>
        protected async Task<TObject> ReadModelObject(string bucketName, string keyName)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };
                using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string responseBody = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<TObject>(responseBody, JsonConverters);
                }
            }
            catch (AmazonS3Exception e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new NotFoundException($"The specified key \"{keyName}\" does not exist in S3 bucket \"{bucketName}\"", e);
                }

                // [TODO]:
                // Analyze the S3 exception for other common errors, and log or convert to domain exceptions 
                // for upper level
                throw;
            }
        }

        /// <summary>
        /// Serialize the given model object and store that into S3 at location specified by 
        /// the bucket and key names. 
        /// Note: This saves new entry if non exists by the given bucket and object key, but 
        ///       will overwrite if one such entry exists already.
        /// </summary>
        protected async Task WriteModelObject(string bucketName, string keyName, TObject modelObject)
        {
            try
            {
                string serialized = JsonConvert.SerializeObject(modelObject, JsonConverters);

                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    ContentType = "application/json",
                    ContentBody = serialized,
                    CannedACL = S3CannedACL.PublicRead
                };

                await S3Client.PutObjectAsync(request);
            }
            catch (AmazonS3Exception e)
            {
                // [TODO]:
                // Analyze the S3 exception, and log or convert to domain exceptions 
                // for upper level
                throw new Exception("WriteModelObject error:", e);
            }
        }

        /// <summary>
        /// Json.Net converters to use during serialization/deserialization of models
        /// </summary>
        protected virtual JsonConverter[] JsonConverters
        {
            get { return new JsonConverter[0]; }
        }
    }
}
