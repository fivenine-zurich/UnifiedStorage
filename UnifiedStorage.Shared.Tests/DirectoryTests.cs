using System;
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
    public abstract class DirectoryTests
    {
        protected readonly IFileSystem Filesystem;

        protected DirectoryTests(IFileSystem filesystem)
        {
            Filesystem = filesystem;
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_a_new_directory_class_can_be_created_without_the_need_for_the_directory_to_exist()
        {
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);

            var directory = await Filesystem.GetDirectoryFromPathAsync(filePath.Combine(Guid.NewGuid().ToString()));
            directory.Should().NotBeNull();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_ExistsAsync_returns_false_for_a_nonexistent_directory()
        {
            var filePath = Filesystem.CreatePath(Filesystem.LocalStorage.Path);

            var directory = await Filesystem.GetDirectoryFromPathAsync(filePath.Combine(Guid.NewGuid().ToString()));
            directory.Should().NotBeNull();
            var result = await directory.ExistsAsync();

            result.Should().BeFalse();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
        public virtual async Task Verify_that_ExistsAsync_returns_true_for_an_existing_directory()
        {
            var folder = Filesystem.LocalStorage;
            (await folder.ExistsAsync()).Should().BeTrue();
        }

#if MSTEST
        [TestMethod]
#else
        [Test]
#endif
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
