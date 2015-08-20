using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Provider.Exceptions
{
    [Serializable]
    public class UnlockCallWithoutHeldLock : Exception
    {
        public UnlockCallWithoutHeldLock()
        {
            throw new InvalidOperationException("UnlockCallWithoutHeldLock");
        }

        public UnlockCallWithoutHeldLock(string message)
        {            
            throw new InvalidOperationException(message);
        }

        public UnlockCallWithoutHeldLock(string message, Exception ex)
        {
            throw new InvalidOperationException(message, ex);
        }
    }
}
