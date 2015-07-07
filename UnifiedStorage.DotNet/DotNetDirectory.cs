using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnifiedStorage.Extensions;

// ReSharper disable ConvertToAutoProperty
// ReSharper disable UseNameofExpression
// ReSharper disable UseStringInterpolation
// ReSharper disable ConvertPropertyToExpressionBody

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

        public async Task<IFile> CreateFileAsync(string desiredName, CollisionOption option,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            string nameToUse = desiredName;
            string newPath = System.IO.Path.Combine(Path, nameToUse);

            if (File.Exists(newPath))
            {
                switch (option)
                {
                    case CollisionOption.GenerateUniqueName:
                    {
                        string desiredRoot = System.IO.Path.GetFileNameWithoutExtension(desiredName);
                        string desiredExtension = System.IO.Path.GetExtension(desiredName);
                        for (int num = 1; File.Exists(newPath); num++)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            nameToUse = string.Format("{0} ({1}).{2}", desiredRoot, num, desiredExtension);
                            newPath = System.IO.Path.Combine(Path, nameToUse);
                        }

                        break;
                    }
                    case CollisionOption.ReplaceExisting:
                    {
                        File.Delete(newPath);
                        CreateFile(newPath);

                        break;
                    }
                    case CollisionOption.FailIfExists:
                        throw new Exceptions.IOException("Cannot create file, it already exists");

                    case CollisionOption.OpenIfExists:
                    {
                        // Do nothing
                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException("option", option, null);
                }
            }
            else
            {
                //	Create file
                CreateFile(newPath);
            }

            return new DotNetFile(newPath);
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

        private void CreateFile(string filePath)
        {
            using (var stream = File.Create(filePath))
            {
            }
        }
    }
}