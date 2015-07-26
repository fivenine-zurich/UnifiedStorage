using System;
using Windows.Storage;

// ReSharper disable UseNameofExpression
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
                    throw new ArgumentOutOfRangeException("option", option, null);
            }
        }

        public static CreationCollisionOption ToCreationCollisionOption(this CollisionOption option)
        {
            switch (option)
            {
                case CollisionOption.GenerateUniqueName:
                    return CreationCollisionOption.GenerateUniqueName;
                case CollisionOption.ReplaceExisting:
                    return CreationCollisionOption.ReplaceExisting;
                case CollisionOption.FailIfExists:
                    return CreationCollisionOption.FailIfExists;
                case CollisionOption.OpenIfExists:
                    return CreationCollisionOption.OpenIfExists;
                default:
                    throw new ArgumentOutOfRangeException("option", option, null);
            }
        }
    }
}
