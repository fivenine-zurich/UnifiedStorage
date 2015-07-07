using System;

namespace UnifiedStorage.Exceptions
{
    public class IOException : UnifiedStorageException
    {
        public IOException()
        {
        }

        public IOException(string message) 
            : base(message)
        {
        }

        public IOException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
