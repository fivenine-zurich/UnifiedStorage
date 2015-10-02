using System;

namespace UnifiedStorage.Exceptions
{
    /// <summary>
    /// A IO exception for UnifiedStorage operations.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class UnifiedIOException : UnifiedStorageException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedIOException"/> class.
        /// </summary>
        public UnifiedIOException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedIOException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnifiedIOException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedIOException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public UnifiedIOException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
