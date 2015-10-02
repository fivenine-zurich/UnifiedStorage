using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using UnifiedStorage.Exceptions;
using UnifiedStorage.Extensions;
using UnifiedStorage.WindowsStorage.Extensions;
using FileNotFoundException = System.IO.FileNotFoundException;

// ReSharper disable MergeConditionalExpression
// ReSharper disable UseNameofExpression
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable once CheckNamespace

namespace UnifiedStorage.WindowsStorage
{
    [DebuggerDisplay("Name = {Name}")]
    internal class WindowsStorageDirectory : IDirectory
    {
        private readonly IStorageFolder _storage;
        private readonly string _path;

        public WindowsStorageDirectory(IStorageFolder storage)
        {
            _storage = storage;
            _path = storage.Path;
        }

        public WindowsStorageDirectory(string path)
        {
            _path = path;
        }

        public string Name
        {
            get { return _storage != null ? _storage.Name : System.IO.Path.GetDirectoryName(_path); }
        }

        public string Path
        {
            get { return _path; }
        }

        public async Task<IFile> CreateFileAsync(string desiredName, CollisionOption option,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await EnsureExistsAsync(cancellationToken)
                .ConfigureAwait(false);

            StorageFile file;
            try
            {
                file = await _storage.CreateFileAsync(desiredName, option.ToCreationCollisionOption())
                    .AsTask(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex.HResult == WindowsStorageFile.FileAlreadyExists)
                {
                    //  File already exists (and potentially other failures, not sure what the HResult represents)
                    throw new UnifiedIOException(ex.Message, ex);
                }
                throw;
            }

            return new WindowsStorageFile(file);
        }

        public async Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken = new CancellationToken())
        {
            await EnsureExistsAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var file = await _storage.GetFileAsync(name)
                    .AsTask(cancellationToken)
                    .ConfigureAwait(false);

                return new WindowsStorageFile(file);
            }
            catch (FileNotFoundException)
            {
                throw new Exceptions.FileNotFoundException(new WindowsStorageFile(name));
            }
        }

        public async Task<IList<IFile>> GetFilesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await EnsureExistsAsync(cancellationToken)
                .ConfigureAwait(false);

            var storageFiles = await _storage.GetFilesAsync()
                .AsTask(cancellationToken)
                .ConfigureAwait(false);

            var files = storageFiles.Select(f => new WindowsStorageFile(f)).ToList<IFile>();
            return new ReadOnlyCollection<IFile>(files);
        }

        public async Task<IList<IFile>> GetFilesAsync(string searchPattern, CancellationToken cancellationToken = new CancellationToken())
        {
            var allFiles = await GetFilesAsync(cancellationToken);
            return allFiles.Where(f => FitsMask(f.Name, searchPattern))
                .ToList();
        }

        public async Task<IDirectory> CreateDirectoryAsync(string desiredName, CollisionOption option,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await EnsureExistsAsync(cancellationToken).ConfigureAwait(false);
            StorageFolder storageFolder;
            try
            {
                storageFolder = await _storage.CreateFolderAsync(desiredName, option.ToCreationCollisionOption() )
                    .AsTask(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex.HResult == WindowsStorageFile.FileAlreadyExists)
                {
                    //  Folder already exists (and potentially other failures, not sure what the HResult represents)
                    throw new UnifiedIOException(ex.Message, ex);
                }

                throw;
            }

            return new WindowsStorageDirectory(storageFolder);
        }

        public async Task<IDirectory> GetDirectoryAsync(string name, CancellationToken cancellationToken = new CancellationToken())
        {
            await EnsureExistsAsync(cancellationToken)
                .ConfigureAwait(false);

            try
            {
                var storageDir = await _storage.GetFolderAsync(name)
                    .AsTask(cancellationToken)
                    .ConfigureAwait(false);

                return new WindowsStorageDirectory(storageDir);
            }
            catch (FileNotFoundException)
            {
                return new WindowsStorageDirectory(System.IO.Path.Combine(_path, name));
            }
        }

        public Task<IList<IDirectory>> GetDirectoriesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return await WindowsStoragePath.ItemExistsAsync(_path, Name, cancellationToken) ==
                   WindowsStoragePath.ExistenceResult.DirectoryExists;
        }

        public async Task DeleteAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            await EnsureExistsAsync(cancellationToken);

            await _storage.DeleteAsync(StorageDeleteOption.Default);
        }

        private async Task EnsureExistsAsync(CancellationToken cancellationToken)
        {
            if (await ExistsAsync(cancellationToken) == false)
            {
                throw new DirectoryNotFoundException(this);
            }
        }

        private static bool FitsMask(string fileName, string fileMask)
        {
            Regex mask = new Regex(
                '^' +
                fileMask
                    .Replace(".", "[.]")
                    .Replace("*", ".*")
                    .Replace("?", ".")
                + '$',
                RegexOptions.IgnoreCase);
            return mask.IsMatch(fileName);
        }
    }
}