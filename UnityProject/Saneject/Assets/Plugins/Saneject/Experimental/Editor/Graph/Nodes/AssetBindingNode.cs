using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class AssetBindingNode : BindingNode
    {
        public AssetBindingNode(
            AssetBinding binding,
            ScopeNode scopeNode) : base(binding, scopeNode)
        {
            AssetPath = binding.AssetPath;
            AssetLoadType = binding.AssetLoadType;
        }

        public string AssetPath { get; }
        public AssetLoadType AssetLoadType { get; }
    }
}