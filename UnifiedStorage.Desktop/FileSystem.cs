using System.Windows.Forms;
using UnifiedStorage.DotNet;

namespace UnifiedStorage.Desktop
{
    /// <summary>
    /// A <see cref="IFileSystem"/> implementation for desktop applications using the classic .NET System.IO API.
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
                var localAppData = Application.LocalUserAppDataPath;
                return new DotNetDirectory(localAppData);
            }
        }

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user.
        /// </summary>
        public override IDirectory RoamingStorage
        {
            get
            {
                var roamingAppData = Application.UserAppDataPath;
                return new DotNetDirectory(roamingAppData);
            }
        }
    }
}