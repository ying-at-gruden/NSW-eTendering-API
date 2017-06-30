using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Utilities.Exceptions
{
    public class AuditServiceNotInitialisedException : Exception
    {

        public AuditServiceNotInitialisedException() : base()
        {
            
        }

        public AuditServiceNotInitialisedException(string message) : base(message)
        {
            
        }

    }
}
