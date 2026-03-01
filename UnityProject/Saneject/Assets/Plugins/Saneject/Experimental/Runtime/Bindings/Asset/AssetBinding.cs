using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Asset
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AssetBinding : Binding
    {
        public string AssetPath { get; set; }
        public AssetLoadType AssetLoadType { get; set; }
    }
}