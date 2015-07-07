using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable CheckNamespace

namespace UnifiedStorage.WindowsStorage
{
    /// <summary>
    /// A <see cref="IFileSystem"/> implementation for the Windows Storage API.
    /// </summary>
    public class FileSystem : IFileSystem
    {
        private ApplicationData _appData;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystem"/> class.
        /// </summary>
        public FileSystem()
        {
            _appData = ApplicationData.Current;
        }

        /// <summary>
        /// A folder representing storage which is local to the current device
        /// </summary>
        public IDirectory LocalStorage
        {
            get { return new WindowsStorageDirectory(_appData.LocalFolder); }
        }

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user
        /// </summary>
        public IDirectory RoamingStorage
        {
            get { return new WindowsStorageDirectory(_appData.RoamingFolder); }
        }

        /// <summary>
        /// Gets the temporary storage directory.
        /// </summary>
        /// <value>
        /// The temporary storage directory.
        /// </value>
        public IDirectory TemporaryStorage
        {
            get { return new WindowsStorageDirectory(_appData.TemporaryFolder); }
        }

        /// <summary>
        /// Creates the path object from the given <c>string</c>.
        /// </summary>
        /// <param name="path">The path to create.</param>
        /// <returns>A new instance of a <see cref="IPath"/>.</returns>
        public IPath CreatePath(string path)
        {
            return new WindowsStoragePath(path);
        }

        /// <summary>
        /// Gets a file, given its path.
        /// </summary>
        /// <param name="path">The path to a file, as returned from the <see cref="IFile.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A file for the given path.</returns>
        public async Task<IFile> GetFileFromPathAsync(string path, CancellationToken cancellationToken = new CancellationToken())
        {
            StorageFile storageFile;
            try
            {
                storageFile = await StorageFile.GetFileFromPathAsync(path)
                    .AsTask(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (FileNotFoundException)
            {
                return new WindowsStorageFile(path);
            }

            return new WindowsStorageFile(storageFile);
        }

        /// <summary>
        /// Gets a folder, given its path.
        /// </summary>
        /// <param name="path">The path to a directory, as returned from the <see cref="IDirectory.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A folder for the specified path.</returns>
        public async Task<IDirectory> GetFolderFromPathAsync(string path, CancellationToken cancellationToken = new CancellationToken())
        {
            StorageFolder storageFolder;
            try
            {
                storageFolder = await StorageFolder.GetFolderFromPathAsync(path)
                    .AsTask(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (FileNotFoundException)
            {
                return new WindowsStorageDirectory(path);
            }

            return new WindowsStorageDirectory(storageFolder);
        }
    }
}
