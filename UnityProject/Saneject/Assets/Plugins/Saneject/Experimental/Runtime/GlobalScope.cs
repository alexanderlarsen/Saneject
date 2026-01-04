using System;
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Runtime
{
    /// <summary>
    /// Static global registry and service locator for <see cref="UnityEngine.Object" /> singletons.
    /// Allows systems to register, query, and remove global instances by type at runtime.
    /// Intended for play mode usage only; not available in Edit Mode.
    /// </summary>
    public static class GlobalScope
    {
        private static readonly Dictionary<Type, Component> Instances = new();
        private static readonly Dictionary<Type, Object> OwnerTypeMap = new();

        /// <summary>
        /// Used by tests to override runtime requirement.
        /// </summary>
        private static bool allowUseInEditMode;

        public static bool IsModificationAllowed => Application.isPlaying || allowUseInEditMode;

        /// <summary>
        /// Remove all registered global instances. Only valid in Play Mode.
        /// </summary>
        public static void Clear()
        {
            if (!CheckModificationAllowed())
                return;

            Instances.Clear();
            OwnerTypeMap.Clear();

            if (UserSettings.LogGlobalScopeRegistration)
                Debug.Log("Saneject: GlobalScope cleared. All global registrations removed.");
        }

        /// <summary>
        /// Get the registered global instance of type <typeparamref name="T" />. Returns <c>null</c> if not registered.
        /// </summary>
        public static T GetComponent<T>() where T : Component
        {
            return Instances.TryGetValue(typeof(T), out Component instance)
                ? instance as T
                : null;
        }

        /// <summary>
        /// Register a global instance. Only valid in Play Mode. Only one instance per type.
        /// </summary>
        public static void RegisterComponent(
            Component instance,
            Object caller)
        {
            if (!CheckModificationAllowed())
                return;

            if (caller == null)
                throw new ArgumentNullException(nameof(caller), $"Saneject: GlobalScope registration failed. Caller is null while attempting to register '{instance?.GetType().Name ?? "<unknown>"}'.");

            if (instance == null)
                throw new ArgumentNullException(nameof(instance), $"Saneject: GlobalScope registration failed. '{caller.GetType().Name}' attempted to register a null instance.");

            Type type = instance.GetType();

            if (!Instances.TryAdd(type, instance))
            {
                Object owner = OwnerTypeMap[type];

                Debug.LogError($"Saneject: GlobalScope registration failed. '{caller.GetType().Name}' attempted to register '{type.Name}', but it is already registered by '{owner.GetType().Name}'.", caller);
                return;
            }

            OwnerTypeMap[type] = caller;

            if (UserSettings.LogGlobalScopeRegistration)
                Debug.Log($"Saneject: GlobalScope registration success. '{caller.GetType().Name}' registered '{type.Name}'.", caller);
        }

        /// <summary>
        /// Unregister a global instance. Only valid in Play Mode.
        /// </summary>
        public static void UnregisterComponent(
            Component instance,
            Object caller)
        {
            if (!CheckModificationAllowed())
                return;

            if (caller == null)
                throw new ArgumentNullException(nameof(caller), $"Saneject: GlobalScope unregistration failed. Caller is null while attempting to unregister '{instance?.GetType().Name ?? "<unknown>"}'.");

            if (instance == null)
                throw new ArgumentNullException(nameof(instance), $"Saneject: GlobalScope unregistration failed. '{caller.GetType().Name}' attempted to unregister a null instance.");

            Type type = instance.GetType();

            if (!OwnerTypeMap.TryGetValue(type, out Object owner))
            {
                Debug.LogWarning($"Saneject: GlobalScope unregistration failed. '{caller.GetType().Name}' attempted to remove '{type.Name}', but it is not registered.", caller);
                return;
            }

            if (!ReferenceEquals(owner, caller))
            {
                Debug.LogError($"Saneject: GlobalScope unregistration failed. '{caller.GetType().Name}' does not own '{type.Name}'.", caller);
                return;
            }

            Instances.Remove(type);
            OwnerTypeMap.Remove(type);

            if (UserSettings.LogGlobalScopeRegistration)
                Debug.Log($"Saneject: GlobalScope unregistration success. '{caller.GetType().Name}' removed '{type.Name}' from GlobalScope.", caller);
        }

        /// <summary>
        /// Returns true if a global instance of type <typeparamref name="T" /> is registered.
        /// </summary>
        public static bool IsRegistered<T>() where T : Component
        {
            return Instances.ContainsKey(typeof(T));
        }

        private static bool CheckModificationAllowed()
        {
            if (!IsModificationAllowed)
                Debug.LogWarning("Saneject: GlobalScope modification rejected. Play Mode is required.");

            return IsModificationAllowed;
        }
    }
}