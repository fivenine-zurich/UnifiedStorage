using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UnifiedStorage.WindowsStorage;

namespace UnifiedStorage.WindowsPhone.Tests
{
    [TestClass]
    public class WindowsPhoneFileTests
    {
        private readonly FileSystem Filesystem;

        public WindowsPhoneFileTests()
        {
            Filesystem = new FileSystem();
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

        [TestMethod]
        public virtual async Task Verify_that_a_new_file_class_can_be_created_without_the_need_for_the_file_to_exist()
        {
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);

            var file = await Filesystem.GetFileFromPathAsync(filePath.Combine(Guid.NewGuid() + ".txt"));
            file.Should().NotBeNull();

            Debug.WriteLine(file.Name);
        }

        [TestMethod]
        public virtual async Task Verify_that_Name_returns_the_filename_including_its_extension()
        {
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);
            var filename = Guid.NewGuid() + ".txt";

            var file = await Filesystem.GetFileFromPathAsync(filePath.Combine(filename));
            file.Should().NotBeNull();
            file.Name.Should().Be(filename);
        }

        [DataTestMethod]
        [DataRow("test.txt", ".txt")]
        [DataRow(".gitignore", ".gitignore")]
        public virtual async Task Verify_that_Extension_returns_the_files_extension(string filename,
            string expectedExtension)
        {
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);

            var file = await Filesystem.GetFileFromPathAsync(filePath.Combine(filename));
            file.Should().NotBeNull();
            file.Extension.Should().Be(expectedExtension);
        }

        [TestMethod]
        public virtual async Task Verify_that_Exists_returns_false_for_a_nonexistent_file()
        {
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);

            var file = await Filesystem.GetFileFromPathAsync(filePath.Combine(Guid.NewGuid() + ".txt"));
            file.Should().NotBeNull();
            var result = await file.ExistsAsync();

            result.Should().BeFalse();
        }

        [TestMethod]
        public virtual async Task Verify_that_a_file_can_be_moved_no_collision()
        {
            var filename = CreateUniqueFileName();
            var newFilename = CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var newFilepath = Filesystem.CreatePath(folder.Path).Combine(newFilename);

            var file = await GenerateFileAsync(folder, filename);
            var newFile = await file.MoveAsync(newFilepath, CollisionOption.FailIfExists);

            file.Path.Should().Be(newFile.Path);
            file.Name.Should().Be(newFilename);

            // Cleanup
            await file.DeleteAsync();
        }

        [TestMethod]
        public virtual async Task Verify_that_a_file_can_be_moved_and_replaces_the_destination_if_specified()
        {
            var filename = CreateUniqueFileName();
            var newFilename = CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var newFilepath = Filesystem.CreatePath(folder.Path).Combine(newFilename);

            var file = await GenerateFileAsync(folder, filename);
            await GenerateFileAsync(folder, newFilename);

            var newFile = await file.MoveAsync(newFilepath, CollisionOption.ReplaceExisting);

            file.Path.Should().Be(newFile.Path);
            file.Name.Should().Be(newFile.Name);

            // Cleanup
            await file.DeleteAsync();
        }

        [TestMethod]
        public virtual async Task Verify_that_a_file_can_be_moved_and_a_new_name_is_generated_if_specified()
        {
            var filename = CreateUniqueFileName();
            var newFilename = CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var newFilepath = Filesystem.CreatePath(folder.Path).Combine(newFilename);

            var sourceFile = await GenerateFileAsync(folder, filename);
            var existingFile = await GenerateFileAsync(folder, newFilename);

            var newFile = await sourceFile.MoveAsync(newFilepath, CollisionOption.GenerateUniqueName);

            newFile.Path.Should().NotBe(existingFile.Path);
            newFile.Name.Should().NotBe(existingFile.Name);
            (await newFile.ExistsAsync()).Should().BeTrue();

            // Cleanup
            await existingFile.DeleteAsync();
            await newFile.DeleteAsync();
        }

        [TestMethod]
        public virtual async Task Verify_that_a_MoveAsync_throws_an_exception_if_the_destination_file_exists_and_the_option_is_specified()
        {
            var filename = CreateUniqueFileName();
            var newFilename = CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var newFilepath = Filesystem.CreatePath(folder.Path).Combine(newFilename);

            var sourceFile = await GenerateFileAsync(folder, filename);
            var existingFile = await GenerateFileAsync(folder, newFilename);

            Func<Task> act = () => sourceFile.MoveAsync(newFilepath, CollisionOption.FailIfExists);
            act.ShouldThrow<Exceptions.UnifiedIOException>();

            (await sourceFile.ExistsAsync()).Should().BeTrue("Expecting the source file to be untouched");

            // Cleanup
            await existingFile.DeleteAsync();
            await sourceFile.DeleteAsync();
        }
    }
}
