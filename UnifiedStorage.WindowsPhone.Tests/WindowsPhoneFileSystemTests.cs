﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UnifiedStorage.Shared.Tests;
using UnifiedStorage.WindowsStorage;

namespace UnifiedStorage.WindowsPhone.Tests
{
    [TestClass]
    public class WindowsPhoneFileSystemTests : FileSystemTests
    {
        public WindowsPhoneFileSystemTests()
            : base(new FileSystem())
        {
        }
    }
}