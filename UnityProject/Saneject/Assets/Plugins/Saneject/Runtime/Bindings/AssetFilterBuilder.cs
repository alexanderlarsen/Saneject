using System;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Fluent builder for applying filters to asset bindings within a <see cref="Scope" />.
    /// Used to refine the selection of <see cref="UnityEngine.Object" /> instances after an asset locator has been configured.
    /// Supports filtering by name, GameObject properties, custom predicates, and injection target type.
    /// Typically returned from asset binding methods such as <c>FromResources()</c> or <c>FromAssetLoad()</c>.
    /// </summary>
    public class AssetFilterBuilder<TAsset> : BaseFilterBuilder<TAsset> where TAsset : Object
    {
        private readonly Binding binding;
        private readonly Scope scope;

        public AssetFilterBuilder(
            Binding binding,
            Scope scope)
        {
            this.binding = binding;
            this.scope = scope;
        }

        #region GAMEOBJECT METHODS

        /// <summary>
        /// Filter to only search for <see cref="GameObject" />s with a specific <see cref="GameObject.tag" />.
        /// </summary>
        public AssetFilterBuilder<TAsset> WhereGameObjectTagIs(string tag)
        {
            if (typeof(GameObject).IsAssignableFrom(typeof(TAsset)))
                binding.AddFilter(o => GetGameObject(o)?.CompareTag(tag) == true);
            else
                Debug.LogError($"Saneject: 'AssetFilterBuilder<T>.{nameof(WhereGameObjectTagIs)}()' can only be used with GameObject types. '{typeof(TAsset)}' is not a GameObject.", scope);

            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="GameObject" />s with a specific <see cref="GameObject.layer" />.
        /// </summary>
        public AssetFilterBuilder<TAsset> WhereGameObjectLayerIs(int layer)
        {
            if (typeof(GameObject).IsAssignableFrom(typeof(TAsset)))
                binding.AddFilter(o => GetGameObject(o)?.layer == layer);
            else
                Debug.LogError($"Saneject: 'AssetFilterBuilder<T>.{nameof(WhereGameObjectLayerIs)}()' can only be used with GameObject types. '{typeof(TAsset)}' is not a GameObject.", scope);

            return this;
        }

        #endregion

        #region SPECIAL METHODS

        /// <summary>
        /// Filter to only search for <see cref="Object" />s with <see cref="Object.name" />s that contain a substring.
        /// </summary>
        public AssetFilterBuilder<TAsset> WhereNameContains(string substring)
        {
            binding.AddFilter(o => o.name.Contains(substring));
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Object" />s with a specific <see cref="Object.name" />.
        /// </summary>
        public AssetFilterBuilder<TAsset> WhereNameIs(string name)
        {
            binding.AddFilter(o => o.name == name);
            return this;
        }

        /// <summary>
        /// Filter using a predicate on <typeparamref name="TAsset" /> for custom search logic.
        /// </summary>
        public AssetFilterBuilder<TAsset> Where(Func<TAsset, bool> predicate)
        {
            binding.AddFilter(o => o is TAsset t && predicate(t));
            return this;
        }

        /// <summary>
        /// Filter to only resolve dependencies using this binding when the injection target is of type <typeparamref name="TTarget" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public AssetFilterBuilder<TAsset> WhereTargetIs<TTarget>()
        {
            binding.AddTargetFilter(obj => obj is TTarget, typeof(TTarget));
            return this;
        }

        #endregion
    }
}