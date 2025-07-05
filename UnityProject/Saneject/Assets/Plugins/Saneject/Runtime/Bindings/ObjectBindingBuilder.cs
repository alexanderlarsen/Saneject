using System;
using System.ComponentModel;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Scopes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Fluent builder for registering an object binding.
    /// Used to define how dependencies of type <c>T</c> (an <see cref="Object"/>) are located or created.
    /// Typical usage: call <see cref="Scope.RegisterObject{T}"/> or <see cref="Scope.RegisterObject{TInterface,TConcrete}"/>
    /// and chain with methods on this builder to specify the search strategy for the dependency.
    /// </summary>
    /// <typeparam name="T">The <see cref="Object"/> type to bind.</typeparam>
    public class ObjectBindingBuilder<T> where T : Object
    {
        private readonly Type interfaceType;
        private readonly Type concreteType;
        private string id;

        public ObjectBindingBuilder(Type concreteType)
        {
            interfaceType = null;
            this.concreteType = concreteType;
        }

        public ObjectBindingBuilder(
            Type interfaceType,
            Type concreteType)
        {
            this.interfaceType = interfaceType;
            this.concreteType = concreteType;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event Action<Binding> OnBindingCreated;

        /// <summary>
        /// Set the binding ID for resolution of fields/properties with the same ID in their <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ObjectBindingBuilder<T> WithId(string id)
        {
            this.id = id;
            return this;
        }

        /// <summary>
        /// Bind to the specified <see cref="Object" /> instance.
        /// </summary>
        public FilterableObjectBindingBuilder<T> FromInstance(T instance)
        {
            Binding binding = new(interfaceType, concreteType, id, _ => instance.WrapInEnumerable());
            OnBindingCreated?.Invoke(binding);
            return new FilterableObjectBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> using the provided factory method.
        /// </summary>
        public FilterableObjectBindingBuilder<T> From(Func<T> factory)
        {
            Binding binding = new(interfaceType, concreteType, id, _ => factory?.Invoke().WrapInEnumerable());
            OnBindingCreated?.Invoke(binding);
            return new FilterableObjectBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> in a <see cref="Resources" /> folder at the specified path using <see cref="Resources.Load(string)" />.
        /// </summary>
        public FilterableObjectBindingBuilder<T> FromResources(string path)
        {
            Binding binding = new(interfaceType, concreteType, id, _ => Resources.Load<T>(path).WrapInEnumerable());
            OnBindingCreated?.Invoke(binding);
            return new FilterableObjectBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate all <see cref="Object" />s in a <see cref="Resources" /> folder at the specified path using <see cref="Resources.LoadAll(string, System.Type)" />.
        /// </summary>
        public FilterableObjectBindingBuilder<T> FromResourcesAll(string path)
        {
            Binding binding = new(interfaceType, concreteType, id, _ => Resources.LoadAll<T>(path));
            OnBindingCreated?.Invoke(binding);
            return new FilterableObjectBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> anywhere in the scene using <see cref="Object.FindObjectsByType(Type, FindObjectsSortMode)" />.
        /// </summary>
        public FilterableObjectBindingBuilder<T> FromAnywhereInScene()
        {
            Binding binding = new(interfaceType, concreteType, id, _ => Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None));
            OnBindingCreated?.Invoke(binding);
            return new FilterableObjectBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> asset at the specified path using <see cref="UnityEditor.AssetDatabase.LoadAssetAtPath(string, System.Type)" />.
        /// </summary>
        public FilterableObjectBindingBuilder<T> FromAssetLoad(string assetPath)
        {
#if UNITY_EDITOR
            Binding binding = new(interfaceType, concreteType, id, _ => AssetDatabase.LoadAssetAtPath<T>(assetPath).WrapInEnumerable());
            OnBindingCreated?.Invoke(binding);
            return new FilterableObjectBindingBuilder<T>(binding);
#else
            return null;
#endif
        }
    }
}