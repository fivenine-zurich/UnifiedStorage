using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UnifiedStorage.Shared.Tests;
using UnifiedStorage.WindowsStorage;

namespace UnifiedStorage.WindowsStore.Tests
{
    [TestClass]
    public class WindowsStoreDirectoryTests : DirectoryTests
    {
        public WindowsStoreDirectoryTests()
            : base(new FileSystem())
        {
        }
    }
}