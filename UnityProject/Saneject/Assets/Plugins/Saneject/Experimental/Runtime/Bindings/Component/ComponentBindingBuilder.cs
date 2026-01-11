using System;
using Plugins.Saneject.Experimental.Runtime.Proxy;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    /// <summary>
    /// Fluent builder for configuring component bindings within a <see cref="Scope" />.
    /// Allows specifying how to locate a <see cref="Component" /> instance (or collection) from the scene hierarchy or injection context.
    /// Supports locating from the scope, injection target, custom transforms, scene-wide queries, or explicit instances.
    /// Typically returned from binding methods like <c>BindComponent&lt;TComponent&gt;()</c> or <c>BindMultipleComponents&lt;TComponent&gt;()</c>.
    /// </summary>
    public class ComponentBindingBuilder<TComponent> : BaseComponentBindingBuilder<TComponent> where TComponent : class
    {
        public ComponentBindingBuilder(ComponentBinding binding) : base(binding)
        {
        }

        /// <summary>
        /// Creates or locates a <see cref="RuntimeProxy{TConcrete}" /> for <c>TComponent</c>, acting as a weak reference that resolves to a concrete <see cref="Component" /> at runtime. This enables serializing references across boundaries Unity normally can’t (e.g. between scenes or prefabs). Uses the first existing proxy project-wide or generates a new one, including a stub script and proxy ScriptableObject asset if missing.
        /// </summary>
        public void FromRuntimeProxy()
        {
            binding.ResolveFromRuntimeProxy = true;
            binding.LocatorStrategySpecified = true;
        }

        #region QUALIFIER METHODS

        /// <summary>
        /// Qualifies this binding with an ID.
        /// Only injection targets annotated with <see cref="Attributes.InjectAttribute" />
        /// that specify the same ID will resolve using this binding.
        /// </summary>
        /// <param name="ids">The identifiers to match against injection targets.</param>
        public ComponentBindingBuilder<TComponent> ToID(params string[] ids)
        {
            binding.IdQualifiers.AddRange(ids);
            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is of the given type.
        /// The injection target is the <see cref="UnityEngine.Component" /> that owns the field or property
        /// marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <typeparam name="TTarget">The target type this binding applies to.</typeparam>
        public ComponentBindingBuilder<TComponent> ToTarget<TTarget>()
        {
            binding.TargetTypeQualifiers.Add(typeof(TTarget));
            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is one of the specified types.
        /// The injection target is the <see cref="UnityEngine.Component" /> that owns the field or property
        /// marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <param name="targetTypes">One or more target <see cref="Type" /> objects to match against.</param>
        public ComponentBindingBuilder<TComponent> ToTarget(params Type[] targetTypes)
        {
            binding.TargetTypeQualifiers.AddRange(targetTypes);
            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target member (field or property) has one of the specified names.
        /// </summary>
        /// <param name="memberNames">The field or property names on the injection target that this binding should apply to.</param>
        public ComponentBindingBuilder<TComponent> ToMember(params string[] memberNames)
        {
            binding.MemberNameQualifiers.AddRange(memberNames);
            return this;
        }

        #endregion
    }
}