using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;

namespace Plugins.Saneject.Experimental.Editor.Graph.BindingNodes
{
    public class AssetBindingNode : BaseBindingNode
    {
        public AssetBindingNode(AssetBinding binding) : base(binding)
        {
            AssetPath = binding.AssetPath;
            AssetLoadType = binding.AssetLoadType;
        }

        public string AssetPath { get; }
        public AssetLoadType AssetLoadType { get; }
    }
}