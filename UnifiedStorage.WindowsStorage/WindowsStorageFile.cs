using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

// ReSharper disable CheckNamespace
namespace UnifiedStorage.WindowsStorage
{
    [DebuggerDisplay("Name = {Name}")]
    public class WindowsStorageFile : IFile
    {
        private IStorageFile _storageFile;
        private string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsStorageFile"/> class.
        /// </summary>
        /// <param name="storageFile">The storage file.</param>
        public WindowsStorageFile(IStorageFile storageFile)
        {
            _storageFile = storageFile;
            _path = storageFile.Path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsStorageFile"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public WindowsStorageFile(string path)
        {
            _path = path;
        }

        public string Name
        {
            get { return WindowsStoragePath.GetFilename(_path); }
        }

        public string Extension
        {
            get { return WindowsStoragePath.GetExtension(_path); }
        }

        public string Path
        {
            get { return _path; }
        }

        public IDirectory Directory
        {
            get
            {
                var directoryPath = WindowsStoragePath.GetDirectoryFromFilePath(_path);
                var storageDir = StorageFolder.GetFolderFromPathAsync(directoryPath).AsTask().Result;

                return new WindowsStorageDirectory(storageDir);
            }
        }

        public Task<Stream> OpenAsync(FileAccessOption accessOption, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IFile> RenameAsync(string newName, CollisionOption collisionOption = CollisionOption.FailIfExists,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IFile> MoveAsync(string newPath, CollisionOption collisionOption = CollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IFile> CopyAsync(string newPath, CollisionOption collisionOption = CollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken)
        {
            if (_storageFile != null)
            {
                return true;
            }

            try
            {
                _storageFile = await StorageFile.GetFileFromPathAsync(_path)
                    .AsTask(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                _storageFile = null;
            }

            return _storageFile != null;
        }
    }
}
