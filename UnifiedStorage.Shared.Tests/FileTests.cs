using System;
using System.Diagnostics;
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
        protected readonly IFileSystem Filesystem;

        protected FileTests(IFileSystem filesystem)
        {
            Filesystem = filesystem;
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_a_new_file_class_can_be_created_without_the_need_for_the_file_to_exist()
        {
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);

            var file = await Filesystem.GetFileFromPathAsync(filePath.Combine(Guid.NewGuid() + ".txt"));
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
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);
            var filename = Guid.NewGuid() + ".txt";

            var file = await Filesystem.GetFileFromPathAsync(filePath.Combine(filename));
            file.Should().NotBeNull();
            file.Name.Should().Be(filename);
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_Path_returns_the_full_path_of_the_file_including_its_filename()
        {
            var fileFolder = Filesystem.CreatePath(Filesystem.LocalStorage.Path);
            var filename = Guid.NewGuid() + ".txt";
            var fullPath = fileFolder.Combine(filename);

            var file = await Filesystem.GetFileFromPathAsync(fullPath);

            file.Should().NotBeNull();
            file.Path.Should().Be(fullPath);
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
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);

            var file = await Filesystem.GetFileFromPathAsync(filePath.Combine(filename));
            file.Should().NotBeNull();
            file.Extension.Should().Be(expectedExtension);
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_ExistsAsync_returns_false_for_a_nonexistent_file()
        {
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);

            var file = await Filesystem.GetFileFromPathAsync(filePath.Combine(Guid.NewGuid() + ".txt"));
            file.Should().NotBeNull();
            var result = await file.ExistsAsync();

            result.Should().BeFalse();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_ExistsAsync_returns_true_for_an_existing_file()
        {
            var filename = Helper.CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;
            var sourceFile = await Helper.GenerateFileAsync(folder, filename);

            (await sourceFile.ExistsAsync()).Should().BeTrue();

            // Cleanup
            await sourceFile.DeleteAsync();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_MoveAsync_succeeds()
        {
            var filename = Helper.CreateUniqueFileName();
            var newFilename = Helper.CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var newFilepath = Filesystem.CreatePath(folder.Path).Combine(newFilename);

            var file = await Helper.GenerateFileAsync(folder, filename);
            var newFile = await file.MoveAsync(newFilepath, CollisionOption.FailIfExists);

            file.Path.Should().Be(newFile.Path);
            file.Name.Should().Be(newFilename);

            // Cleanup
            await file.DeleteAsync();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_MoveAsync_succeeds_and_replaces_the_destination_if_ReplaceExisting_is_specified()
        {
            var filename = Helper.CreateUniqueFileName();
            var newFilename = Helper.CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var newFilepath = Filesystem.CreatePath(folder.Path).Combine(newFilename);

            var file = await Helper.GenerateFileAsync(folder, filename);
            await Helper.GenerateFileAsync(folder, newFilename);

            var newFile = await file.MoveAsync(newFilepath, CollisionOption.ReplaceExisting);

            file.Path.Should().Be(newFile.Path);
            file.Name.Should().Be(newFile.Name);

            // Cleanup
            await file.DeleteAsync();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_MoveAsync_succeeds_and_a_new_name_is_generated_if_specified()
        {
            var filename = Helper.CreateUniqueFileName();
            var newFilename = Helper.CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var newFilepath = Filesystem.CreatePath(folder.Path).Combine(newFilename);

            var sourceFile = await Helper.GenerateFileAsync(folder, filename);
            var existingFile = await Helper.GenerateFileAsync(folder, newFilename);

            var newFile = await sourceFile.MoveAsync(newFilepath, CollisionOption.GenerateUniqueName);

            newFile.Path.Should().NotBe(existingFile.Path);
            newFile.Name.Should().NotBe(existingFile.Name);
            (await newFile.ExistsAsync()).Should().BeTrue();

            // Cleanup
            await existingFile.DeleteAsync();
            await newFile.DeleteAsync();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_a_MoveAsync_throws_an_exception_if_the_destination_file_exists_and_FailIfExists_is_specified()
        {
            var filename = Helper.CreateUniqueFileName();
            var newFilename = Helper.CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var newFilepath = Filesystem.CreatePath(folder.Path).Combine(newFilename);

            var sourceFile = await Helper.GenerateFileAsync(folder, filename);
            var existingFile = await Helper.GenerateFileAsync(folder, newFilename);

            Func<Task> act = () => sourceFile.MoveAsync(newFilepath, CollisionOption.FailIfExists);
            act.ShouldThrow<Exceptions.UnifiedIOException>();

            (await sourceFile.ExistsAsync()).Should().BeTrue("Expecting the source file to be untouched");

            // Cleanup
            await existingFile.DeleteAsync();
            await sourceFile.DeleteAsync();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_DeleteAsync_deletes_an_existing_file()
        {
            var filename = Helper.CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;
            var sourceFile = await Helper.GenerateFileAsync(folder, filename);

            (await sourceFile.ExistsAsync()).Should().BeTrue();

            await sourceFile.DeleteAsync();

            (await sourceFile.ExistsAsync()).Should().BeFalse();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_DeleteAsync_throws_an_exception_if_the_file_does_not_exist()
        {
            var filename = Helper.CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;
            var path = Filesystem.CreatePath(folder.Path);

            var sourceFile = await Filesystem.GetFileFromPathAsync(path.Combine(filename));

            (await sourceFile.ExistsAsync()).Should().BeFalse();

            Func<Task> act = () => sourceFile.DeleteAsync();
            act.ShouldThrow<Exceptions.FileNotFoundException>();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_RenameAsync_succeeds()
        {
            var filename = Helper.CreateUniqueFileName();
            var newFilename = Helper.CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var sourceFile = await Helper.GenerateFileAsync(folder, filename);
            var sourcePath = sourceFile.Path;
            var sourceName = sourceFile.Name;

            var destinationFile = await sourceFile.RenameAsync(newFilename, CollisionOption.FailIfExists);

            destinationFile.Path.Should().NotBe(sourcePath);
            destinationFile.Name.Should().NotBe(sourceName);

            destinationFile.Path.Should().Be(sourceFile.Path);
            destinationFile.Name.Should().Be(sourceFile.Name);

            // Cleanup
            await sourceFile.DeleteAsync();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_RenameAsync_succeeds_and_replaces_the_destination_if_ReplaceExisting_is_specified()
        {
            var filename = Helper.CreateUniqueFileName();
            var newFilename = Helper.CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var file = await Helper.GenerateFileAsync(folder, filename);
            await Helper.GenerateFileAsync(folder, newFilename);

            var newFile = await file.RenameAsync(newFilename, CollisionOption.ReplaceExisting);

            file.Path.Should().Be(newFile.Path);
            file.Name.Should().Be(newFile.Name);

            // Cleanup
            await file.DeleteAsync();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_RenameAsync_succeeds_and_a_new_name_is_generated_if_specified()
        {
            var filename = Helper.CreateUniqueFileName();
            var newFilename = Helper.CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var sourceFile = await Helper.GenerateFileAsync(folder, filename);
            var existingFile = await Helper.GenerateFileAsync(folder, newFilename);

            var newFile = await sourceFile.RenameAsync(newFilename, CollisionOption.GenerateUniqueName);

            newFile.Path.Should().NotBe(existingFile.Path);
            newFile.Name.Should().NotBe(existingFile.Name);
            (await newFile.ExistsAsync()).Should().BeTrue();

            // Cleanup
            await existingFile.DeleteAsync();
            await newFile.DeleteAsync();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_a_RenameAsync_throws_an_exception_if_the_destination_file_exists_and_FailIfExists_is_specified()
        {
            var filename = Helper.CreateUniqueFileName();
            var newFilename = Helper.CreateUniqueFileName();
            var folder = Filesystem.LocalStorage;

            var sourceFile = await Helper.GenerateFileAsync(folder, filename);
            var existingFile = await Helper.GenerateFileAsync(folder, newFilename);

            Func<Task> act = () => sourceFile.RenameAsync(newFilename, CollisionOption.FailIfExists);
            act.ShouldThrow<Exceptions.UnifiedIOException>();

            (await sourceFile.ExistsAsync()).Should().BeTrue("Expecting the source file to be untouched");

            // Cleanup
            await existingFile.DeleteAsync();
            await sourceFile.DeleteAsync();
        }
    }
}
