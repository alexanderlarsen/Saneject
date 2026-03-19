using System.ComponentModel;
using Plugins.Saneject.Editor.Proxy;
using UnityEditor;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Editor.MenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ProxyMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + 300;

        private const int Priority_Group_Default = Priority_Base + 0;
        private const int Priority_Item_GenerateMissingProxyScripts = Priority_Group_Default + 1;
        private const int Priority_Item_CleanUpUnusedScriptsAndAssets = Priority_Group_Default + 2;

        #endregion

        #region Menu item methods

        [MenuItem("Saneject/Runtime Proxy/Generate Missing Proxy Scripts", false, Priority_Item_GenerateMissingProxyScripts)]
        private static void GenerateMissingProxyScripts()
        {
            RuntimeProxyScriptGenerator.GenerateMissingProxyScripts();
        }
        
        [MenuItem("Saneject/Runtime Proxy/Clean Up Unused Scripts And Assets", false, Priority_Item_CleanUpUnusedScriptsAndAssets)]
        private static void CleanUpUnusedScriptsAndAssets()
        {
            RuntimeProxyCleaner.CleanUnusedScriptsAndAssets();
        }

        #endregion
    }
}
