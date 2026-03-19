using System;
using Plugins.Saneject.Runtime.Bindings.RuntimeProxy;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;

namespace Plugins.Saneject.Runtime.Bindings.Component
{
    /// <summary>
    /// Fluent builder for configuring component bindings within a <see cref="Scope" />.
    /// Allows specifying how to locate <see cref="Component" /> instances from the scene- or prefab hierarchy.
    /// Supports locating from the scope, injection target, custom transforms, scene-wide queries, or explicit instances.
    /// Returned from binding methods like <c>BindComponent&lt;TComponent&gt;()</c> or <c>BindMultipleComponents&lt;TComponent&gt;()</c>.
    /// </summary>
    public class ComponentBindingBuilder<TComponent> : BaseComponentBindingBuilder<TComponent> where TComponent : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBindingBuilder{TComponent}"/> class.
        /// </summary>
        /// <param name="binding">The <see cref="ComponentBinding"/> to configure.</param>
        public ComponentBindingBuilder(ComponentBinding binding) : base(binding)
        {
        }

        /// <summary>
        /// Configures this binding to use a <see cref="RuntimeProxy{TComponent}"/> intermediary.
        /// <remarks>
        /// Runtime proxies act as serializable asset placeholders for real component instances, allowing references across scene or prefab boundaries. The proxy asset is automatically generated or reused during injection.
        /// The proxy is automatically swapped with the real instance at runtime during initialization according to the resolution strategy specified by this method chain.
        /// The default resolution strategy is <see cref="RuntimeProxyResolveMethod.FromGlobalScope"/>, if not otherwise specified.
        /// </remarks>
        /// </summary>
        /// <returns>A <see cref="RuntimeProxyBindingBuilder"/> to configure the runtime proxy resolution strategy.</returns>
        public RuntimeProxyBindingBuilder FromRuntimeProxy()
        {
            binding.LocatorStrategySpecified = true;

            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromGlobalScope,
                instanceMode: RuntimeProxyInstanceMode.Singleton,
                prefab: null,
                dontDestroyOnLoad: false
            );

            return new RuntimeProxyBindingBuilder(binding);
        }

        #region QUALIFIER METHODS

        /// <summary>
        /// Qualifies this binding with one or more IDs.
        /// Only injection sites (fields/properties/methods) annotated with an <see cref="Attributes.InjectAttribute" />
        /// that specify the same ID will resolve using this binding.
        /// </summary>
        /// <param name="ids">The identifiers to match against injection sites.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        public ComponentBindingBuilder<TComponent> ToID(params string[] ids)
        {
            binding.IdQualifiers.AddRange(ids);
            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is of the given type.
        /// The injection target is the <see cref="UnityEngine.Component" /> that owns the field, property or method
        /// marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <typeparam name="TTarget">The target type this binding applies to.</typeparam>
        /// <returns>The builder instance for fluent chaining.</returns>
        public ComponentBindingBuilder<TComponent> ToTarget<TTarget>()
        {
            binding.TargetTypeQualifiers.Add(typeof(TTarget));
            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is one of the specified types.
        /// The injection target is the <see cref="UnityEngine.Component" /> that owns the field, property or method
        /// marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <param name="targetTypes">One or more target <see cref="Type" /> objects to match against.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        public ComponentBindingBuilder<TComponent> ToTarget(params Type[] targetTypes)
        {
            binding.TargetTypeQualifiers.AddRange(targetTypes);
            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target member (field or property) has one of the specified names.
        /// </summary>
        /// <param name="memberNames">The field or property names on the injection target that this binding should apply to.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        public ComponentBindingBuilder<TComponent> ToMember(params string[] memberNames)
        {
            binding.MemberNameQualifiers.AddRange(memberNames);
            return this;
        }

        #endregion
    }
}