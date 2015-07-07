using System;
using System.Linq;
using System.Text;

// ReSharper disable CheckNamespace
namespace UnifiedStorage.WindowsStorage
{
    internal class WindowsStoragePath : IPath
    {
        private readonly string _path;

        public static string GetFilename(string path)
        {
            return path.Split('\\').LastOrDefault();
        }

        public static string GetExtension(string path)
        {
            var lastDot = path.LastIndexOf('.');
            if (lastDot < 0)
            {
                return string.Empty;
            }

            return path.Substring(lastDot);
        }

        public static string GetDirectoryFromFilePath(string path)
        {
            var name = GetFilename(path);
            return path.Substring(0, path.Length - name.Length);
        }

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
            var sb = new StringBuilder(_path.TrimEnd('\\'));
            foreach (var fragment in fragments)
            {
                sb.AppendFormat(@"\{0}", fragment);
            }

            return sb.ToString();
        }
    }
}