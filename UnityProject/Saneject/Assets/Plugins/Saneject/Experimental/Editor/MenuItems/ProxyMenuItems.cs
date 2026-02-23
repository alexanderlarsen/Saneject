using Plugins.Saneject.Experimental.Editor.Proxy;
using UnityEditor;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class ProxyMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + MenuPriority.Section * 4;

        private const int Priority_Group_Default = Priority_Base + MenuPriority.Group * 0;
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