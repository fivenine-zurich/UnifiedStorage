using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;

#if MSTEST
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using NUnit.Framework;
#endif

// ReSharper disable CheckNamespace
namespace UnifiedStorage.Shared.Tests
{
    public abstract class FileTests
    {
        protected readonly IFileSystem _filesystem;

        protected FileTests(IFileSystem filesystem)
        {
            _filesystem = filesystem;
        }

        protected string CreateUniqueFileName()
        {
            return Guid.NewGuid() + ".txt";
        }

        protected async Task<IFile> GenerateFileAsync( IDirectory parentDirectory, string filename)
        {
            const int sizeInMb = 2;

            const int blockSize = 1024 * 8;
            const int blocksPerMb = (1024 * 1024) / blockSize;
            byte[] data = new byte[blockSize];

            var random = new Random( (int) DateTime.Now.Ticks);
            var file = await parentDirectory.CreateFileAsync(filename, CollisionOption.FailIfExists);

            using (var writer = new StreamWriter(await file.OpenAsync(FileAccessOption.ReadWrite)))
            {
                for (int i = 0; i < sizeInMb*blocksPerMb; i++)
                {
                    random.NextBytes(data);
                    writer.Write(Convert.ToString(data));
                }

                await writer.FlushAsync();
            }

            return file;
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_a_new_file_class_can_be_created_without_the_need_for_the_file_to_exist()
        {
            var filePath = _filesystem.CreatePath(_filesystem.LocalStorage.Path);

            var file = await _filesystem.GetFileFromPathAsync(filePath.Combine(Guid.NewGuid() + ".txt"));
            file.Should().NotBeNull();

            Debug.WriteLine(file.Name);
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_Name_returns_the_filename_including_its_extension()
        {
            var filePath = _filesystem.CreatePath(_filesystem.LocalStorage.Path);
            var filename = Guid.NewGuid() + ".txt";

            var file = await _filesystem.GetFileFromPathAsync(filePath.Combine(filename));
            file.Should().NotBeNull();
            file.Name.Should().Be(filename);
        }

#if MSTEST
        [DataTestMethod]
        [DataRow("test.txt", ".txt")]
        [DataRow(".gitignore", ".gitignore")]
#else
        [TestCase("test.txt", ".txt")]
        [TestCase(".gitignore", ".gitignore")]
#endif
        public virtual async Task Verify_that_Extension_returns_the_files_extension(string filename,
            string expectedExtension)
        {
            var filePath = _filesystem.CreatePath(_filesystem.LocalStorage.Path);

            var file = await _filesystem.GetFileFromPathAsync(filePath.Combine(filename));
            file.Should().NotBeNull();
            file.Extension.Should().Be(expectedExtension);
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_Exists_returns_false_for_a_nonexistent_file()
        {
            var filePath = _filesystem.CreatePath(_filesystem.LocalStorage.Path);

            var file = await _filesystem.GetFileFromPathAsync(filePath.Combine(Guid.NewGuid() + ".txt"));
            file.Should().NotBeNull();
            var result = await file.ExistsAsync();

            result.Should().BeFalse();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_a_file_can_be_moved_no_collision()
        {
            var filename = CreateUniqueFileName();
            var newFilename = CreateUniqueFileName();

            var folder = _filesystem.LocalStorage;

            var file = await GenerateFileAsync(folder, filename);
            var newFile = await file.MoveAsync(newFilename, CollisionOption.FailIfExists);

            file.Path.Should().Be(newFile.Path);
            file.Name.Should().Be(newFilename);
        }
    }
}
