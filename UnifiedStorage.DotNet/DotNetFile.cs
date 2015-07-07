using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnifiedStorage.Extensions;

// ReSharper disable UseNameofExpression
// ReSharper disable UseStringInterpolation
// ReSharper disable ConvertPropertyToExpressionBody

namespace UnifiedStorage.DotNet
{
    internal class DotNetFile : IFile
    {
        private string _path;

        public DotNetFile(string path)
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
                {
                    // Make sure the source file exists
                    EnsureExists();

                    return File.OpenRead(Path);
                }

                case FileAccessOption.ReadWrite:
                    return File.Open(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                default:
                    throw new ArgumentOutOfRangeException("accessOption", accessOption, null);
            }
        }

        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            // Make sure the source file exists
            EnsureExists();

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            
            File.Delete(Path);
        }

        public Task<IFile> RenameAsync(string newName, CollisionOption collisionOption = CollisionOption.FailIfExists,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Make sure the source file exists
            EnsureExists();

            var directoryPath = System.IO.Path.GetDirectoryName(_path);
            if (directoryPath == null)
            {
                throw new ArgumentException("newName");
            }

            return MoveAsync( System.IO.Path.Combine(directoryPath, newName), collisionOption, cancellationToken );
        }

        public async Task<IFile> MoveAsync(string newPath, CollisionOption collisionOption = CollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Make sure the source file exists
            EnsureExists();

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            string newDirectory = System.IO.Path.GetDirectoryName(newPath);
            string newName = System.IO.Path.GetFileName(newPath);

            for (int counter = 1;; counter++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string candidateName = newName;
                if (counter > 1)
                {
                    candidateName = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} ({1}){2}",
                        System.IO.Path.GetFileNameWithoutExtension(newName),
                        counter,
                        System.IO.Path.GetExtension(newName));
                }

                string candidatePath = System.IO.Path.Combine(newDirectory, candidateName);

                if (File.Exists(candidatePath))
                {
                    switch (collisionOption)
                    {
                        case CollisionOption.FailIfExists:
                        {
                            throw new Exceptions.UnifiedIOException("File already exists.");
                        }

                        case CollisionOption.GenerateUniqueName:
                        {
                            // Continue with the loop and generate a new name
                            continue;
                        }

                        case CollisionOption.ReplaceExisting:
                        {
                            File.Delete(candidatePath);
                            break;
                        }
                        default:
                        {
                            throw new ArgumentOutOfRangeException("collisionOption", collisionOption, null);
                        }
                    }
                }

                File.Move(_path, candidatePath);

                _path = candidatePath;
                return this;
            }
        }

        public async Task<IFile> CopyAsync(string newPath, CollisionOption collisionOption = CollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Make sure the source file exists
            EnsureExists();

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            using (Stream source = File.Open(_path, FileMode.Open))
            {
                const int bufferSize = 4096;
                using (Stream destination = File.Create(newPath, bufferSize, FileOptions.Asynchronous))
                {
                    await source.CopyToAsync(destination, bufferSize, cancellationToken);
                }
            }

            return new DotNetFile(newPath);
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            return File.Exists(_path);
        }

        private void EnsureExists()
        {
            if (!File.Exists(_path))
            {
                throw new Exceptions.FileNotFoundException(this);
            }
        }
    }
}
