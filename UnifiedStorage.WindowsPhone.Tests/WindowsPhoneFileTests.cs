using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UnifiedStorage.WindowsStorage;

namespace UnifiedStorage.WindowsPhone.Tests
{
    [TestClass]
    public class WindowsPhoneFileTests
    {
        private readonly FileSystem _filesystem;

        public WindowsPhoneFileTests()
        {
            _filesystem = new FileSystem();
        }

        [TestMethod]
        public async Task Verify_that_a_new_file_class_can_be_created_without_the_need_for_the_file_to_exist()
        {
            var path = new WindowsStoragePath(ApplicationData.Current.LocalFolder.Path);

            var file = await _filesystem.GetFileAsync(path.Combine("Test.txt"));
            file.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Verify_that_Name_returns_the_filename_including_its_extension()
        {
            var filePath = _filesystem.CreatePath(_filesystem.LocalStorage.Path);
            var filename = Guid.NewGuid() + ".txt";

            var file = await _filesystem.GetFileAsync(filePath.Combine(filename));
            file.Should().NotBeNull();
            file.Name.Should().Be(filename);
        }

        [DataTestMethod]
        [DataRow("test.txt", ".txt")]
        [DataRow(".gitignore", ".gitignore")]
        public async Task Verify_that_Extension_returns_the_files_extension(string filename, string expectedExtension)
        {
            var filePath = _filesystem.CreatePath(_filesystem.LocalStorage.Path);

            var file = await _filesystem.GetFileAsync(filePath.Combine(filename));
            file.Should().NotBeNull();
            file.Extension.Should().Be(expectedExtension);
        }

        [TestMethod]
        public async Task Verify_that_Exists_returns_false_for_a_nonexistent_file()
        {
            var filePath = _filesystem.CreatePath(_filesystem.LocalStorage.Path);

            var file = await _filesystem.GetFileAsync(filePath.Combine(Guid.NewGuid() + ".txt"));
            file.Should().NotBeNull();
            var result = await file.ExistsAsync();

            result.Should().BeFalse();
        }
    }
}
