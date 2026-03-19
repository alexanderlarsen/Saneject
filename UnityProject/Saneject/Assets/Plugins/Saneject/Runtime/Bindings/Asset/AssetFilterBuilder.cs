using System;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Bindings.Asset
{
    /// <summary>
    /// Builder for configuring filters on asset bindings.
    /// </summary>
    /// <typeparam name="TAsset">The type of the asset being filtered.</typeparam>
    public class AssetFilterBuilder<TAsset> where TAsset : Object
    {
        private readonly AssetBinding binding;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetFilterBuilder{TAsset}"/> class.
        /// </summary>
        /// <param name="binding">The asset binding to configure.</param>
        public AssetFilterBuilder(AssetBinding binding)
        {
            this.binding = binding;
        }

        #region BASE METHODS

        /// <summary>
        /// Filter using a predicate on <typeparamref name="TAsset" /> for custom search logic.
        /// </summary>
        /// <param name="predicate">The predicate function.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        public AssetFilterBuilder<TAsset> Where(Func<TAsset, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new AssetFilter
                (
                    AssetFilterType.Where,
                    o => o is TAsset t && predicate(t)
                ));

            return this;
        }

        #endregion
    }
}