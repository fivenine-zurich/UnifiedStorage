using System.Threading;
using System.Threading.Tasks;

namespace UnifiedStorage
{
    /// <summary>
    /// Defines an abstract view of a filesystem. The <see cref="IFileSystem"/> implementation is the main
    /// entry point for working with the Unified file API.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// A folder representing storage which is local to the current device
        /// </summary>
        IDirectory LocalStorage { get; }

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user
        /// </summary>
        IDirectory RoamingStorage { get; }

        /// <summary>
        /// Gets the temporary storage directory.
        /// </summary>
        /// <value>
        /// The temporary storage directory.
        /// </value>
        IDirectory TemporaryStorage { get; }

        /// <summary>
        /// Creates the path object from the given <c>string</c>.
        /// </summary>
        /// <param name="path">The path to create.</param>
        /// <returns>A new instance of a <see cref="IPath"/>.</returns>
        IPath CreatePath(string path);

        /// <summary>
        /// Gets a file, given its path.
        /// </summary>
        /// <param name="path">The path to a file, as returned from the <see cref="IFile.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A file for the given path.</returns>
        Task<IFile> GetFileFromPathAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a directory object from the given path.
        /// </summary>
        /// <param name="path">The path to a directory, as returned from the <see cref="IDirectory.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A folder for the specified path.</returns>
        Task<IDirectory> GetDirectoryFromPathAsync(string path,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
