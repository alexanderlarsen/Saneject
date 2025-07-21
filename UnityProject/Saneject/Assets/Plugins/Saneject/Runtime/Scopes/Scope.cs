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
        /// Registers a <see cref="UnityEngine.Component" /> as a global-scoped dependency.
        /// Dependencies registered with this method are added to/serialized in a <see cref="Plugins.Saneject.Runtime.Global.SceneGlobalContainer" /> during injection, and added to <see cref="Plugins.Saneject.Runtime.Global.GlobalScope" /> at runtime.
        /// Use this for dependencies that live in another scene or outside a prefab (and not in the Project folder as these can just be injected with <see cref="RegisterObject{T}" /> or <see cref="RegisterObject{TInterface, TConcrete}" />).
        /// </summary>
        /// <returns>A fluent builder to configure the binding</returns>
        protected BindingBuilder<T> Bind<T>() where T : Object
        {
            BindingBuilder<T> builder = new(this, typeof(T));

            if (!bindings.Add(builder.binding))
            {
                string errorMessage = $"Saneject: Scope '{GetType().Name}' has duplicate bindings for '{typeof(T).Name}' (ID: {builder.binding.Id})";
                Debug.LogError(errorMessage);
            }

            return builder;
        }

        /// <summary>
        /// Starts registration for an interface-to-object binding in this scope.
        /// Use this to resolve a dependency of interface type <typeparamref name="TInterface" /> with a concrete <typeparamref name="TConcrete" /> implementation.
        /// </summary>
        /// <returns>A fluent builder to configure the binding</returns>
        protected BindingBuilder<TConcrete> Bind<TInterface, TConcrete>()
            where TConcrete : Object, TInterface
            where TInterface : class
        {
            BindingBuilder<TConcrete> builder = new(this, typeof(TInterface), typeof(TConcrete));

            if (!bindings.Add(builder.binding))
            {
                string errorMessage = $"Saneject: Scope '{GetType().Name}' has duplicate bindings for '{typeof(TInterface).Name}' (ID: {builder.binding.Id})";
                Debug.LogError(errorMessage);
            }

            return builder;
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