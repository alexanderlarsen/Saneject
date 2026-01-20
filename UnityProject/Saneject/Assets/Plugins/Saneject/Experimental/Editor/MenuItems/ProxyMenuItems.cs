using Plugins.Saneject.Experimental.Editor.Proxy;
using UnityEditor;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class ProxyMenuItems
    {
        #region Menu item methods

        [MenuItem("Saneject/Runtime Proxy/Generate Missing Proxy Scripts", false, Priority_Item_GenerateMissingProxyScripts)]
        private static void GenerateMissingProxyScripts()
        {
            ProxyScriptGenerator.GenerateMissingProxyScripts();
        }

        #endregion

        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + MenuPriority.Section * 2;

        private const int Priority_Group_Default = Priority_Base + MenuPriority.Group * 0;
        private const int Priority_Item_GenerateMissingProxyScripts = Priority_Group_Default + 1;

        #endregion
    }
}