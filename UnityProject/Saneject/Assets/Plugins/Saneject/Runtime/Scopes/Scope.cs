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
        private readonly Dictionary<BindingKey, Binding> bindings = new();
        private readonly Dictionary<BindingKey, Binding> globalBindings = new();

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
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Binding GetBindingRecursiveUpwards(
            object id,
            Type type)
        {
            if (Application.isPlaying)
                throw new Exception("Saneject: Injection is editor-only. Exit Play Mode to inject.");

            BindingKey key = new(id, type);

            if (bindings.TryGetValue(key, out Binding binding))
                return binding;

            return ParentScope ? ParentScope.GetBindingRecursiveUpwards(id, type) : null;
        }

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Binding> GetGlobalBindings()
        {
            return globalBindings.Values.ToList();
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
            globalBindings.Clear();
            ParentScope = null;
        }

        /// <summary>
        /// For internal use by Saneject. Not intended for user code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Binding> GetUnusedBindings()
        {
            return bindings.Values.Where(binding => !binding.IsUsed).ToList();
        }

        /// <summary>
        /// Registers a <see cref="UnityEngine.Component" /> as a global-scoped dependency.
        /// Dependencies registered with this method are added to/serialized in a <see cref="Plugins.Saneject.Runtime.Global.SceneGlobalContainer" /> during injection, and added to <see cref="Plugins.Saneject.Runtime.Global.GlobalScope" /> at runtime.
        /// Use this for dependencies that live in another scene or outside a prefab (and not in the Project folder as these can just be injected with <see cref="RegisterObject{T}" /> or <see cref="RegisterObject{TInterface, TConcrete}" />).
        /// </summary>
        /// <returns>A fluent builder to configure the binding</returns>
        protected ComponentBindingBuilder<T> RegisterGlobalComponent<T>() where T : Component
        {
            ComponentBindingBuilder<T> builder = new(this, typeof(T));
            builder.OnBindingCreated += RegisterBinding;
            return builder;

            void RegisterBinding(Binding binding)
            {
                BindingKey key = new(binding.id, typeof(T));

                if (globalBindings.ContainsKey(key))
                {
                    string errorMessage = $"Saneject: Scope '{GetType().Name}' has duplicate global bindings for '{typeof(T).Name}' (ID: {binding.id})";
                    Debug.LogError(errorMessage);
                }

                globalBindings[key] = binding;
                builder.OnBindingCreated -= RegisterBinding;
            }
        }

        /// <summary>
        /// Registers a <see cref="UnityEngine.Object" /> as a global-scoped dependency.
        /// Dependencies registered with this method are added to/serialized in a <see cref="Plugins.Saneject.Runtime.Global.SceneGlobalContainer" /> during injection, and added to <see cref="Plugins.Saneject.Runtime.Global.GlobalScope" /> at runtime.
        /// Use this for dependencies that live in another scene or outside a prefab (and not in the Project folder as these can just be injected with <see cref="RegisterObject{T}" /> or <see cref="RegisterObject{TInterface, TConcrete}" />).
        /// </summary>
        /// <returns>A fluent builder to configure the binding</returns>
        protected ObjectBindingBuilder<T> RegisterGlobalObject<T>() where T : Object
        {
            ObjectBindingBuilder<T> builder = new(typeof(T));
            builder.OnBindingCreated += RegisterBinding;
            return builder;

            void RegisterBinding(Binding binding)
            {
                BindingKey key = new(binding.id, typeof(T));

                if (globalBindings.ContainsKey(key))
                {
                    string errorMessage = $"Saneject: Scope '{GetType().Name}' has duplicate global bindings for '{typeof(T).Name}' (ID: {binding.id})";
                    Debug.LogError(errorMessage);
                }

                globalBindings[key] = binding;
                builder.OnBindingCreated -= RegisterBinding;
            }
        }

        /// <summary>
        /// Starts registration for a dependency of type <see cref="UnityEngine.Component" /> in this scope.
        /// </summary>
        /// <returns>A fluent builder to configure the binding</returns>
        protected ComponentBindingBuilder<T> RegisterComponent<T>() where T : Component
        {
            ComponentBindingBuilder<T> builder = new(this, typeof(T));
            builder.OnBindingCreated += RegisterBinding;
            return builder;

            void RegisterBinding(Binding binding)
            {
                BindingKey key = new(binding.id, typeof(T));

                if (bindings.ContainsKey(key))
                {
                    string errorMessage = $"Saneject: Scope '{GetType().Name}' has duplicate bindings for '{typeof(T).Name}' (ID: {binding.id})";
                    Debug.LogError(errorMessage);
                }

                bindings[key] = binding;
                builder.OnBindingCreated -= RegisterBinding;
            }
        }

        /// <summary>
        /// Starts registration for an interface-to-component binding in this scope.
        /// Use this to resolve a dependency of interface type <typeparamref name="TInterface" /> with a concrete <typeparamref name="TConcrete" /> implementation.
        /// </summary>
        /// <returns>A fluent builder to configure the binding</returns>
        protected ComponentBindingBuilder<TConcrete> RegisterComponent<TInterface, TConcrete>()
            where TConcrete : Component, TInterface
            where TInterface : class
        {
            ComponentBindingBuilder<TConcrete> builder = new(this, typeof(TInterface), typeof(TConcrete));
            builder.OnBindingCreated += RegisterBinding;
            return builder;

            void RegisterBinding(Binding binding)
            {
                BindingKey key = new(binding.id, typeof(TInterface));

                if (bindings.ContainsKey(key))
                {
                    string errorMessage = $"Saneject: Scope '{GetType().Name}' has duplicate bindings for '{typeof(TInterface).Name}' (ID: {binding.id})";
                    Debug.LogError(errorMessage);
                }

                bindings[key] = binding;
                builder.OnBindingCreated -= RegisterBinding;
            }
        }

        /// <summary>
        /// Starts registration for a dependency of type <see cref="UnityEngine.Object" /> in this scope.
        /// Returns a fluent builder to configure the binding.
        /// </summary>
        /// <returns>A fluent builder to configure the binding</returns>
        protected ObjectBindingBuilder<T> RegisterObject<T>() where T : Object
        {
            ObjectBindingBuilder<T> builder = new(typeof(T));
            builder.OnBindingCreated += RegisterBinding;
            return builder;

            void RegisterBinding(Binding binding)
            {
                BindingKey key = new(binding.id, typeof(T));

                if (bindings.ContainsKey(key))
                {
                    string errorMessage = $"Saneject: Scope '{GetType().Name}' has duplicate bindings for '{typeof(T).Name}' (ID: {binding.id})";
                    Debug.LogError(errorMessage);
                }

                bindings[key] = binding;
                builder.OnBindingCreated -= RegisterBinding;
            }
        }

        /// <summary>
        /// Starts registration for an interface-to-object binding in this scope.
        /// Use this to resolve a dependency of interface type <typeparamref name="TInterface" /> with a concrete <typeparamref name="TConcrete" /> implementation.
        /// </summary>
        /// <returns>A fluent builder to configure the binding</returns>
        protected ObjectBindingBuilder<TConcrete> RegisterObject<TInterface, TConcrete>()
            where TConcrete : Object, TInterface
            where TInterface : class
        {
            ObjectBindingBuilder<TConcrete> builder = new(typeof(TInterface), typeof(TConcrete));
            builder.OnBindingCreated += RegisterBinding;
            return builder;

            void RegisterBinding(Binding binding)
            {
                BindingKey key = new(binding.id, typeof(TInterface));

                if (bindings.ContainsKey(key))
                {
                    string errorMessage = $"Saneject: Scope '{GetType().Name}' has duplicate bindings for '{typeof(TInterface).Name}' (ID: {binding.id})";
                    Debug.LogError(errorMessage);
                }

                bindings[key] = binding;
                builder.OnBindingCreated -= RegisterBinding;
            }
        }

        /// <summary>
        /// Key class to ensure uniqueness in bindings per scope.
        /// </summary>
        private class BindingKey : IEquatable<BindingKey>
        {
            private readonly object id;
            private readonly Type type;

            public BindingKey(
                object id,
                Type type)
            {
                this.id = id;
                this.type = type;
            }

            public bool Equals(BindingKey other)
            {
                if (other is null)
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                return Equals(id, other.id) && type == other.type;
            }

            public override bool Equals(object obj)
            {
                if (obj is null)
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                return obj.GetType() == GetType() && Equals((BindingKey)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(id, type);
            }
        }
    }
}