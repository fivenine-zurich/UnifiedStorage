using System.Threading;
using System.Threading.Tasks;

namespace UnifiedStorage
{
    public interface IFileSystem
    {
        /// <summary>
        /// A folder representing storage which is local to the current device
        /// </summary>
        IDirectory LocalStorage { get; }
        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user
        /// </summary>
        IDirectory RoamingStorage { get; }

        /// <summary>
        /// Gets the temporary storage directory.
        /// </summary>
        /// <value>
        /// The temporary storage directory.
        /// </value>
        IDirectory TemporaryStorage { get; }

        IPath CreatePath(string path);

        Task<IDirectory> GetDirectoryAsync(string path, CancellationToken cancellationToken = default(CancellationToken));
        
        Task<IFile> GetFileAsync(string path, CancellationToken cancellationToken = default(CancellationToken));
    }
}
