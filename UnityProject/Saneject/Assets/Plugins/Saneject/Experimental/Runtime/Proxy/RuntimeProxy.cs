using System;
using Plugins.Saneject.Experimental.Runtime.Scopes;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    /// <summary>
    /// Enables serialization of interface references to Unity objects between scenes and prefabs.
    /// Use this as a base for proxies generated with Roslyn (via Saneject.RuntimeProxy.Generator.dll),
    /// which implement all interfaces on the concrete type at compile time and forward methods,
    /// properties and events to a concrete instance located at runtime.
    /// Assign the proxy asset to a serialized interface field (e.g.,
    /// <c>[SerializeInterface] IMyInterface myInterface</c>) at editor-time, and at runtime,
    /// the proxy resolves to the real instance using your chosen strategy.
    /// For details and usage examples, see the README.
    /// </summary>
    public abstract class RuntimeProxy<TConcrete> : RuntimeProxyBase
        where TConcrete : Component
    {
        [NonSerialized]
        protected TConcrete instance;

        /// <summary>
        /// Manually register an instance for this proxy.
        /// Only required for <see cref="ProxyResolveMethod.FromManualRegistration" /> resolve method.
        /// </summary>
        public void RegisterInstance(TConcrete newInstance)
        {
            if (hadInstanceBefore)
                OnTargetInstanceLost();

            instance = newInstance;

            if (instance)
                hadInstanceBefore = true;
        }

        /// <summary>
        /// Unregisters the currently registered instance, if any.
        /// </summary>
        public void UnregisterInstance()
        {
            if (hadInstanceBefore)
                OnTargetInstanceLost();

            instance = null;
        }

        /// <summary>
        /// Returns the resolved concrete instance as <typeparamref name="T" />, or null if not resolved.
        /// Use if you need to access or cache the direct instance for performance-critical scenarios.
        /// </summary>
        /// <returns>The concrete instance cast to T.</returns>
        public T GetInstanceAs<T>() where T : TConcrete
        {
            if (!instance)
            {
                Debug.LogWarning($"Saneject: '{GetType().Name}' instance not resolved. Returning null.");
                return null;
            }

            return (T)instance;
        }

        /// <summary>
        /// Resolves the underlying concrete instance according to the configured resolve strategy.
        /// Called automatically by the generated proxy when the instance is null.
        /// </summary>
        protected void ResolveInstance()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning(
                    $"Saneject: '{GetType().Name}.{nameof(ResolveInstance)}()' called in editor. " +
                    "This is not allowed.");

                return;
            }

            if (instance)
                return;

            TConcrete resolved = resolveMethod switch
            {
                ProxyResolveMethod.FromGlobalScope => GetFromGlobalScope(),
                ProxyResolveMethod.FromManualRegistration => instance,
                ProxyResolveMethod.FromComponentOnPrefab => GetFromPrefab(),
                ProxyResolveMethod.FromNewComponentOnNewGameObject => CreateNewInstanceOnNewGameObject(),
                ProxyResolveMethod.FromAnywhereInLoadedScenes => FindObjectInAnyScene(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (hadInstanceBefore)
                OnTargetInstanceLost();

            instance = resolved;

            if (instance)
                hadInstanceBefore = true;

            if (UserSettings.LogProxyResolve)
            {
                if (instance)
                    Debug.Log($"Saneject: '{GetType().Name}' resolved its instance using {resolveMethod}.");
                else
                    Debug.LogWarning($"Saneject: '{GetType().Name}' instance is null.");
            }
        }

        private TConcrete GetFromGlobalScope()
        {
            return GlobalScope.GetComponent<TConcrete>();
        }

        private TConcrete FindObjectInAnyScene()
        {
            return FindFirstObjectByType<TConcrete>(FindObjectsInactive.Include);
        }

        private TConcrete GetFromPrefab()
        {
            GameObject prefabInstance = Instantiate(prefab);

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(prefabInstance);

            return prefabInstance.TryGetComponent(out TConcrete output)
                ? output
                : throw new NullReferenceException(
                    $"Saneject: '{typeof(TConcrete)}' is not found on prefab instantiated by '{GetType().Name}'");
        }

        private TConcrete CreateNewInstanceOnNewGameObject()
        {
            GameObject gameObj = new(typeof(TConcrete).Name);

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObj);

            return gameObj.AddComponent<TConcrete>();
        }
    }
}