using System.ComponentModel;

namespace Plugins.Saneject.Runtime.Bindings.Asset
{
    /// <summary>
    /// Specifies the type of filter applied to an asset binding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum AssetFilterType
    {
        /// <summary> No filter is applied. </summary>
        None,
        /// <summary> A predicate filter is applied. </summary>
        Where
    }
}