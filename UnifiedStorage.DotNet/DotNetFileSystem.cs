using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnifiedStorage.Extensions;

// ReSharper disable ConvertPropertyToExpressionBody

namespace UnifiedStorage.DotNet
{
    public abstract class DotNetFileSystem : IFileSystem
    {
        /// <summary>
        /// A folder representing storage which is local to the current device
        /// </summary>
        public abstract IDirectory LocalStorage { get; }

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user
        /// </summary>
        public abstract IDirectory RoamingStorage { get; }

        /// <summary>
        /// Gets the temporary storage directory.
        /// </summary>
        /// <value>
        /// The temporary storage directory.
        /// </value>
        public IDirectory TemporaryStorage
        {
            get { return new DotNetDirectory(Path.GetTempPath()); }
        }

        /// <summary>
        /// Creates the path object from the given <c>string</c>.
        /// </summary>
        /// <param name="path">The path to create.</param>
        /// <returns>A new instance of a <see cref="IPath"/>.</returns>
        public IPath CreatePath(string path)
        {
            return new DotNetPath(path);
        }

        /// <summary>
        /// Gets a file, given its path.
        /// </summary>
        /// <param name="path">The path to a file, as returned from the <see cref="IFile.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A file for the given path.</returns>
        public async Task<IFile> GetFileFromPathAsync(string path, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            return new DotNetFile(path);
        }

        /// <summary>
        /// Gets a folder, given its path.
        /// </summary>
        /// <param name="path">The path to a directory, as returned from the <see cref="IDirectory.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A folder for the specified path.</returns>
        public async Task<IDirectory> GetFolderFromPathAsync(string path, CancellationToken cancellationToken = new CancellationToken())
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            return new DotNetDirectory(path);
        }
    }
}
