using System;
using Plugins.Saneject.Legacy.Runtime.Scopes;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Legacy.Runtime.Bindings
{
    /// <summary>
    /// Fluent builder for applying filters to asset bindings within a <see cref="Scope" />.
    /// Used to refine the selection of <see cref="UnityEngine.Object" /> instances after an asset locator has been configured.
    /// Supports filtering by name, GameObject properties, custom predicates, and injection target type.
    /// Typically returned from asset binding methods such as <c>FromResources()</c> or <c>FromAssetLoad()</c>.
    /// </summary>
    public class AssetFilterBuilder<TAsset> : BaseFilterBuilder where TAsset : Object
    {
        private readonly Binding binding;

        public AssetFilterBuilder(
            Binding binding)
        {
            this.binding = binding;
        }

        #region BASE METHODS

        /// <summary>
        /// Filter using a predicate on <typeparamref name="TAsset" /> for custom search logic.
        /// </summary>
        public AssetFilterBuilder<TAsset> Where(Func<TAsset, bool> predicate)
        {
            binding.AddFilter(o => o is TAsset t && predicate(t));
            return this;
        }

        #endregion
    }
}