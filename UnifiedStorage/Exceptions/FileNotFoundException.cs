namespace UnifiedStorage.Exceptions
{
    /// <summary>
    /// An exception that gets thrown if a file could not be found.
    /// </summary>
    public class FileNotFoundException : UnifiedStorageException
    {
        private readonly IFile _file;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNotFoundException"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public FileNotFoundException(IFile file)
            : base(string.Format("The specified file '{0}' could not be found.", file.Path))
        {
            _file = file;
        }

        /// <summary>
        /// Gets the affected file.
        /// </summary>
        /// <value>
        /// The affected file.
        /// </value>
        public IFile File
        {
            get { return _file; }
        }
    }
}
