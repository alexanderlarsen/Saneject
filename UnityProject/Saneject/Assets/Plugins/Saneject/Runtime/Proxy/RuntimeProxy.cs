using System;
using System.ComponentModel;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Plugins.Saneject.Runtime.Proxy
{
    /// <summary>
    /// Generic base class for runtime proxy placeholder assets that stand in for <see cref="Component"/> dependencies at editor time.
    /// </summary>
    /// <remarks>
    /// A <see cref="RuntimeProxy{TComponent}"/> is a <see cref="ScriptableObject"/> placeholder injected into interface members when Unity cannot serialize the real
    /// reference directly. Generated proxy types implement the target component's public non-generic interfaces with members that throw until the proxy is swapped.
    /// During scope startup, registered <see cref="IRuntimeProxySwapTarget"/> components resolve the proxy with the configured
    /// <see cref="RuntimeProxyResolveMethod"/> and replace the placeholder with the real runtime instance.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class RuntimeProxy<TComponent> : RuntimeProxyBase
        where TComponent : Component
    {
        /// <summary>
        /// Resolves and returns the real instance for this proxy using the configured resolution strategy.
        /// </summary>
        /// <remarks>
        /// Lookup-based methods return existing runtime instances. Creation-based methods create or reuse an instance according to
        /// <see cref="RuntimeProxyInstanceMode"/>, using <see cref="GlobalScope"/> to cache singleton instances.
        /// If called outside Play Mode, Saneject logs a warning and returns <c>null</c>.
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
