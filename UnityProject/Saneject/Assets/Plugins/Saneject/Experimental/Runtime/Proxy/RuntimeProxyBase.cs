using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    /// <summary>
    /// Non-generic base class for <see cref="RuntimeProxy{TConcrete}" />.
    /// Required so that Unity Editor scripts can target proxy assets regardless of generic type.
    /// </summary>
    public abstract class RuntimeProxyBase : ScriptableObject
    {
        [SerializeField, Tooltip(
             "Determines how the proxy locates or creates the target instance at runtime.\n\n" +
             "FromGlobalScope: Uses GlobalScopeRegistry. Instance must be registered using RegisterGlobalComponent or RegisterGlobalObject in a scope.\n\n" +
             "FindInLoadedScenes: Finds the first matching component in any loaded scene using FindFirstObjectByType<TConcrete>(FindObjectsInactive.Include).\n\n" +
             "FromComponentOnPrefab: Instantiates a prefab and finds the target component.\n\n" +
             "FromNewComponentOnNewGameObject: Creates a new GameObject and adds the component.\n\n" +
             "ManualRegistration: You must call RuntimeProxy.RegisterInstance() at runtime.")
        ]
        protected ProxyResolveMethod resolveMethod;

        [SerializeField, Tooltip("The prefab from which to get the concrete instance.")]
        protected GameObject prefab;

        [SerializeField, Tooltip("Do not destroy the target Object when loading a new Scene.")]
        protected bool dontDestroyOnLoad = true;

        protected int eventSubscriberCount;
        protected bool hadInstanceBefore;

        public void AssignConfig(RuntimeProxyConfig config)
        {
            resolveMethod = config.ResolveMethod;
            prefab = config.Prefab;
            dontDestroyOnLoad = config.DontDestroyOnLoad;
        }

        public bool HasConfig(RuntimeProxyConfig config)
        {
            return resolveMethod == config.ResolveMethod &&
                   prefab == config.Prefab && 
                   dontDestroyOnLoad == config.DontDestroyOnLoad;
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
                "Event subscriptions are instance-bound and will not be transferred to new instance. " +
                "Re-subscribe via this proxy if needed."
            );

            eventSubscriberCount = 0;
        }
    }
}