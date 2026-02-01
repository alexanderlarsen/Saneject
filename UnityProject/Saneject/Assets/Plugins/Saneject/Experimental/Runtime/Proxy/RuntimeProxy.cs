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

        protected int eventSubscriberCount;

        [SerializeField, Tooltip(
             "Determines how the proxy locates or creates the target instance at runtime.\n\n" +
             "FromGlobalScope: Uses GlobalScopeRegistry. Instance must be registered using RegisterGlobalComponent or RegisterGlobalObject in a scope.\n\n" +
             "FindInLoadedScenes: Finds the first matching component in any loaded scene using FindFirstObjectByType<TConcrete>(FindObjectsInactive.Include).\n\n" +
             "FromComponentOnPrefab: Instantiates a prefab and finds the target component.\n\n" +
             "FromNewComponentOnNewGameObject: Creates a new GameObject and adds the component.\n\n" +
             "ManualRegistration: You must call RuntimeProxy.RegisterInstance() at runtime.")
        ]
        private ResolveMethod resolveMethod;

        [SerializeField, Tooltip("The prefab from which to get the concrete instance.")]
        private GameObject prefab;

        [SerializeField, Tooltip("Do not destroy the target Object when loading a new Scene.")]
        private bool dontDestroyOnLoad = true;

        private bool hadInstanceBefore;

        private enum ResolveMethod
        {
            FromGlobalScope,
            FindInLoadedScenes,
            FromComponentOnPrefab,
            FromNewComponentOnNewGameObject,
            ManualRegistration
        }

        /// <summary>
        /// Manually register an instance for this proxy.
        /// Only required for <see cref="ResolveMethod.ManualRegistration" /> resolve method.
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
                ResolveMethod.FromGlobalScope => GetFromGlobalScope(),
                ResolveMethod.ManualRegistration => instance,
                ResolveMethod.FromComponentOnPrefab => GetFromPrefab(),
                ResolveMethod.FromNewComponentOnNewGameObject => CreateNewInstanceOnNewGameObject(),
                ResolveMethod.FindInLoadedScenes => FindObjectInAnyScene(),
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

        /// <summary>
        /// Called when the proxy switches from one concrete target instance to another.
        /// Generated proxies should override this to clear or replay event subscriptions.
        /// </summary>
        protected virtual void OnTargetInstanceLost()
        {
            if (!hadInstanceBefore)
                return;

            if (eventSubscriberCount <= 0)
                return;

            Debug.LogWarning
            (
                $"Saneject: '{GetType().Name}' target lost. " +
                $"The previous instance had {eventSubscriberCount} event " +
                $"{(eventSubscriberCount == 1 ? "subscriber" : "subscribers")}. " +
                "Event subscriptions are instance-bound and were not transferred. " +
                "Re-subscribe via this proxy if needed."
            );

            eventSubscriberCount = 0;
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