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

            if (count <= 0)
                return;

            string scriptsWord = count == 1 ? "script" : "scripts";

            Debug.LogWarning($"Saneject: {count} proxy {scriptsWord} generated. Unity has recompiled and stopped the injection pass. Click 'Inject' again to complete.");
            SessionState.EraseInt("Saneject.ProxyStubCount");
        }
    }
}