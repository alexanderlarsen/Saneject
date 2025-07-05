using System;
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Global
{
    /// <summary>
    /// Static global registry and service locator for <see cref="UnityEngine.Object" /> singletons.
    /// Allows systems to register, query, and remove global instances by type at runtime.
    /// Intended for play mode usage only; not available in Edit Mode.
    /// </summary>
    public static class GlobalScope
    {
        private static readonly Dictionary<Type, Object> Instances = new();

        /// <summary>
        /// Remove all registered global instances. Only valid in Play Mode.
        /// </summary>
        public static void Clear()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning($"Saneject: {nameof(GlobalScope)} can only be used in Play Mode.");
                return;
            }

            Instances.Clear();
        }

        /// <summary>
        /// Get the registered global instance of type <typeparamref name="T" />. Returns <c>null</c> if not registered. Only valid in Play Mode.
        /// </summary>
        public static T Get<T>() where T : Object
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning($"Saneject: {nameof(GlobalScope)} can only be used in Play Mode.");
                return null;
            }

            if (!Instances.TryGetValue(typeof(T), out Object instance))
            {
                Debug.LogWarning($"Saneject: Type '{typeof(T).Name}' is not registered in GlobalScope.");
                return null;
            }

            return instance as T;
        }

        /// <summary>
        /// Register a global instance for the specified <see cref="Type" />. Only valid in Play Mode. Only one instance per type.
        /// </summary>
        public static void Register(
            Type type,
            Object instance)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning($"Saneject: {nameof(GlobalScope)} can only be used in Play Mode.");
                return;
            }

            bool added = Instances.TryAdd(type, instance);

            if (!UserSettings.LogGlobalScopeRegistration)
                return;

            if (added)
                Debug.Log($"Saneject: Added '{type.Name}' to GlobalScope.");
            else
                Debug.LogError($"Saneject: Type '{type.Name}' is already registered in GlobalScope. The GlobalScope can only contain one instance of each type.");
        }

        /// <summary>
        /// Register a global instance of type <typeparamref name="T" />. Only valid in Play Mode. Only one instance per type.
        /// </summary>
        public static void Register<T>(T instance) where T : Object
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning($"Saneject: {nameof(GlobalScope)} can only be used in Play Mode.");
                return;
            }

            Type type = typeof(T);
            bool added = Instances.TryAdd(type, instance);

            if (!UserSettings.LogGlobalScopeRegistration)
                return;

            if (added)
                Debug.Log($"Saneject: Added '{type.Name}' to GlobalScope.");
            else
                Debug.LogError($"Saneject: Type '{type.Name}' is already registered in GlobalScope. The GlobalScope can only contain one instance of each type.");
        }

        /// <summary>
        /// Unregister the global instance of the specified <see cref="Type" />. Only valid in Play Mode.
        /// </summary>
        public static void Unregister(Type type)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning($"Saneject: {nameof(GlobalScope)} can only be used in Play Mode.");
                return;
            }

            bool removed = Instances.Remove(type);

            if (!UserSettings.LogGlobalScopeRegistration)
                return;

            if (removed)
                Debug.Log($"Saneject: Removed '{type.Name}' from GlobalScope.");
            else
                Debug.LogWarning($"Saneject: You are trying to remove '{type.Name}' from GlobalScope but it isn't registered at this time.");
        }

        /// <summary>
        /// Unregister the global instance of type <typeparamref name="T" />. Only valid in Play Mode.
        /// </summary>
        public static void Unregister<T>() where T : Object
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning($"Saneject: {nameof(GlobalScope)} can only be used in Play Mode.");
                return;
            }

            Type type = typeof(T);
            bool removed = Instances.Remove(type);

            if (!UserSettings.LogGlobalScopeRegistration)
                return;

            if (removed)
                Debug.Log($"Saneject: Removed '{type.Name}' from GlobalScope.");
            else
                Debug.LogWarning($"Saneject: You are trying to remove '{type.Name}' from GlobalScope but it isn't registered at this time.");
        }

        /// <summary>
        /// Returns true if a global instance of type <typeparamref name="T" /> is registered. Only valid in Play Mode.
        /// </summary>
        public static bool IsRegistered<T>() where T : Object
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning($"Saneject: {nameof(GlobalScope)} can only be used in Play Mode.");
                return false;
            }

            return Instances.ContainsKey(typeof(T));
        }
    }
}