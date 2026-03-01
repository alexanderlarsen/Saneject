using System.ComponentModel;
using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;

namespace Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AssetBindingNode : BindingNode
    {
        public AssetBindingNode(
            AssetBinding binding,
            ScopeNode scopeNode) : base(binding, scopeNode)
        {
            Path = binding.AssetPath;
            AssetLoadType = binding.AssetLoadType;
        }

        public string Path { get; }
        public AssetLoadType AssetLoadType { get; }
    }
}