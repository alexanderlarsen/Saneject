using System;

namespace Plugins.Saneject.Experimental.GraphSystem.Bindings
{
    public class NewAssetFilterBuilder<TAsset> where TAsset : UnityEngine.Object
    {
        private readonly NewAssetBinding binding;

        public NewAssetFilterBuilder(NewAssetBinding binding)
        {
            this.binding = binding;
        }
        
        #region BASE METHODS

        /// <summary>
        /// Filter using a predicate on <typeparamref name="TAsset" /> for custom search logic.
        /// </summary>
        public NewAssetFilterBuilder<TAsset> Where(Func<TAsset, bool> predicate)
        {
            binding.AddFilter(o => o is TAsset t && predicate(t));
            return this;
        }

        #endregion
    }
}