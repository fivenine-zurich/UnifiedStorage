using System;
using System.IO;
using UnifiedStorage.DotNet;

namespace UnifiedStorage.iOS
{
    /// <summary>
    /// A <see cref="IFileSystem"/> implementation for the iOS platform.
    /// </summary>
    public class FileSystem : DotNetFileSystem
    {
        /// <summary>
        /// A folder representing storage which is local to the current device.
        /// </summary>
        public override IDirectory LocalStorage
        {
            get
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var path = Path.Combine(documents, "..", "Library");

                return new DotNetDirectory(path);
            }
        }

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user.
        /// </summary>
        public override IDirectory RoamingStorage
        {
            get { throw new NotSupportedException("Roaming storage is not supported on this platform."); }
        }
    }
}