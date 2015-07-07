using System.Windows.Forms;
using UnifiedStorage.DotNet;

namespace UnifiedStorage.Desktop
{
    public class FileSystem : DotNetFileSystem
    {
        public override IDirectory LocalStorage
        {
            get
            {
                var localAppData = Application.LocalUserAppDataPath;
                return new DotNetDirectory(localAppData);
            }
        }

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