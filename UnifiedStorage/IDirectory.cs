using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnifiedStorage
{
    /// <summary>
    /// A directory in the filesystem.
    /// </summary>
    public interface IDirectory
    {
        /// <summary>
        /// Gets the name of the directory.
        /// </summary>
        /// <value>
        /// The directory name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the full path of the directory including its name.
        /// </summary>
        /// <value>
        /// The full directory path.
        /// </value>
        string Path { get; }

        /// <summary>
        /// Creates a file in this folder
        /// </summary>
        /// <param name="desiredName">The name of the file to create</param>
        /// <param name="option">Specifies how to behave if the specified file already exists</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly created file</returns>
        Task<IFile> CreateFileAsync(string desiredName, CollisionOption option, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a file in this folder
        /// </summary>
        /// <param name="name">The name of the file to get</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The requested file, or null if it does not exist</returns>
        Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of the files in this folder
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of the files in the folder</returns>
        Task<IList<IFile>> GetFilesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of the files in this folder
        /// </summary>
        /// <param name="searchPattern">The file pattern.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A list of the files in the folder
        /// </returns>
        Task<IList<IFile>> GetFilesAsync(string searchPattern, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a subfolder in this folder
        /// </summary>
        /// <param name="desiredName">The name of the folder to create</param>
        /// <param name="option">Specifies how to behave if the specified folder already exists</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly created folder</returns>
        Task<IDirectory> CreateFolderAsync(string desiredName, CollisionOption option, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a subfolder in this folder
        /// </summary>
        /// <param name="name">The name of the folder to get</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The requested folder, or null if it does not exist</returns>
        Task<IDirectory> GetFolderAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of subfolders in this folder
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of subfolders in the folder</returns>
        Task<IList<IDirectory>> GetFoldersAsync(CancellationToken cancellationToken = default(CancellationToken));

        ///// <summary>
        ///// Checks whether a folder or file exists at the given location.
        ///// </summary>
        ///// <param name="name">The name of the file or folder to check for.</param>
        ///// <param name="cancellationToken">The cancellation token.</param>
        ///// <returns>A task whose result is the result of the existence check.</returns>
        //Task<ExistenceCheckResult> CheckExistsAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes this folder and all of its contents
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the folder is deleted</returns>
        Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
