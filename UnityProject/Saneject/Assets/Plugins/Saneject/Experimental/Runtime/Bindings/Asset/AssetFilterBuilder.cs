using System;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Asset
{
    public class AssetFilterBuilder<TAsset> where TAsset : UnityEngine.Object
    {
        private readonly AssetBinding binding;

        public AssetFilterBuilder(AssetBinding binding)
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