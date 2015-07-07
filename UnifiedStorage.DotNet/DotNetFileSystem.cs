using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnifiedStorage.Extensions;

namespace UnifiedStorage.DotNet
{
    public abstract class DotNetFileSystem : IFileSystem
    {
        public abstract IDirectory LocalStorage { get; }

        public abstract IDirectory RoamingStorage { get; }

        public IDirectory TemporaryStorage
        {
            get { return new DotNetDirectory(Path.GetTempPath()); }
        }

        public IPath CreatePath(string path)
        {
            return new DotNetPath(path);
        }

        public Task<IDirectory> GetDirectoryAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IFile> GetFileAsync(string path, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            return new DotNetFile(path);
        }
    }
}
