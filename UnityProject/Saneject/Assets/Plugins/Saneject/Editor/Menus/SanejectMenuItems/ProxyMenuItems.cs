using System.ComponentModel;
using Plugins.Saneject.Editor.Proxy;
using UnityEditor;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Editor.Menus.SanejectMenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ProxyMenuItems
    {
        #region Menu item methods

        [MenuItem("Saneject/Runtime Proxy/Generate Missing Proxy Scripts", false, SanejectMenuPriority.RuntimeProxy.GenerateMissingProxyScripts)]
        private static void GenerateMissingProxyScripts()
        {
            RuntimeProxyScriptGenerator.GenerateMissingProxyScripts();
        }
        
        [MenuItem("Saneject/Runtime Proxy/Clean Up Unused Scripts And Assets", false, SanejectMenuPriority.RuntimeProxy.CleanUpUnusedScriptsAndAssets)]
        private static void CleanUpUnusedScriptsAndAssets()
        {
            RuntimeProxyCleaner.CleanUnusedScriptsAndAssets();
        }

        #endregion
    }
}
