using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Asset
{
    /// <summary>
    /// Represents a binding to a Unity asset.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AssetBinding : Binding
    {
        /// <summary>
        /// The path to the asset in the project.
        /// </summary>
        public string AssetPath { get; set; }

        /// <summary>
        /// The <see cref="AssetLoadType"/> used to load the asset.
        /// </summary>
        public AssetLoadType AssetLoadType { get; set; }
    }
}