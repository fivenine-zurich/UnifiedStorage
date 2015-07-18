using NUnit.Framework;
using UnifiedStorage.Shared.Tests;

namespace UnifiedStorage.Desktop.Tests
{
    [TestFixture]
    public class DesktopFileSystemTests : FileSystemTests
    {
        public DesktopFileSystemTests()
            : base(new FileSystem())
        {
        }
    }
}