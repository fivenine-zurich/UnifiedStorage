using System;
using System.IO;
using System.Linq;

namespace UnifiedStorage.DotNet
{
    internal class DotNetPath : IPath
    {
        private readonly string _path;

        public DotNetPath( string path)
        {
            _path = path;
        }

        public bool IsRoot
        {
            get { return Path.GetPathRoot(_path).Equals(_path, StringComparison.InvariantCultureIgnoreCase); }
        }

        public string Combine(params string[] fragments)
        {
            return fragments.Aggregate(_path, Path.Combine);
        }

        public override string ToString()
        {
            return _path;
        }
    }
}
