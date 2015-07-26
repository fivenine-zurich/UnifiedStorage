using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using UnifiedStorage.WindowsStorage.Extensions;

// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable UseNameofExpression
// ReSharper disable CheckNamespace

namespace UnifiedStorage.WindowsStorage
{
    internal class WindowsStoragePath : IPath
    {
        private readonly string _path;

        public WindowsStoragePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path");
            }

            _path = path;
        }

        public bool IsRoot
        {
            get
            {
                return _path.Equals(ApplicationData.Current.LocalFolder.Path,
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
            // Optimization if the current folder is a know root folder as GetFolderFromPathAsync is unable to retrieve this folder
            if (string.Compare(path, ApplicationData.Current.LocalFolder.Path, StringComparison.OrdinalIgnoreCase) == 0 
                || string.Compare(path, ApplicationData.Current.RoamingFolder.Path, StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(path, ApplicationData.Current.TemporaryFolder.Path, StringComparison.OrdinalIgnoreCase) == 0 )
            {
                return ExistenceResult.DirectoryExists;
            }

            var directory = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(path))
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
                    // Rethrow unexpected exceptions.
                    result.GetAwaiter().GetResult();
                    throw result.Exception;
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