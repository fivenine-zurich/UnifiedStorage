namespace UnifiedStorage
{
    /// <summary>
    /// Defines a filesystem path.
    /// </summary>
    public interface IPath
    {
        /// <summary>
        /// Gets wheter the current path is the root path of the filesystem.
        /// </summary>
        /// <value>
        ///   <c>True</c> if root; otherwise, <c>false</c>.
        /// </value>
        bool IsRoot { get; }

        /// <summary>
        /// Combines the specified fragments to a new path.
        /// </summary>
        /// <param name="fragments">The fragments to combine.</param>
        /// <returns>The combined path.</returns>
        string Combine(params string[] fragments);
    }
}