namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    public enum ProxyResolveMethod
    {
        FromGlobalScope,
        FromAnywhereInLoadedScenes,
        FromComponentOnPrefab,
        FromNewComponentOnNewGameObject,
        FromManualRegistration
    }
}