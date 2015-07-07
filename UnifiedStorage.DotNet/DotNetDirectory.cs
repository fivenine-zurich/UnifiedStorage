using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnifiedStorage.Extensions;

namespace UnifiedStorage.DotNet
{
    internal class DotNetDirectory : IDirectory
    {
        private readonly string _path;
        private readonly string _name;

        public DotNetDirectory(string path)
        {
            _path = System.IO.Path.GetFullPath(path);
            _name = System.IO.Path.GetDirectoryName(path);
        }

        public string Name
        {
            get { return _name; }
        }

        public string Path
        {
            get { return _path; }
        }

        public Task<IFile> CreateFileAsync(string desiredName, CollisionOption option,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IList<IFile>> GetFilesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IDirectory> CreateFolderAsync(string desiredName, CollisionOption option,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IDirectory> GetFolderAsync(string name, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IList<IDirectory>> GetFoldersAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            Directory.Delete(_path, true);
        }
    }
}