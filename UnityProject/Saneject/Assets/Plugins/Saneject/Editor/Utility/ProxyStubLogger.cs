using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Core
{
    [InitializeOnLoad]
    internal static class ProxyStubLogger
    {
        static ProxyStubLogger()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private static void OnAfterAssemblyReload()
        {
            int count = SessionState.GetInt("Saneject.ProxyStubCount", 0);

            if (count > 0)
            {
                Debug.LogWarning($"Saneject: {count} proxy scripts generated. Unity recompiled scripts, which stopped the injection pass. Click Inject again to complete.");
                SessionState.EraseInt("Saneject.ProxyStubCount");
            }
        }
    }
}