using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

// ReSharper disable CheckNamespace
namespace UnifiedStorage.WindowsStorage
{
    public class FileSystem : IFileSystem
    {
        private ApplicationData _appData;

        public FileSystem()
        {
            _appData = ApplicationData.Current;
        }


        public IDirectory LocalStorage
        {
            get { return new WindowsStorageDirectory(_appData.LocalFolder); }
        }

        public IDirectory RoamingStorage
        {
            get { return new WindowsStorageDirectory(_appData.RoamingFolder); }
        }

        public IDirectory TemporaryStorage
        {
            get { return new WindowsStorageDirectory(_appData.TemporaryFolder); }
        }

        public IPath CreatePath(string path)
        {
            return new WindowsStoragePath(path);
        }

        public Task<IDirectory> GetDirectoryAsync(string path, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public async Task<IFile> GetFileAsync(string path,
            CancellationToken cancellationToken = default(CancellationToken))
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
    }
}
