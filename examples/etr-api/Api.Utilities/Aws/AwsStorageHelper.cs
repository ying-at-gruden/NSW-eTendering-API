using System;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace Api.Utilities.Aws
{
    public class AwsStorageHelper
    {

        public AwsCredentialsHelper Credentials { get; set; }

        public string ServiceUrl { get; }

        public AwsStorageHelper(string serviceUrl)
        {
            ServiceUrl = serviceUrl;
        }

        /// <summary>
        /// Generate temporary URL to give public users access to a file
        /// </summary>
        /// <param name="bucket_name">S3 bucket name</param>
        /// <param name="object_key">S3 object ID</param>
        /// <param name="timeout_minutes">timeout in minutes for URL to be valid</param>
        /// <returns></returns>
        public string GeneratePreSignedUrl(string bucket_name, string object_key, int timeout_minutes)
        {
            string urlString;

            var urlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucket_name,
                Key = object_key,
                Expires = DateTime.Now.AddMinutes(timeout_minutes)

            };

            using (var client = GetClient())
            {
                urlString = client.GetPreSignedURL(urlRequest);
            }

            return urlString;
        }


        /// <summary>
        /// Generate temporary URL to give public users access to a file
        /// </summary>
        /// <param name="bucket_name">S3 bucket name</param>
        /// <param name="object_key">S3 object ID</param>
        /// <param name="timeout_minutes">timeout in minutes for URL to be valid</param>
        /// <returns></returns>
        public string GeneratePreSignedUploadUrl(string bucket_name, string object_key, string content_type, int timeout_minutes)
        {
            string urlString;

            var urlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucket_name,
                Key = object_key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.Now.AddMinutes(timeout_minutes),
                ContentType = content_type

            };

            using (var client = GetClient())
            {
                urlString = client.GetPreSignedURL(urlRequest);
            }

            return urlString;
        }


        /// <summary>
        /// Get S3 Client for connecting to S3
        /// </summary>
        /// <returns>S3 Client</returns>
        public IAmazonS3 GetClient()
        {
            var config = new AmazonS3Config { ServiceURL = ServiceUrl };

            return new AmazonS3Client(config);
        }
    }
}
