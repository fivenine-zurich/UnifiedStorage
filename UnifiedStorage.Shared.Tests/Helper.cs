using System;
using System.IO;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace

namespace UnifiedStorage.Shared.Tests
{
    internal static class Helper
    {
        public static string CreateUniqueFileName()
        {
            return Guid.NewGuid() + ".txt";
        }

        public static async Task<IFile> GenerateFileAsync(IDirectory parentDirectory, string filename)
        {
            const int iterations = 10;
            byte[] data = new byte[1024];

            var random = new Random((int) DateTime.Now.Ticks);
            var file = await parentDirectory.CreateFileAsync(filename, CollisionOption.FailIfExists);

            using (var writer = new StreamWriter(await file.OpenAsync(FileAccessOption.ReadWrite)))
            {
                for (int i = 0; i < iterations; i++)
                {
                    random.NextBytes(data);
                    writer.Write(Convert.ToString(data));
                }

                await writer.FlushAsync();
            }

            return file;
        }
    }
}