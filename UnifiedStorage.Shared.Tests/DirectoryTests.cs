using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable CheckNamespace
namespace UnifiedStorage.Shared.Tests
{
    public abstract class DirectoryTests
    {
        protected readonly IFileSystem Filesystem;

        protected DirectoryTests(IFileSystem filesystem)
        {
            Filesystem = filesystem;
        }

        protected string CreateUniqueFileName()
        {
            return Guid.NewGuid() + ".txt";
        }

        protected async Task<IFile> GenerateFileAsync(IDirectory parentDirectory, string filename)
        {
            const int sizeInMb = 2;

            const int blockSize = 1024 * 8;
            const int blocksPerMb = (1024 * 1024) / blockSize;
            byte[] data = new byte[blockSize];

            var random = new Random((int)DateTime.Now.Ticks);
            var file = await parentDirectory.CreateFileAsync(filename, CollisionOption.FailIfExists);

            using (var writer = new StreamWriter(await file.OpenAsync(FileAccessOption.ReadWrite)))
            {
                for (int i = 0; i < sizeInMb * blocksPerMb; i++)
                {
                    random.NextBytes(data);
                    writer.Write(Convert.ToString(data));
                }

                await writer.FlushAsync();
            }

            return file;
        }

        [Test]
        public virtual async Task Verify_that_a_new_directory_class_can_be_created_without_the_need_for_the_directory_to_exist()
        {
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);

            var directory = await Filesystem.GetFolderFromPathAsync(filePath.Combine(Guid.NewGuid().ToString()));
            directory.Should().NotBeNull();
        }
    }
}
