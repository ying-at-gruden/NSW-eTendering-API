using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Api.Utilities.Aws
{
    public class AwsCredentialsHelper
    {

        public string AccessKey { get; }
        public string SecretKey { get; }

        public AwsCredentialsHelper(string accessKey, string secretKey)
        {
            AccessKey = accessKey;
            SecretKey = secretKey;
        }

    }
}
