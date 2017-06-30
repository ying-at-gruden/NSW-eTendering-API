using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Utilities.Exceptions
{
    public class InvalidBucketNameException : Exception
    {

        public InvalidBucketNameException() : base()
        {
            
        }

        public InvalidBucketNameException(string message) : base(message)
        {
            
        }

    }
}
