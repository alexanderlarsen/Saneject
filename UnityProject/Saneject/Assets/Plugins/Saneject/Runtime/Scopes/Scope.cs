using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Runtime.Bindings;
using UnityEngine;
using Component = UnityEngine.Component;
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
        #region INTERNAL METHODS/FIELDS

        [EditorBrowsable(EditorBrowsableState.Never)]
        private readonly HashSet<Binding> validBindings = new();

        [EditorBrowsable(EditorBrowsableState.Never)]
        private readonly List<Binding> unvalidatedBindings = new();

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Scope ParentScope { get; set; }

        /// <summary>
        /// Resolves all dependencies matching target type from scope.
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
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
            return validBindings.Where(b => b.IsGlobal).ToList();
        }

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            ParentScope = null;
            validBindings.Clear();
            unvalidatedBindings.Clear();
        }

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Binding> GetUnusedBindings()
        {
            return validBindings.Where(binding => !binding.IsUsed).ToList();
        }

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ValidateBindings()
        {
            foreach (Binding binding in unvalidatedBindings)
            {
                if (!binding.IsValid())
                    continue;

                if (!validBindings.Add(binding))
                    Debug.LogError($"Saneject: Scope '{GetType().Name}' has duplicate bindings for '{binding.GetName()}'.");
            }
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
            IEnumerable<Binding> matchingBindings = validBindings.Where(binding =>
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

        #endregion

        #region USER-FACING METHODS

        /// <summary>
        /// Sets <see cref="HideFlags" /> to <see cref="HideFlags.DontSaveInBuild" />. If you override this method, call <c>base.OnValidate()</c> to strip <see cref="Scope" /> from build.
        /// </summary>
        protected virtual void OnValidate()
        {
            if ((hideFlags & HideFlags.DontSaveInBuild) == 0)
                hideFlags |= HideFlags.DontSaveInBuild;
        }

        /// <summary>
        /// Set up your bindings in this method.
        /// </summary>
        public abstract void ConfigureBindings();

        /// <summary>
        /// Registers a binding for a single <typeparamref name="TComponent" /> component or interface type in this scope.
        /// Use this for concrete <see cref="Component" /> or interface you wish to inject directly.
        /// </summary>
        /// <typeparam name="TComponent">The concrete component type to bind.</typeparam>
        /// <returns>A fluent builder for configuring the component binding.</returns>
        protected ComponentBindingBuilder<TComponent> BindComponent<TComponent>() where TComponent : class
        {
            Binding binding = typeof(TComponent).IsInterface
                ? new Binding(typeof(TComponent), null, this)
                : new Binding(null, typeof(TComponent), this);

            binding.MarkComponentBinding();
            unvalidatedBindings.Add(binding);
            return new ComponentBindingBuilder<TComponent>(binding, this);
        }

        /// <summary>
        /// Registers a collection binding for <typeparamref name="TComponent" /> component type in this scope.
        /// Use this to inject all matching components as a collection.
        /// Shorthand for <see cref="BindMultipleComponents{TConcrete}" />.
        /// </summary>
        /// <typeparam name="TComponent">The concrete component type to bind as a collection.</typeparam>
        /// <returns>A fluent builder for configuring the component collection binding.</returns>
        protected ComponentBindingBuilder<TComponent> BindComponents<TComponent>() where TComponent : class
        {
            return BindMultipleComponents<TComponent>();
        }

        /// <summary>
        /// Registers a collection binding for <typeparamref name="TComponent" /> component type in this scope.
        /// Use this to inject all matching components or interfaces as a collection.
        /// </summary>
        /// <typeparam name="TComponent">The concrete component type to bind as a collection.</typeparam>
        /// <returns>A fluent builder for configuring the component collection binding.</returns>
        protected ComponentBindingBuilder<TComponent> BindMultipleComponents<TComponent>() where TComponent : class
        {
            Binding binding = typeof(TComponent).IsInterface
                ? new Binding(typeof(TComponent), null, this)
                : new Binding(null, typeof(TComponent), this);

            binding.MarkComponentBinding();
            binding.MarkCollectionBinding();
            unvalidatedBindings.Add(binding);
            return new ComponentBindingBuilder<TComponent>(binding, this);
        }

        /// <summary>
        /// Registers a binding from interface type <typeparamref name="TInterface" /> to concrete component type <typeparamref name="TConcrete" />.
        /// Use this to inject components via an interface abstraction.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to inject.</typeparam>
        /// <typeparam name="TConcrete">The concrete component type that implements the interface.</typeparam>
        /// <returns>A fluent builder for configuring the component binding.</returns>
        protected ComponentBindingBuilder<TConcrete> BindComponent<TInterface, TConcrete>()
            where TConcrete : Component, TInterface
            where TInterface : class
        {
            Binding binding = new(typeof(TInterface), typeof(TConcrete), this);
            binding.MarkComponentBinding();
            unvalidatedBindings.Add(binding);
            return new ComponentBindingBuilder<TConcrete>(binding, this);
        }

        /// <summary>
        /// Registers a collection binding from interface type <typeparamref name="TInterface" /> to concrete component type <typeparamref name="TConcrete" />.
        /// Use this to inject multiple components implementing the interface as a collection.
        /// Shorthand for <see cref="BindMultipleComponents{TInterface, TConcrete}" />.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to inject.</typeparam>
        /// <typeparam name="TConcrete">The concrete component type that implements the interface.</typeparam>
        /// <returns>A fluent builder for configuring the component collection binding.</returns>
        protected ComponentBindingBuilder<TConcrete> BindComponents<TInterface, TConcrete>()
            where TConcrete : Component, TInterface
            where TInterface : class
        {
            return BindMultipleComponents<TInterface, TConcrete>();
        }

        /// <summary>
        /// Registers a collection binding from interface type <typeparamref name="TInterface" /> to concrete component type <typeparamref name="TConcrete" />.
        /// Use this to inject multiple components implementing the interface as a collection.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to inject.</typeparam>
        /// <typeparam name="TConcrete">The concrete component type that implements the interface.</typeparam>
        /// <returns>A fluent builder for configuring the component collection binding.</returns>
        protected ComponentBindingBuilder<TConcrete> BindMultipleComponents<TInterface, TConcrete>()
            where TConcrete : Component, TInterface
            where TInterface : class
        {
            Binding binding = new(typeof(TInterface), typeof(TConcrete), this);
            binding.MarkComponentBinding();
            binding.MarkCollectionBinding();
            unvalidatedBindings.Add(binding);
            return new ComponentBindingBuilder<TConcrete>(binding, this);
        }

        /// <summary>
        /// Registers a binding for a single asset of type <typeparamref name="TConcrete" />.
        /// Use this to inject a specific asset (e.g. <see cref="Material" />, <see cref="Texture" />, etc.) by reference.
        /// </summary>
        /// <typeparam name="TConcrete">The UnityEngine.Object-derived asset type to bind.</typeparam>
        /// <returns>A fluent builder for configuring the asset binding.</returns>
        protected AssetBindingBuilder<TConcrete> BindAsset<TConcrete>() where TConcrete : Object
        {
            Binding binding = new(null, typeof(TConcrete), this);
            binding.MarkAssetBinding();
            unvalidatedBindings.Add(binding);
            return new AssetBindingBuilder<TConcrete>(binding, this);
        }

        /// <summary>
        /// Registers a collection binding for all assets of type <typeparamref name="TConcrete" />.
        /// Use this to inject multiple assets (e.g. from a folder or group) as a collection.
        /// Shorthand for <see cref="BindMultipleAssets{TConcrete}" />.
        /// </summary>
        /// <typeparam name="TConcrete">The UnityEngine.Object-derived asset type to bind as a collection.</typeparam>
        /// <returns>A fluent builder for configuring the asset collection binding.</returns>
        protected AssetBindingBuilder<TConcrete> BindAssets<TConcrete>() where TConcrete : Object
        {
            return BindMultipleAssets<TConcrete>();
        }

        /// <summary>
        /// Registers a collection binding for all assets of type <typeparamref name="TConcrete" />.
        /// Use this to inject multiple assets (e.g. from a folder or group) as a collection.
        /// </summary>
        /// <typeparam name="TConcrete">The UnityEngine.Object-derived asset type to bind as a collection.</typeparam>
        /// <returns>A fluent builder for configuring the asset collection binding.</returns>
        protected AssetBindingBuilder<TConcrete> BindMultipleAssets<TConcrete>() where TConcrete : Object
        {
            Binding binding = new(null, typeof(TConcrete), this);
            binding.MarkAssetBinding();
            binding.MarkCollectionBinding();
            unvalidatedBindings.Add(binding);
            return new AssetBindingBuilder<TConcrete>(binding, this);
        }

        /// <summary>
        /// Registers a binding from interface type <typeparamref name="TInterface" /> to asset type <typeparamref name="TConcrete" />.
        /// Use this to inject an asset by interface abstraction (e.g. ScriptableObject implementing an interface).
        /// </summary>
        /// <typeparam name="TInterface">The interface type to inject.</typeparam>
        /// <typeparam name="TConcrete">The concrete asset type that implements the interface.</typeparam>
        /// <returns>A fluent builder for configuring the asset binding.</returns>
        protected AssetBindingBuilder<TConcrete> BindAsset<TInterface, TConcrete>()
            where TConcrete : Object, TInterface
            where TInterface : class
        {
            Binding binding = new(typeof(TInterface), typeof(TConcrete), this);
            binding.MarkAssetBinding();
            unvalidatedBindings.Add(binding);
            return new AssetBindingBuilder<TConcrete>(binding, this);
        }

        /// <summary>
        /// Registers a collection binding from interface type <typeparamref name="TInterface" /> to asset type <typeparamref name="TConcrete" />.
        /// Use this to inject multiple assets implementing the interface as a collection.
        /// Shorthand for <see cref="BindMultipleAssets{TInterface,TConcrete}" />.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to inject.</typeparam>
        /// <typeparam name="TConcrete">The concrete asset type that implements the interface.</typeparam>
        /// <returns>A fluent builder for configuring the asset collection binding.</returns>
        protected AssetBindingBuilder<TConcrete> BindAssets<TInterface, TConcrete>()
            where TConcrete : Object, TInterface
            where TInterface : class
        {
            return BindMultipleAssets<TInterface, TConcrete>();
        }

        /// <summary>
        /// Registers a collection binding from interface type <typeparamref name="TInterface" /> to asset type <typeparamref name="TConcrete" />.
        /// Use this to inject multiple assets implementing the interface as a collection.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to inject.</typeparam>
        /// <typeparam name="TConcrete">The concrete asset type that implements the interface.</typeparam>
        /// <returns>A fluent builder for configuring the asset collection binding.</returns>
        protected AssetBindingBuilder<TConcrete> BindMultipleAssets<TInterface, TConcrete>()
            where TConcrete : Object, TInterface
            where TInterface : class
        {
            Binding binding = new(typeof(TInterface), typeof(TConcrete), this);
            binding.MarkAssetBinding();
            binding.MarkCollectionBinding();
            unvalidatedBindings.Add(binding);
            return new AssetBindingBuilder<TConcrete>(binding, this);
        }

        /// <summary>
        /// Registers a global binding for the scene component <typeparamref name="TComponent" />.
        /// Promotes the component into the global scope via <c>SceneGlobalContainer</c>.
        /// Enables cross-scene access through global resolution (e.g., via <c>InterfaceProxyObject</c>).
        /// </summary>
        /// <typeparam name="TComponent">The <see cref="Component" /> type to bind globally.</typeparam>
        /// <returns>A fluent builder for configuring the global component binding.</returns>
        protected ComponentBindingBuilder<TComponent> BindGlobal<TComponent>() where TComponent : Component
        {
            Binding binding = new(null, typeof(TComponent), this);
            binding.MarkComponentBinding();
            binding.MarkGlobal();
            unvalidatedBindings.Add(binding);
            return new ComponentBindingBuilder<TComponent>(binding, this);
        }

        #endregion
    }
}