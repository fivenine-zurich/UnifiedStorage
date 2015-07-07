using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using UnifiedStorage.WindowsStorage.Extensions;

// ReSharper disable CheckNamespace
namespace UnifiedStorage.WindowsStorage
{
    internal class WindowsStoragePath : IPath
    {
        private readonly string _path;

        public WindowsStoragePath(string path)
        {
            _path = path;
        }

        public bool IsRoot
        {
            get
            {
                return _path.Equals(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        public string Combine(params string[] fragments)
        {
            return fragments.Aggregate(_path, (s, s1) => Path.Combine(s, s1));
        }

        internal enum ExistenceResult
        {
            FileExists,
            DirectoryExists,
            NotFound
        }

        internal static async Task<ExistenceResult> ItemExistsAsync(string path, string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var directory = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(path))
                .AsTask(cancellationToken)
                .ConfigureAwait(false);


            var result = await directory.GetItemAsync(name)
                .AsTaskNoThrow(cancellationToken);

            if (result.IsFaulted)
            {
                if (result.Exception.InnerException is FileNotFoundException)
                {
                    return ExistenceResult.NotFound;
                }
                else
                {
                    // rethrow unexpected exceptions.
                    result.GetAwaiter().GetResult();
                    throw result.Exception; // shouldn't reach here anyway.
                }
            }
            else if (result.IsCanceled)
            {
                throw new OperationCanceledException();
            }
            else
            {
                IStorageItem storageItem = result.Result;
                if (storageItem.IsOfType(StorageItemTypes.File))
                {
                    return ExistenceResult.FileExists;
                }
                else if (storageItem.IsOfType(StorageItemTypes.Folder))
                {
                    return ExistenceResult.DirectoryExists;
                }
                else
                {
                    return ExistenceResult.NotFound;
                }
            }
        }
    }
}