using System;
using System.ComponentModel;
using Plugins.Saneject.Experimental.Runtime.Scopes;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEngine;
using Component = UnityEngine.Component;

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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class RuntimeProxy<TConcrete> : RuntimeProxyBase
        where TConcrete : Component
    {
        public override object ResolveInstance()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning(
                    $"Saneject: '{GetType().Name}.{nameof(ResolveInstance)}()' called in editor. This is not allowed.");

                return null;
            }

            // Pure lookup-only
            if (resolveMethod == RuntimeProxyResolveMethod.FromGlobalScope)
                return GlobalScope.GetComponent<TConcrete>();

            // Global get-or-create pre-check
            if (instanceMode == RuntimeProxyInstanceMode.Singleton &&
                GlobalScope.TryGetComponent(out TConcrete existing))
                return existing;

            // Resolve via configured source
            TConcrete resolved = resolveMethod switch
            {
                RuntimeProxyResolveMethod.FromComponentOnPrefab
                    => GetFromPrefab(),

                RuntimeProxyResolveMethod.FromNewComponentOnNewGameObject
                    => CreateNewInstanceOnNewGameObject(),

                RuntimeProxyResolveMethod.FromAnywhereInLoadedScenes
                    => FindFirstObjectByType<TConcrete>(FindObjectsInactive.Include),
                
                RuntimeProxyResolveMethod.FromGlobalScope => throw new InvalidOperationException("Saneject: Resolve method should never reach this switch arm."),
                
                _ => throw new ArgumentOutOfRangeException()
            };

            if (resolved == null)
            {
                Debug.LogError(
                    $"Saneject: '{typeof(TConcrete)}' could not be resolved by '{GetType().Name}' using '{resolveMethod}'.");

                return null;
            }

            // Register instance only if this proxy created it and policy demands it
            if (instanceMode == RuntimeProxyInstanceMode.Singleton &&
                resolveMethod is RuntimeProxyResolveMethod.FromComponentOnPrefab or RuntimeProxyResolveMethod.FromNewComponentOnNewGameObject)
                if (!GlobalScope.IsRegistered<TConcrete>())
                    GlobalScope.RegisterComponent(resolved, this);

            if (UserSettings.LogProxyResolve)
                Debug.Log(
                    $"Saneject: '{typeof(TConcrete)}' resolved by '{GetType().Name}' using '{resolveMethod}'.");

            return resolved;
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