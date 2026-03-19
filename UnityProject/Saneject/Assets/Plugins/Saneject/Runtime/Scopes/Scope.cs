using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Bindings.Asset;
using Plugins.Saneject.Runtime.Bindings.Component;
using Plugins.Saneject.Runtime.Proxy;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Scopes
{
    /// <summary>
    /// Base class for declaring dependency bindings within a <see cref="GameObject" /> hierarchy.
    /// A <c>Scope</c> locates dependencies for components below itself with bindings declared in <see cref="DeclareBindings" />.
    /// Saneject resolves dependencies by searching this scope and walking up the parent scope hierarchy until a match is found, allowing lower-level scopes to override bindings from higher-level scopes.
    /// <remarks>
    /// Although primarily used for editor-time DI, a <c>Scope</c> also manages runtime initialization by registering global components in <see cref="GlobalScope"/> and swapping <see cref="RuntimeProxy{TComponent}"/> placeholders with their real instances.
    /// </remarks>
    /// </summary>
    [DisallowMultipleComponent, DefaultExecutionOrder(-10000)]
    public abstract class Scope : MonoBehaviour
    {
        #region Internal stuff

        [SerializeField, HideInInspector, EditorBrowsable(EditorBrowsableState.Never)]
        private List<Component> globalComponents = new();

        [SerializeField, HideInInspector, EditorBrowsable(EditorBrowsableState.Never)]
        private List<Component> proxySwapTargets = new();

        /// <summary>
        /// Called by Unity on initialization. Registers global components in GlobalScope and swaps all proxy references with real instances.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private void Awake()
        {
            foreach (Component obj in globalComponents)
                GlobalScope.RegisterComponent(obj, this);

            foreach (IRuntimeProxySwapTarget swapTarget in proxySwapTargets.OfType<IRuntimeProxySwapTarget>())
                swapTarget.SwapProxiesWithRealInstances();
        }

        /// <summary>
        /// Called by Unity when this Scope is destroyed. Unregisters all global components from GlobalScope.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private void OnDestroy()
        {
            foreach (Component obj in globalComponents)
                GlobalScope.UnregisterComponent(obj, this);
        }

        /// <summary>
        /// Updates the collection of components to register in GlobalScope when this Scope initializes.
        /// Called by the editor injection system.
        /// </summary>
        /// <param name="globalObjects">The components to register globally.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateGlobalComponents(IEnumerable<Component> globalObjects)
        {
            globalComponents.Clear();
            globalComponents.AddRange(globalObjects.Where(obj => obj));
        }

        /// <summary>
        /// Clears the collection of proxy swap targets. Called by the editor injection system.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ClearProxySwapTargets()
        {
            proxySwapTargets.Clear();
        }

        /// <summary>
        /// Registers a component to have its proxy references swapped with real instances on Awake.
        /// </summary>
        /// <param name="component">The component implementing <see cref="IRuntimeProxySwapTarget"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the component does not implement <see cref="IRuntimeProxySwapTarget"/>.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddProxySwapTarget(Component component)
        {
            if (component is not IRuntimeProxySwapTarget)
                throw new ArgumentException($"Component {component.name} does not implement {nameof(IRuntimeProxySwapTarget)}");

            if (proxySwapTargets.Contains(component))
                return;

            proxySwapTargets.Add(component);
        }

        /// <summary>
        /// Collection of bindings declared in this scope. For internal use only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private readonly HashSet<Binding> bindings = new();

        /// <summary>
        /// Called by the Saneject injection system to collect all bindings declared in this scope.
        /// Invokes <see cref="DeclareBindings"/> to populate the bindings collection, then returns a copy.
        /// </summary>
        /// <returns>A read-only collection of all bindings declared in this scope.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IReadOnlyCollection<Binding> CollectBindings()
        {
            DeclareBindings();
            HashSet<Binding> bindingsCopy = bindings.ToHashSet();
            bindings.Clear();
            return bindingsCopy;
        }

        #endregion

        #region User-facing stuff

        /// <summary>
        /// Set up your bindings in this method.
        /// </summary>
        protected abstract void DeclareBindings();

        #region Component methods

        /// <summary>
        /// Registers a binding for a single <typeparamref name="T" /> component or interface type in this scope.
        /// Use this for concrete <see cref="UnityEngine.Component" /> or interface you wish to inject.
        /// </summary>
        /// <typeparam name="T">The concrete component or interface type to bind.</typeparam>
        /// <returns>A fluent builder for configuring the component binding.</returns>
        protected ComponentBindingBuilder<T> BindComponent<T>() where T : class
        {
            Type targetType = typeof(T);
            bool isInterface = targetType.IsInterface;

            ComponentBinding binding = new()
            {
                InterfaceType = isInterface ? targetType : null,
                ConcreteType = isInterface ? null : targetType
            };

            bindings.Add(binding);
            return new ComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Registers a collection binding for <typeparamref name="T" /> component type in this scope.
        /// Use this to inject all matching components or interfaces as a collection.
        /// Shorthand for <see cref="BindMultipleComponents{T}" />.
        /// </summary>
        /// <typeparam name="T">The concrete component or interface type to bind as a collection.</typeparam>
        /// <returns>A fluent builder for configuring the component collection binding.</returns>
        protected ComponentBindingBuilder<T> BindComponents<T>() where T : class
        {
            return BindMultipleComponents<T>();
        }

        /// <summary>
        /// Registers a collection binding for <typeparamref name="T" /> component type in this scope.
        /// Use this to inject all matching components or interfaces as a collection.
        /// </summary>
        /// <typeparam name="T">The concrete component or interface type to bind as a collection.</typeparam>
        /// <returns>A fluent builder for configuring the component collection binding.</returns>
        protected ComponentBindingBuilder<T> BindMultipleComponents<T>() where T : class
        {
            Type targetType = typeof(T);
            bool isInterface = targetType.IsInterface;

            ComponentBinding binding = new()
            {
                InterfaceType = isInterface ? targetType : null,
                ConcreteType = isInterface ? null : targetType,
                IsCollectionBinding = true
            };

            bindings.Add(binding);
            return new ComponentBindingBuilder<T>(binding);
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
            ComponentBinding binding = new()
            {
                InterfaceType = typeof(TInterface),
                ConcreteType = typeof(TConcrete)
            };

            bindings.Add(binding);
            return new ComponentBindingBuilder<TConcrete>(binding);
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
            ComponentBinding binding = new()
            {
                InterfaceType = typeof(TInterface),
                ConcreteType = typeof(TConcrete),
                IsCollectionBinding = true
            };

            bindings.Add(binding);
            return new ComponentBindingBuilder<TConcrete>(binding);
        }

        #endregion

        #region Asset methods

        /// <summary>
        /// Registers a binding for a single asset of type <typeparamref name="TConcrete" />.
        /// Use this to inject a specific asset (e.g. <see cref="GameObject" />, <see cref="Material" />, <see cref="Texture" />, etc.) by reference.
        /// </summary>
        /// <typeparam name="TConcrete">The <see cref="UnityEngine.Object"/>-derived asset type to bind.</typeparam>
        /// <returns>A fluent builder for configuring the asset binding.</returns>
        protected AssetBindingBuilder<TConcrete> BindAsset<TConcrete>() where TConcrete : Object
        {
            AssetBinding binding = new()
            {
                ConcreteType = typeof(TConcrete)
            };

            bindings.Add(binding);
            return new AssetBindingBuilder<TConcrete>(binding);
        }

        /// <summary>
        /// Registers a collection binding for all assets of type <typeparamref name="TConcrete" />.
        /// Use this to inject multiple assets (e.g. from a folder) as a collection.
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
        /// Use this to inject multiple assets (e.g. from a folder) as a collection.
        /// </summary>
        /// <typeparam name="TConcrete">The <see cref="UnityEngine.Object"/>-derived asset type to bind as a collection.</typeparam>
        /// <returns>A fluent builder for configuring the asset collection binding.</returns>
        protected AssetBindingBuilder<TConcrete> BindMultipleAssets<TConcrete>() where TConcrete : Object
        {
            AssetBinding binding = new()
            {
                ConcreteType = typeof(TConcrete),
                IsCollectionBinding = true
            };

            bindings.Add(binding);
            return new AssetBindingBuilder<TConcrete>(binding);
        }

        /// <summary>
        /// Registers a binding from interface type <typeparamref name="TInterface" /> to asset type <typeparamref name="TConcrete" />.
        /// Use this to inject an asset by interface abstraction (e.g. <see cref="ScriptableObject"/> implementing an interface).
        /// </summary>
        /// <typeparam name="TInterface">The interface type to inject.</typeparam>
        /// <typeparam name="TConcrete">The concrete asset type that implements the interface.</typeparam>
        /// <returns>A fluent builder for configuring the asset binding.</returns>
        protected AssetBindingBuilder<TConcrete> BindAsset<TInterface, TConcrete>()
            where TConcrete : Object, TInterface
            where TInterface : class
        {
            AssetBinding binding = new()
            {
                InterfaceType = typeof(TInterface),
                ConcreteType = typeof(TConcrete)
            };

            bindings.Add(binding);
            return new AssetBindingBuilder<TConcrete>(binding);
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
            AssetBinding binding = new()
            {
                InterfaceType = typeof(TInterface),
                ConcreteType = typeof(TConcrete),
                IsCollectionBinding = true
            };

            bindings.Add(binding);
            return new AssetBindingBuilder<TConcrete>(binding);
        }

        #endregion

        #region Global methods

        /// <summary>
        /// Registers a scene component globally, enabling efficient cross-scene and prefab resolution through <see cref="GlobalScope"/>.
        /// </summary>
        /// <remarks>
        /// During editor-time injection, this binding stores a reference to the target <typeparamref name="TConcrete"/> in this scope. 
        /// At runtime, before any Awake methods run and before proxies are resolved, the target is added to the <see cref="GlobalScope"/>
        /// This allows <see cref="RuntimeProxy{TComponent}"/> to resolve the component via fast dictionary lookup.
        /// Only works for components that exist in the scene or prefab at edit time.
        /// </remarks>
        /// <typeparam name="TConcrete">The <see cref="Component" /> type to bind globally.</typeparam>
        /// <returns>A fluent builder for configuring the global component binding.</returns>
        protected GlobalComponentBindingBuilder<TConcrete> BindGlobal<TConcrete>() where TConcrete : Component
        {
            GlobalComponentBinding binding = new()
            {
                ConcreteType = typeof(TConcrete)
            };

            bindings.Add(binding);
            return new GlobalComponentBindingBuilder<TConcrete>(binding);
        }

        #endregion

        #endregion
    }
}