using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.GraphSystem.Bindings
{
    /// <summary>
    /// Fluent builder for configuring asset bindings in a <see cref="Scope" />.
    /// Allows locating and binding <see cref="UnityEngine.Object" /> assets via direct instance, Resources, or AssetDatabase.
    /// Typically returned from <c>BindAsset&lt;TAsset&gt;()</c> or <c>BindAssets&lt;TAsset&gt;()</c>.
    /// </summary>
    public class NewAssetBindingBuilder<TAsset> where TAsset : Object
    {
        private readonly NewAssetBinding binding;

        public NewAssetBindingBuilder(NewAssetBinding binding)
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
        public NewAssetBindingBuilder<TAsset> ToID(params string[] ids)
        {
            foreach (string id in ids)
                binding.AddIdQualifier(id);

            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is of the given type.
        /// The injection target is the <see cref="Component" /> that owns the field or property
        /// marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        /// <typeparam name="TTarget">The target type this binding applies to.</typeparam>
        public NewAssetBindingBuilder<TAsset> ToTarget<TTarget>()
        {
            binding.AddInjectionTargetTypeQualifier(typeof(TTarget));
            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is one of the specified types.
        /// The injection target is the <see cref="Component" /> that owns the field or property
        /// marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        /// <param name="targetTypes">One or more target <see cref="Type" /> objects to match against.</param>
        public NewAssetBindingBuilder<TAsset> ToTarget(params Type[] targetTypes)
        {
            foreach (Type type in targetTypes)
                binding.AddInjectionTargetTypeQualifier(type);

            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target member (field or property)
        /// has one of the specified names.
        /// </summary>
        /// <param name="memberNames">The field or property names on the injection target that this binding should apply to.</param>
        public NewAssetBindingBuilder<TAsset> ToMember(params string[] memberNames)
        {
            foreach (string memberName in memberNames)
                binding.AddInjectionTargetMemberNameQualifier(memberName);

            return this;
        }

        #endregion

        #region PROJECT FOLDER METHODS

        /// <summary>
        /// Locate the <see cref="Object" /> in a <see cref="Resources" /> folder at the specified path using <see cref="Resources.Load(string)" />.
        /// </summary>
        public NewAssetFilterBuilder<TAsset> FromResources(string path)
        {
            binding.SetAssetPath(path);
            binding.SetAssetLoadType(AssetLoadType.Resources);
            binding.MarkLocatorStrategySpecified();
            return new NewAssetFilterBuilder<TAsset>(binding);
        }

        /// <summary>
        /// Locate all <see cref="Object" />s in a <see cref="Resources" /> folder at the specified path using <see cref="Resources.LoadAll(string, System.Type)" />.
        /// </summary>
        public NewAssetFilterBuilder<TAsset> FromResourcesAll(string path)
        {
            binding.SetAssetPath(path);
            binding.SetAssetLoadType(AssetLoadType.ResourcesAll);
            binding.MarkLocatorStrategySpecified();
            return new NewAssetFilterBuilder<TAsset>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> asset at the specified path using <see cref="UnityEditor.AssetDatabase.LoadAssetAtPath(string, System.Type)" />.
        /// </summary>
        public NewAssetFilterBuilder<TAsset> FromAssetLoad(string assetPath)
        {
            binding.SetAssetPath(assetPath);
            binding.SetAssetLoadType(AssetLoadType.AssetLoad);
            binding.MarkLocatorStrategySpecified();
            return new NewAssetFilterBuilder<TAsset>(binding);
        }

        /// <summary>
        /// Locate all sub-assets of type <see cref="Object" /> in a single asset file
        /// at the specified path using <see cref="UnityEditor.AssetDatabase.LoadAllAssetsAtPath(string)" />.
        /// </summary>
        public NewAssetFilterBuilder<TAsset> FromAssetLoadAll(string assetPath)
        {
            binding.SetAssetPath(assetPath);
            binding.SetAssetLoadType(AssetLoadType.AssetLoadAll);
            binding.MarkLocatorStrategySpecified();
            return new NewAssetFilterBuilder<TAsset>(binding);
        }

        /// <summary>
        /// Locate all <see cref="Object" />s of type <typeparamref name="TAsset" /> in the specified folder
        /// using <see cref="UnityEditor.AssetDatabase.FindAssets(string, string[])" />.
        /// </summary>
        public NewAssetFilterBuilder<TAsset> FromFolder(string folderPath)
        {
            binding.SetAssetPath(folderPath);
            binding.SetAssetLoadType(AssetLoadType.Folder);
            binding.MarkLocatorStrategySpecified();
            return new NewAssetFilterBuilder<TAsset>(binding);
        }

        #endregion

        #region SPECIAL METHODS

        /// <summary>
        /// Bind to the specified <see cref="Object" /> instance.
        /// </summary>
        public NewAssetFilterBuilder<TAsset> FromInstance(TAsset instance)
        {
            binding.SetAssetLoadType(AssetLoadType.Instance);
            binding.ResolveFromInstances(instance);
            binding.MarkLocatorStrategySpecified();
            return new NewAssetFilterBuilder<TAsset>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> using the provided method.
        /// </summary>
        public NewAssetFilterBuilder<TAsset> FromMethod(Func<TAsset> method)
        {
            binding.SetAssetLoadType(AssetLoadType.Instance);
            binding.ResolveFromInstances(method?.Invoke());
            binding.MarkLocatorStrategySpecified();
            return new NewAssetFilterBuilder<TAsset>(binding);
        }

        /// <summary>
        /// Locate multiple <see cref="Object" />s using the provided method.
        /// </summary>
        public NewAssetFilterBuilder<TAsset> FromMethod(Func<IEnumerable<TAsset>> method)
        {
            binding.SetAssetLoadType(AssetLoadType.Instance);
            binding.ResolveFromInstances(method?.Invoke());
            binding.MarkLocatorStrategySpecified();
            return new NewAssetFilterBuilder<TAsset>(binding);
        }

        #endregion
    }
}