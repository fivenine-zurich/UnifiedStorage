using NUnit.Framework;
using UnifiedStorage.Shared.Tests;

namespace UnifiedStorage.Desktop.Tests
{
    [TestFixture]
    public class DesktopFileTests : FileTests
    {
        public DesktopFileTests() 
            : base(new FileSystem())
        {
        }
    }
}
