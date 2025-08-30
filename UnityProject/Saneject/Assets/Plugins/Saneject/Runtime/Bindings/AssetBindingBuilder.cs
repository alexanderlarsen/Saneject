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

        public AssetBindingBuilder(Binding binding)
        {
            this.binding = binding;
        }

        #region QUALIFIER METHODS

        /// <summary>
        /// Qualifies this binding with an ID.
        /// Only injection targets annotated with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />
        /// that specify the same ID will resolve using this binding.
        /// </summary>
        /// <param name="ids">The identifiers to match against injection targets.</param>
        public AssetBindingBuilder<TAsset> ToID(params string[] ids)
        {
            foreach (string id in ids)
                binding.AddIdQualifier(fieldId => fieldId == id, id);

            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is of the given type.
        /// The injection target is the <see cref="Component" /> that owns the field or property
        /// marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        /// <typeparam name="TTarget">The target type this binding applies to.</typeparam>
        public AssetBindingBuilder<TAsset> ToTarget<TTarget>()
        {
            binding.AddInjectionTargetQualifier(obj => obj is TTarget, typeof(TTarget));
            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is one of the specified types.
        /// The injection target is the <see cref="Component" /> that owns the field or property
        /// marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        /// <param name="targetTypes">One or more target <see cref="Type" /> objects to match against.</param>
        public AssetBindingBuilder<TAsset> ToTarget(params Type[] targetTypes)
        {
            foreach (Type t in targetTypes)
                binding.AddInjectionTargetQualifier(obj => t.IsInstanceOfType(obj), t);

            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target member (field or property)
        /// has one of the specified names.
        /// </summary>
        /// <param name="memberNames">The field or property names on the injection target that this binding should apply to.</param>
        public AssetBindingBuilder<TAsset> ToMember(params string[] memberNames)
        {
            foreach (string memberName in memberNames)
                binding.AddInjectionTargetMemberQualifier(name => name == memberName, memberName);

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
            return new AssetFilterBuilder<TAsset>(binding);
        }

        /// <summary>
        /// Locate all <see cref="Object" />s in a <see cref="Resources" /> folder at the specified path using <see cref="Resources.LoadAll(string, System.Type)" />.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromResourcesAll(string path)
        {
            binding.SetLocator(_ => Resources.LoadAll<TAsset>(path));
            return new AssetFilterBuilder<TAsset>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> asset at the specified path using <see cref="UnityEditor.AssetDatabase.LoadAssetAtPath(string, System.Type)" />.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromAssetLoad(string assetPath)
        {
#if UNITY_EDITOR
            binding.SetLocator(_ => AssetDatabase.LoadAssetAtPath<TAsset>(assetPath).WrapInEnumerable());
            return new AssetFilterBuilder<TAsset>(binding);
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
            return new AssetFilterBuilder<TAsset>(binding);
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

            return new AssetFilterBuilder<TAsset>(binding);
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
            return new AssetFilterBuilder<TAsset>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> using the provided method.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromMethod(Func<TAsset> method)
        {
            binding.SetLocator(_ => method?.Invoke().WrapInEnumerable());
            return new AssetFilterBuilder<TAsset>(binding);
        }

        /// <summary>
        /// Locate multiple <see cref="Object" />s using the provided method.
        /// </summary>
        public AssetFilterBuilder<TAsset> FromMethod(Func<IEnumerable<TAsset>> method)
        {
            binding.SetLocator(_ => method?.Invoke());
            return new AssetFilterBuilder<TAsset>(binding);
        }

        #endregion
    }
}