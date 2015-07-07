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
        protected readonly IFileSystem _filesystem;

        protected FileTests(IFileSystem filesystem)
        {
            _filesystem = filesystem;
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_a_new_file_class_can_be_created_without_the_need_for_the_file_to_exist()
        {
            var filePath = _filesystem.CreatePath(_filesystem.LocalStorage.Path);

            var file = await _filesystem.GetFileAsync(filePath.Combine(Guid.NewGuid() + ".txt"));
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

            var file = await _filesystem.GetFileAsync(filePath.Combine(filename));
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
        public virtual async Task Verify_that_Extension_returns_the_files_extension( string filename, string expectedExtension)
        {
            var filePath = _filesystem.CreatePath(_filesystem.LocalStorage.Path);

            var file = await _filesystem.GetFileAsync(filePath.Combine(filename));
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

            var file = await _filesystem.GetFileAsync(filePath.Combine(Guid.NewGuid() + ".txt"));
            file.Should().NotBeNull();
            var result = await file.ExistsAsync();

            result.Should().BeFalse();
        }
    }
}
