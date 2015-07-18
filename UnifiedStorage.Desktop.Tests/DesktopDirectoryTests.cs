using NUnit.Framework;
using UnifiedStorage.Shared.Tests;

namespace UnifiedStorage.Desktop.Tests
{
    [TestFixture]
    public class DesktopDirectoryTests : DirectoryTests
    {
        public DesktopDirectoryTests()
            : base(new FileSystem())
        {
        }
    }
}