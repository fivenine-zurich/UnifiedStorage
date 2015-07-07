using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace UnifiedStorage
{
    /// <summary>
    /// A file in the filesystem.
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <value>
        /// The file extension, eg <c>.txt</c>.
        /// </value>
        string Extension { get; }

        /// <summary>
        /// Gets the path of the file.
        /// </summary>
        /// <value>
        /// The file path including the filename.
        /// </value>
        string Path { get; }

        /// <summary>
        /// Gets the directory the current file resides in.
        /// </summary>
        /// <value>
        /// The <see cref="IDirectory"/> of the current file.
        /// </value>
        IDirectory Directory { get; }

        /// <summary>
        /// Opens the file
        /// </summary>
        /// <param name="accessOption">Specifies whether the file should be opened in read-only or read/write mode</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Stream"/> which can be used to read from or write to the file</returns>
        Task<Stream> OpenAsync(FileAccessOption accessOption,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is deleted.
        /// </returns>
        Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Renames a file without changing its location.
        /// </summary>
        /// <param name="newName">The new leaf name of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is renamed.
        /// </returns>
        Task<IFile> RenameAsync(string newName, CollisionOption collisionOption = CollisionOption.FailIfExists,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="newPath">The new full path of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        Task<IFile> MoveAsync(string newPath, CollisionOption collisionOption = CollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copies the file to the new location.
        /// </summary>
        /// <param name="newPath">The new full path of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        Task<IFile> CopyAsync(string newPath, CollisionOption collisionOption = CollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Checks if the current file exists.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><c>True</c> if the file exists; otherwise <c>false</c>.</returns>
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}