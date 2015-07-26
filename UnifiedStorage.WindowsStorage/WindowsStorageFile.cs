using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using UnifiedStorage.Extensions;
using UnifiedStorage.WindowsStorage.Extensions;

// ReSharper disable UseNameofExpression
// ReSharper disable UseStringInterpolation
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable CheckNamespace

namespace UnifiedStorage.WindowsStorage
{
    [DebuggerDisplay("Name = {Name}")]
    internal class WindowsStorageFile : IFile
    {
        private readonly IStorageFile _storageFile;
        private string _path;

        /// <summary>
        /// The HRESULT on a System.Exception thrown when a file collision occurs.
        /// </summary>
        internal const int FileAlreadyExists = unchecked((int)0x800700B7);

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
            get { return System.IO.Path.GetFileName(_path); }
        }

        public string Extension
        {
            get { return System.IO.Path.GetExtension(_path); }
        }

        public string Path
        {
            get { return _path; }
        }

        public IDirectory Directory
        {
            get
            {
                var directoryPath = System.IO.Path.GetDirectoryName(_path);
                var storageDir = StorageFolder.GetFolderFromPathAsync(directoryPath).AsTask().Result;

                return new WindowsStorageDirectory(storageDir);
            }
        }

        public async Task<Stream> OpenAsync(FileAccessOption accessOption, CancellationToken cancellationToken = new CancellationToken())
        {
            FileAccessMode fileAccessMode;
            switch (accessOption)
            {
                case FileAccessOption.ReadOnly:
                    fileAccessMode = FileAccessMode.Read;
                    break;
                case FileAccessOption.ReadWrite:
                    fileAccessMode = FileAccessMode.ReadWrite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(accessOption), accessOption, null);
            }

            var wrtStream = await _storageFile.OpenAsync(fileAccessMode)
                .AsTask(cancellationToken)
                .ConfigureAwait(false);

            return wrtStream.AsStream();
        }

        public async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            await EnsureExistsAsync(cancellationToken);

            await _storageFile.DeleteAsync()
                .AsTask(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IFile> RenameAsync(string newName, CollisionOption collisionOption = CollisionOption.FailIfExists,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var directory = System.IO.Path.GetDirectoryName(_path);
            if (newName.StartsWith(directory))
            {
                throw new ArgumentException("The filename must not contain a path", "newName");
            }

            return await MoveAsync(System.IO.Path.Combine(directory, newName), collisionOption, cancellationToken);
        }

        public async Task<IFile> MoveAsync(string newPath, CollisionOption collisionOption = CollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var newFolder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(newPath))
                .AsTask(cancellationToken)
                .ConfigureAwait(false);

            string newName = System.IO.Path.GetFileName(newPath);

            try
            {
                await _storageFile.MoveAsync(newFolder, newName, collisionOption.ToNameCollisionOption())
                    .AsTask(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex.HResult == FileAlreadyExists)
                {
                    throw new Exceptions.UnifiedIOException(string.Format("The file {0} already exists", newPath), ex);
                }

                throw new Exceptions.UnifiedIOException(
                    string.Format("Could not move the file {0} to {1}: {2}", _path, newPath, ex.Message), ex);
            }

            _path = _storageFile.Path;
            return this;
        }

        public async Task<IFile> CopyAsync(string newPath, CollisionOption collisionOption = CollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            await EnsureExistsAsync(cancellationToken);

            StorageFile newFile;

            using (Stream source = await _storageFile.OpenStreamForReadAsync())
            {
                newFile = await StorageFile.GetFileFromPathAsync(newPath)
                    .AsTask(cancellationToken)
                    .ConfigureAwait(false);

                const int bufferSize = 4096;
                using (Stream destination = await newFile.OpenStreamForWriteAsync())
                {
                    await source.CopyToAsync(destination, bufferSize, cancellationToken);
                }
            }

            return new WindowsStorageFile(newFile);
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken)
        {
            return await WindowsStoragePath.ItemExistsAsync(_path, Name, cancellationToken) ==
                   WindowsStoragePath.ExistenceResult.FileExists;
        }

        protected virtual async Task EnsureExistsAsync(CancellationToken cancellationToken)
        {
            if (!await ExistsAsync(cancellationToken))
            {
                throw new Exceptions.FileNotFoundException(this);
            }
        }
    }
}
