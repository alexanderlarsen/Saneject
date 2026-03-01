using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum RuntimeProxyResolveMethod
    {
        FromGlobalScope,
        FromAnywhereInLoadedScenes,
        FromComponentOnPrefab,
        FromNewComponentOnNewGameObject
    }
}