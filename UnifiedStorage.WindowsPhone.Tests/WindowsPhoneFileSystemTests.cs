using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UnifiedStorage.WindowsStorage;

namespace UnifiedStorage.WindowsPhone.Tests
{
    [TestClass]
    public class WindowsPhoneFileSystemTests
    {
        protected readonly FileSystem FileSystem = new FileSystem();

        [TestMethod]
        public async Task Verify_that_LocalStorage_returns_an_existing_directory()
        {
            var directory = FileSystem.LocalStorage;

            directory.Should().NotBeNull();
            (await directory.ExistsAsync()).Should().BeTrue();
        }

        [TestMethod]
        public async Task Verify_that_RoamingStorage_returns_an_existing_directory()
        {
            var directory = FileSystem.RoamingStorage;

            directory.Should().NotBeNull();
            (await directory.ExistsAsync()).Should().BeTrue();
        }

        [TestMethod]
        public async Task Verify_that_TemporaryStorage_returns_an_existing_directory()
        {
            var directory = FileSystem.TemporaryStorage;

            directory.Should().NotBeNull();
            (await directory.ExistsAsync()).Should().BeTrue();
        }
    }
}