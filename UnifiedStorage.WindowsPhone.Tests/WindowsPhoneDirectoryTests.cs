using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UnifiedStorage.WindowsStorage;

namespace UnifiedStorage.WindowsPhone.Tests
{
    [TestClass]
    public class WindowsPhoneDirectoryTests
    {
        protected readonly FileSystem Filesystem = new FileSystem();

        [TestMethod]
        public virtual async Task Verify_that_a_new_directory_class_can_be_created_without_the_need_for_the_directory_to_exist()
        {
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);

            var directory = await Filesystem.GetDirectoryFromPathAsync(filePath.Combine(Guid.NewGuid().ToString()));
            directory.Should().NotBeNull();
        }

        [TestMethod]
        public virtual async Task Verify_that_ExistsAsync_returns_false_for_a_nonexistent_directory()
        {
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);

            var directory = await Filesystem.GetDirectoryFromPathAsync(filePath.Combine(Guid.NewGuid().ToString()));
            directory.Should().NotBeNull();
            var result = await directory.ExistsAsync();

            result.Should().BeFalse();
        }

        [TestMethod]
        public virtual async Task Verify_that_ExistsAsync_returns_true_for_an_existing_directory()
        {
            var folder = Filesystem.LocalStorage;
            (await folder.ExistsAsync()).Should().BeTrue();
        }

        [TestMethod]
        public virtual async Task Verify_that_CreateDirectoryAsync_succeeds_if_the_directory_does_not_exist()
        {
            var folder = Filesystem.LocalStorage;

            var directory = await folder.CreateDirectoryAsync(Guid.NewGuid().ToString(), CollisionOption.FailIfExists);

            (await directory.ExistsAsync()).Should().BeTrue();

            // Cleanup
            await directory.DeleteAsync();
        }
    }
}
