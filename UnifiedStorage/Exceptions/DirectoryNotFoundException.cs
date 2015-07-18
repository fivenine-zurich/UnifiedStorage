namespace UnifiedStorage.Exceptions
{
    /// <summary>
    /// An exception that gets thrown if a directory could not be found.
    /// </summary>
    public class DirectoryNotFoundException : UnifiedStorageException
    {
        private readonly IDirectory _directory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryNotFoundException"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        public DirectoryNotFoundException(IDirectory directory)
            : base(string.Format("The specified directory '{0}' could not be found.", directory.Path))
        {
            _directory = directory;
        }

        /// <summary>
        /// Gets the affected directory.
        /// </summary>
        /// <value>
        /// The affected directory.
        /// </value>
        public IDirectory Directory
        {
            get { return _directory; }
        }
    }
}