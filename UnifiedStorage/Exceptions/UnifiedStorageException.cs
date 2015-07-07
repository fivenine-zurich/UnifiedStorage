using System;

namespace UnifiedStorage.Exceptions
{
    /// <summary>
    /// The base class for all storage related exceptions.
    /// </summary>
    public abstract class UnifiedStorageException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedStorageException"/> class.
        /// </summary>
        protected UnifiedStorageException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedStorageException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        protected UnifiedStorageException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedStorageException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        protected UnifiedStorageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
