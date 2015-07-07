using System;
using Windows.Storage;

// ReSharper disable CheckNamespace

namespace UnifiedStorage.WindowsStorage.Extensions
{
    internal static class CollisionOptionExtensions
    {
        public static NameCollisionOption ToNameCollisionOption(this CollisionOption option)
        {
            switch (option)
            {
                case CollisionOption.GenerateUniqueName:
                    return NameCollisionOption.GenerateUniqueName;
                case CollisionOption.ReplaceExisting:
                    return NameCollisionOption.ReplaceExisting;
                case CollisionOption.FailIfExists:
                    return NameCollisionOption.FailIfExists;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }
    }
}
