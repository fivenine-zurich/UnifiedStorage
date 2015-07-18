﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UnifiedStorage.Shared.Tests;
using UnifiedStorage.WindowsStorage;

namespace UnifiedStorage.WindowsPhone.Tests
{
    [TestClass]
    public class WindowsPhoneFileTests : FileTests
    {
        public WindowsPhoneFileTests()
            : base(new FileSystem())
        {
        }
    }
}
