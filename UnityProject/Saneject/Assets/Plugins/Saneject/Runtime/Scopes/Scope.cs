using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Runtime.Bindings;
using UnityEngine;
using Component = UnityEngine.Component;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Scopes
{
    /// <summary>
    /// Base class for dependency injection scopes in Saneject.
    /// Inherit from this class to configure bindings for your project, using the provided methods to register and resolve <see cref="Component" />s and <see cref="Object" />s.
    /// Each <see cref="Scope" /> acts as a local context for dependency resolution, with support for hierarchical scopes and global bindings.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class Scope : MonoBehaviour
    {
        private readonly HashSet<Binding> bindings = new();

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Scope ParentScope { get; set; }

        protected virtual void OnValidate()
        {
            if ((hideFlags & HideFlags.DontSaveInBuild) == 0)
                hideFlags |= HideFlags.DontSaveInBuild;
        }

        /// <summary>
        /// Resolves all dependencies matching target type from scope.
        /// </summary>
        public IEnumerable<Object> GetAllMatchingDependencies(
            Type interfaceType,
            Type concreteType,
            string injectId,
            bool isCollection,
            Object injectionTarget)
        {
            Binding binding = GetBindingRecursiveUpwards(interfaceType, concreteType, injectId, isCollection, injectionTarget);
            IEnumerable<Object> resolved = binding?.LocateDependencies(injectionTarget);
            return resolved;
        }

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Binding> GetGlobalBindings()
        {
            return bindings.Where(b => b.IsGlobal).ToList();
        }

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract void Configure();

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            bindings.Clear();
            ParentScope = null;
        }

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Binding> GetUnusedBindings()
        {
            return bindings.Where(binding => !binding.IsUsed).ToList();
        }

        /// <summary>
        /// Registers a binding for the concrete type <typeparamref name="T" /> in this scope.
        /// Use this to inject non-interface dependencies such as components, assets, or ScriptableObjects.
        /// Supports collection bindings and global scope registration.
        /// </summary>
        /// <typeparam name="T">The concrete type to bind. Must inherit from <see cref="UnityEngine.Object" />.</typeparam>
        /// <returns>A fluent builder for configuring the binding.</returns>
        protected BindingBuilder<T> Bind<T>() where T : Object
        {
            BindingBuilder<T> builder = new(this, typeof(T));
            builder.OnFinalized += HandleFinalized;
            return builder;

            void HandleFinalized(Binding binding)
            {
                builder.OnFinalized -= HandleFinalized;
                AddBinding(binding);
            }
        }

        /// <summary>
        /// Registers a binding from interface type <typeparamref name="TInterface" /> to concrete type <typeparamref name="TConcrete" />.
        /// Enables injection of interfaces with a specified implementation.
        /// Supports filtering, collection bindings, and global registration.
        /// </summary>
        /// <typeparam name="TInterface">The interface to inject.</typeparam>
        /// <typeparam name="TConcrete">The concrete type to resolve. Must implement <typeparamref name="TInterface" /> and inherit from <see cref="UnityEngine.Object" />.</typeparam>
        /// <returns>A fluent builder for configuring the binding.</returns>
        protected BindingBuilder<TConcrete> Bind<TInterface, TConcrete>()
            where TConcrete : Object, TInterface
            where TInterface : class
        {
            BindingBuilder<TConcrete> builder = new(this, typeof(TInterface), typeof(TConcrete));
            builder.OnFinalized += HandleFinalized;
            return builder;

            void HandleFinalized(Binding binding)
            {
                builder.OnFinalized -= HandleFinalized;
                AddBinding(binding);
            }
        }

        /// <summary>
        /// Add the binding if it doesn't already exist in the tracked bindings. Otherwise throw an error.
        /// </summary>
        /// <param name="binding"></param>
        private void AddBinding(Binding binding)
        {
            if (!bindings.Add(binding))
                Debug.LogError($"Saneject: Scope '{GetType().Name}' has duplicate bindings for '{binding.GetName()}'.");
        }

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        private Binding GetBindingRecursiveUpwards(
            Type interfaceType,
            Type concreteType,
            string injectId,
            bool isCollection,
            Object injectionTarget)
        {
            if (Application.isPlaying)
                throw new Exception("Saneject: Injection is editor-only. Exit Play Mode to inject.");

            // Find all matching bindings for the type
            IEnumerable<Binding> matchingBindings = bindings.Where(binding =>
                !binding.IsGlobal &&
                binding.InterfaceType == interfaceType &&
                (binding.ConcreteType == concreteType || concreteType == null) && // skip concrete check if we have an interface
                binding.Id == injectId &&
                binding.IsCollection == isCollection);

            // If we have an injection target, try to find a binding that passes target filters
            if (injectionTarget != null)
            {
                foreach (Binding binding in matchingBindings.Where(binding => binding.PassesTargetFilters(injectionTarget)))
                    return binding;
            }
            else
            {
                // No injection target provided, return first matching binding (backwards compatibility)
                Binding binding = matchingBindings.FirstOrDefault();

                if (binding != null)
                    return binding;
            }

            return ParentScope
                ? ParentScope.GetBindingRecursiveUpwards(
                    interfaceType,
                    concreteType,
                    injectId,
                    isCollection,
                    injectionTarget)
                : null;
        }
    }
}