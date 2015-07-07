using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnifiedStorage.Extensions;

namespace UnifiedStorage.DotNet
{
    internal class DotNetFile : IFile
    {
        private readonly string _path;
        private readonly string _name;

        public DotNetFile(string path)
        {
            _path = path;
            _name = System.IO.Path.GetFileName(path);
        }

        public string Name
        {
            get { return _name; }
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
                var dir = System.IO.Path.GetDirectoryName(_path);
                return new DotNetDirectory(dir);
            }
        }

        public async Task<Stream> OpenAsync(FileAccessOption accessOption, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            switch (accessOption)
            {
                case FileAccessOption.ReadOnly:
                    return File.OpenRead(Path);

                case FileAccessOption.ReadWrite:
                    return File.Open(Path, FileMode.Open, FileAccess.ReadWrite);

                default:
                    throw new ArgumentOutOfRangeException("accessOption", accessOption, null);
            }
        }

        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (!File.Exists(Path))
            {
                throw new Exceptions.FileNotFoundException(this);
            }

            File.Delete(Path);
        }

        public Task<IFile> RenameAsync(string newName, CollisionOption collisionOption = CollisionOption.FailIfExists,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IFile> MoveAsync(string newPath, CollisionOption collisionOption = CollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IFile> CopyAsync(string newPath, CollisionOption collisionOption = CollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            using (Stream source = File.Open(_path, FileMode.Open))
            {
                const int bufferSize = 4084;
                using (Stream destination = File.Create(newPath, bufferSize, FileOptions.Asynchronous))
                {
                    await source.CopyToAsync(destination);
                }
            }

            return new DotNetFile(newPath);
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            return File.Exists(_path);
        }
    }
}
