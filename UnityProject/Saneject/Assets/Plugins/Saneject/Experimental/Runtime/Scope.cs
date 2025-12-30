using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Experimental.Runtime.Bindings;
using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;
using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Runtime
{
    public abstract class Scope : MonoBehaviour
    {
        #region INTERNAL METHODS/FIELDS

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private readonly HashSet<Binding> bindings = new();

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IReadOnlyCollection<Binding> GetBindings()
        {
            DeclareBindings();
            HashSet<Binding> bindingsCopy = bindings.ToHashSet();
            bindings.Clear();
            return bindingsCopy;
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
        protected abstract void DeclareBindings();

        #region COMPONENT METHODS

        /// <summary>
        /// Registers a binding for a single <typeparamref name="T" /> component or interface type in this scope.
        /// Use this for concrete <see cref="UnityEngine.Component" /> or interface you wish to inject directly.
        /// </summary>
        /// <typeparam name="T">The concrete component type to bind.</typeparam>
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
        /// Use this to inject all matching components as a collection.
        /// Shorthand for <see cref="BindMultipleComponents{T}" />.
        /// </summary>
        /// <typeparam name="T">The concrete component type to bind as a collection.</typeparam>
        /// <returns>A fluent builder for configuring the component collection binding.</returns>
        protected ComponentBindingBuilder<T> BindComponents<T>() where T : class
        {
            return BindMultipleComponents<T>();
        }

        /// <summary>
        /// Registers a collection binding for <typeparamref name="T" /> component type in this scope.
        /// Use this to inject all matching components or interfaces as a collection.
        /// </summary>
        /// <typeparam name="T">The concrete component type to bind as a collection.</typeparam>
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

        #region ASSET METHODS

        /// <summary>
        /// Registers a binding for a single asset of type <typeparamref name="TConcrete" />.
        /// Use this to inject a specific asset (e.g. <see cref="Material" />, <see cref="Texture" />, etc.) by reference.
        /// </summary>
        /// <typeparam name="TConcrete">The UnityEngine.Object-derived asset type to bind.</typeparam>
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
        /// Use this to inject an asset by interface abstraction (e.g. ScriptableObject implementing an interface).
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
                ConcreteType = typeof(TConcrete),
                IsCollectionBinding = true
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

        #region GLOBAL METHODS

        /// <summary>
        /// Registers a global binding for the scene component <typeparamref name="TConcrete" />.
        /// Promotes the component into the global scope via <c>SceneGlobalContainer</c>.
        /// Enables cross-scene access through global resolution (e.g., via <c>ProxyObject</c>).
        /// </summary>
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