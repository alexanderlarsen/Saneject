namespace Plugins.Saneject.Experimental.GraphSystem.Bindings
{
    public class NewAssetBinding : NewBinding
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