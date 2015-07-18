using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

// ReSharper disable once CheckNamespace
namespace UnifiedStorage.WindowsStorage
{
    [DebuggerDisplay("Name = {Name}")]
    internal class WindowsStorageDirectory : IDirectory
    {
        private readonly IStorageFolder _storage;
        private string path;

        public WindowsStorageDirectory(IStorageFolder storage)
        {
            _storage = storage;
        }

        public WindowsStorageDirectory(string path)
        {
            this.path = path;
        }

        public string Name
        {
            get { return _storage.Name; }
        }

        public string Path
        {
            get { return _storage.Path; }
        }

        public Task<IFile> CreateFileAsync(string desiredName, CollisionOption option,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<IFile>> GetFilesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<IFile>> GetFilesAsync(string searchPattern, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<IDirectory> CreateDirectoryAsync(string desiredName, CollisionOption option,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<IDirectory> GetDirectoryAsync(string name, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<IDirectory>> GetDirectoriesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ExistsAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }
    }
}