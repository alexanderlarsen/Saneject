using System;
using System.ComponentModel;
using Plugins.Saneject.Experimental.Runtime.Scopes;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    /// <summary>
    /// A generic base class for serializable proxy assets that resolve to real component instances at runtime.
    /// </summary>
    /// <remarks>
    /// A <see cref="RuntimeProxy{TConcrete}" /> is a <see cref="ScriptableObject" /> that serves as an editor-time placeholder for a <see cref="UnityEngine.Component" />.
    /// It implements all interfaces of the target type with stub implementations (which throw exceptions). At runtime, a Scope resolves the proxy
    /// to the real instance using the configured <see cref="RuntimeProxyResolveMethod" /> and replaces all proxy references via <see cref="IRuntimeProxySwapTarget.SwapProxiesWithRealInstances" />.
    /// This design allows you to serialize interface references between scenes and prefabs, solving the problem that Unity cannot serialize
    /// direct cross-scene, scene-to-prefab and prefab-to-prefab references.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class RuntimeProxy<TComponent> : RuntimeProxyBase
        where TComponent : Component
    {
        /// <summary>
        /// Resolves and returns the real instance for this proxy using the configured resolution strategy.
        /// </summary>
        /// <remarks>
        /// This method uses the <see cref="RuntimeProxyResolveMethod" /> configured on this proxy to locate or create the target instance.
        /// If a <see cref="RuntimeProxyInstanceMode.Singleton" /> instance is requested, the instance is resolved from the <see cref="GlobalScope" />.
        /// Cannot be called in the editor; logs a warning and returns null if attempted outside play mode.
        /// </remarks>
        /// <returns>The resolved component instance, or null if resolution fails.</returns>
        public override object ResolveInstance()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning(
                    $"Saneject: '{GetType().Name}.{nameof(ResolveInstance)}()' called in editor. This is not allowed.");

#if UNITY_EDITOR
                resolvedInstance = null;
#endif
                return null;
            }

            // Pure lookup-only
            if (resolveMethod == RuntimeProxyResolveMethod.FromGlobalScope)
            {
                TComponent component = GlobalScope.GetComponent<TComponent>();
#if UNITY_EDITOR
                resolvedInstance = component;
#endif
                return component;
            }

            // Global get-or-create pre-check
            if (instanceMode == RuntimeProxyInstanceMode.Singleton &&
                GlobalScope.TryGetComponent(out TComponent existing))
            {
#if UNITY_EDITOR
                resolvedInstance = existing;
#endif
                return existing;
            }

            // Resolve via configured source
            TComponent resolved = resolveMethod switch
            {
                RuntimeProxyResolveMethod.FromComponentOnPrefab
                    => GetFromPrefab(),

                RuntimeProxyResolveMethod.FromNewComponentOnNewGameObject
                    => CreateNewInstanceOnNewGameObject(),

                RuntimeProxyResolveMethod.FromAnywhereInLoadedScenes
                    => FindFirstObjectByType<TComponent>(FindObjectsInactive.Include),

                RuntimeProxyResolveMethod.FromGlobalScope => throw new InvalidOperationException("Saneject: Resolve method should never reach this switch arm."),

                _ => throw new ArgumentOutOfRangeException()
            };

            if (resolved == null)
            {
                Debug.LogError(
                    $"Saneject: '{typeof(TComponent)}' could not be resolved by '{GetType().Name}' using '{resolveMethod}'.");

#if UNITY_EDITOR
                resolvedInstance = null;
#endif
                return null;
            }

            // Register instance only if this proxy created it and policy demands it
            if (instanceMode == RuntimeProxyInstanceMode.Singleton &&
                resolveMethod is RuntimeProxyResolveMethod.FromComponentOnPrefab or RuntimeProxyResolveMethod.FromNewComponentOnNewGameObject)
                if (!GlobalScope.IsRegistered<TComponent>())
                    GlobalScope.RegisterComponent(resolved, this);

            if (UserSettings.LogProxyResolve)
                Debug.Log(
                    $"Saneject: '{typeof(TComponent)}' resolved by '{GetType().Name}' using '{resolveMethod}'.");

#if UNITY_EDITOR
            resolvedInstance = resolved;
#endif
            return resolved;
        }

        private TComponent GetFromPrefab()
        {
            GameObject prefabInstance = Instantiate(prefab);

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(prefabInstance);

            return prefabInstance.TryGetComponent(out TComponent output)
                ? output
                : throw new NullReferenceException(
                    $"Saneject: '{typeof(TComponent)}' is not found on prefab instantiated by '{GetType().Name}'");
        }

        private TComponent CreateNewInstanceOnNewGameObject()
        {
            GameObject gameObj = new(typeof(TComponent).Name);

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObj);

            return gameObj.AddComponent<TComponent>();
        }
    }
}