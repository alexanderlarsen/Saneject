namespace Plugins.Saneject.Experimental.Runtime.Bindings.Asset
{
    public class AssetBinding : BaseBinding
    {
        public string AssetPath { get; private set; }
        public AssetLoadType AssetLoadType { get; private set; }

        public void SetAssetPath(string path)
        {
            AssetPath = path;
        }

        public void SetAssetLoadType(AssetLoadType assetLoadType)
        {
            AssetLoadType = assetLoadType;
        }
    }
}