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
        protected RuntimeProxyResolveMethod resolveMethod;

        [SerializeField, Tooltip("The prefab from which to get the concrete instance.")]
        protected GameObject prefab;

        [SerializeField, Tooltip("Do not destroy the target Object when loading a new Scene.")]
        protected bool dontDestroyOnLoad = true;

        [SerializeField]
        protected RuntimeProxyInstanceMode instanceMode;

        public abstract object ResolveInstance();

        public void AssignConfig(RuntimeProxyConfig config)
        {
            resolveMethod = config.ResolveMethod;
            prefab = config.Prefab;
            dontDestroyOnLoad = config.DontDestroyOnLoad;
            instanceMode = config.InstanceMode;
        }

        public bool HasConfig(RuntimeProxyConfig config)
        {
            return resolveMethod == config.ResolveMethod &&
                   prefab == config.Prefab &&
                   dontDestroyOnLoad == config.DontDestroyOnLoad &&
                   instanceMode == config.InstanceMode;
        }
    }
}