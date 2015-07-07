using System;
using System.IO;
using System.Linq;
using System.Text;

// ReSharper disable CheckNamespace
namespace UnifiedStorage.WindowsStorage
{
    internal class WindowsStoragePath : IPath
    {
        private readonly string _path;

        public WindowsStoragePath(string path)
        {
            _path = path;
        }

        public bool IsRoot
        {
            get
            {
                return _path.Equals(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        public string Combine(params string[] fragments)
        {
            return fragments.Aggregate(_path, (s, s1) => Path.Combine(s, s1));
        }
    }
}