using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Api.Utilities.Aws;
using Api.Utilities.Exceptions;

namespace Api.Utilities.FileStorage
{
    public class AwsStorageClient : IStorageClient
    {

        /// <summary>
        /// Storage Helper - this is injected by Windsor. Provides the S3 client.
        /// </summary>
        public AwsStorageHelper Storage { get; set; }

        /// <summary>
        /// Bucket Name - this must be set before use.
        /// </summary>
        public string BucketName { get; set; }

        public bool Exists(string file_path)
        {
            EnsureBucketName();

            using (var client = Storage.GetClient())
            {
                var s3DirectoryInfo = new S3DirectoryInfo(client, BucketName);

                var path = file_path.Split('/');
                for (var i = 0; i < path.Length - 1; i++)
                {
                    s3DirectoryInfo = s3DirectoryInfo.GetDirectory(path[i]);
                }

                return s3DirectoryInfo.GetFiles(path.Last()).Any();
            }
        }

        public void DeleteFile(string file_path)
        {
            EnsureBucketName();

            using (var client = Storage.GetClient())
            {
                client.DeleteObject(BucketName, file_path);
            }
        }


        public void DeleteFolder(string file_path)
        {
            EnsureBucketName();

            using (var client = Storage.GetClient())
            {
                var directoryToDelete = new S3DirectoryInfo(client, BucketName, file_path);
                directoryToDelete.Delete(true);
            }
        }


        public Stream ReadFile(string file_path)
        {
            EnsureBucketName();

            using (var client = Storage.GetClient())
            {
                var response = client.GetObject(BucketName, file_path);
                return response.ResponseStream;
            }
        }

        /// <summary>
        /// List bucket contents.
        /// Filter can be used to designate a directory, as long as it contains a forward slash
        /// </summary>
        /// <param name="filter">If it contains a forward slash, is directory</param>
        /// <returns></returns>
        public string[] List(string filter)
        {
            EnsureBucketName();

            var path = "";
            var exp = @"^([^\*]+/)([^/]*)$";
            if (Regex.IsMatch(filter, exp))
            {
                path = Regex.Replace(filter, exp, "$1");
                filter = Regex.Replace(filter, exp, "$2");
            }

            using (var client = Storage.GetClient())
            {
                var s3DirectoryInfo = new S3DirectoryInfo(client, BucketName);
                foreach (var bit in path.Split('/'))
                {
                    if (bit.Length > 0)
                    {
                        s3DirectoryInfo = s3DirectoryInfo.GetDirectory(bit);
                    }
                }
                return s3DirectoryInfo
                    // get files
                    .GetFiles(filter)
                    .Select(f => f.Name)
                    // get directories
                    .Union(
                        s3DirectoryInfo
                            .GetDirectories(filter)
                            // add a trailing slash to distinguish directories
                            .Select(f => string.Format("{0}{1}", f.Name, "/"))
                    )
                    .ToArray();
            }
        }

        private void EnsureBucketName()
        {
            if (BucketName == string.Empty)
            {
                throw new InvalidBucketNameException("Bucket Name must be set before using the S3 storage client");
            }
        }

    }
}