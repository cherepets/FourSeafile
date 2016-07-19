using Windows.Storage;

namespace FourSeafile.Component.Extensions
{
    public static class NameCollisionOptionExt
    {
        public static CreationCollisionOption ToCreationOption(this NameCollisionOption option)
        {
            switch (option)
            {
                case NameCollisionOption.GenerateUniqueName:
                    return CreationCollisionOption.GenerateUniqueName;
                case NameCollisionOption.ReplaceExisting:
                    return CreationCollisionOption.ReplaceExisting;
                case NameCollisionOption.FailIfExists:
                    return CreationCollisionOption.FailIfExists;
            }
            return default(CreationCollisionOption);
        }
    }
}
