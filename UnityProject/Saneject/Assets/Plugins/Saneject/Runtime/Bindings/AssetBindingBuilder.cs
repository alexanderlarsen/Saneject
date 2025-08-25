using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Fluent builder for configuring asset bindings in a <see cref="Scope" />.
    /// Allows locating and binding <see cref="UnityEngine.Object" /> assets via direct instance, Resources, or AssetDatabase.
    /// Typically returned from <c>BindAsset&lt;TAsset&gt;()</c> or <c>BindAssets&lt;TAsset&gt;()</c>.
    /// </summary>
    public class AssetBindingBuilder<TAsset> where TAsset : Object
    {
        private readonly Binding binding;
        private readonly Scope scope;

        public AssetBindingBuilder(
            Binding binding,
            Scope scope)
        {
            this.binding = binding;
            this.scope = scope;
        }

        #region CONFIG METHODS

        /// <summary>
        /// Set the binding ID for resolution of fields/properties with the same ID in their <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public AssetBindingBuilder<TAsset> WithId(string id)
        {
            binding.SetId(id);
            return this;
        }

        #endregion

        #region PROJECT FOLDER METHODS

        /// <summary>
        /// Locate the <see cref="Object" /> in a <see cref="Resources" /> folder at the specified path using <see cref="Resources.Load(string)" />.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromResources(string path)
        {
            binding.SetLocator(_ => Resources.Load<TAsset>(path).WrapInEnumerable());
            return new AssetFilterBuilder<TAsset>(binding, scope);
        }

        /// <summary>
        /// Locate all <see cref="Object" />s in a <see cref="Resources" /> folder at the specified path using <see cref="Resources.LoadAll(string, System.Type)" />.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromResourcesAll(string path)
        {
            binding.SetLocator(_ => Resources.LoadAll<TAsset>(path));
            return new AssetFilterBuilder<TAsset>(binding, scope);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> asset at the specified path using <see cref="UnityEditor.AssetDatabase.LoadAssetAtPath(string, System.Type)" />.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromAssetLoad(string assetPath)
        {
#if UNITY_EDITOR
            binding.SetLocator(_ => AssetDatabase.LoadAssetAtPath<TAsset>(assetPath).WrapInEnumerable());
            return new AssetFilterBuilder<TAsset>(binding, scope);
#else
            return null;
#endif
        }

        /// <summary>
        /// Locate all sub-assets of type <see cref="Object" /> in a single asset file
        /// at the specified path using <see cref="UnityEditor.AssetDatabase.LoadAllAssetsAtPath(string)" />.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromAssetLoadAll(string assetPath)
        {
#if UNITY_EDITOR
            binding.SetLocator(_ => AssetDatabase.LoadAllAssetsAtPath(assetPath).Where(asset => asset.GetType() == typeof(TAsset)));
            return new AssetFilterBuilder<TAsset>(binding, scope);
#else
            return null;
#endif
        }

        /// <summary>
        /// Locate all <see cref="Object" />s of type <typeparamref name="TAsset" /> in the specified folder
        /// using <see cref="UnityEditor.AssetDatabase.FindAssets(string, string[])" />.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromFolder(string folderPath)
        {
#if UNITY_EDITOR
            folderPath = folderPath.TrimEnd('/');

            binding.SetLocator(_ =>
                AssetDatabase.FindAssets($"t:{typeof(TAsset).Name}", new[] { folderPath })
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<TAsset>)
                    .Where(x => x));

            return new AssetFilterBuilder<TAsset>(binding, scope);
#else
    return null;
#endif
        }

        #endregion

        #region SPECIAL METHODS

        /// <summary>
        /// Bind to the specified <see cref="Object" /> instance.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromInstance(TAsset instance)
        {
            binding.SetLocator(_ => instance.WrapInEnumerable());
            return new AssetFilterBuilder<TAsset>(binding, scope);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> using the provided method.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromMethod(Func<TAsset> method)
        {
            binding.SetLocator(_ => method?.Invoke().WrapInEnumerable());
            return new AssetFilterBuilder<TAsset>(binding, scope);
        }

        /// <summary>
        /// Locate multiple <see cref="Object" />s using the provided method.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromMethod(Func<IEnumerable<TAsset>> method)
        {
            binding.SetLocator(_ => method?.Invoke());
            return new AssetFilterBuilder<TAsset>(binding, scope);
        }

        #endregion
    }
}