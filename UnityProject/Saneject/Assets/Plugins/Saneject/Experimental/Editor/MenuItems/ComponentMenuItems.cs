using Plugins.Saneject.Experimental.Editor.Utilities;
using Plugins.Saneject.Experimental.Runtime.Scopes;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class ComponentMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.ComponentRoot;

        private const int Priority_Group_Filter = Priority_Base + MenuPriority.Group * 0;
        private const int Priority_Item_FilterLogsByScopeType = Priority_Group_Filter + 1;
        private const int Priority_Item_FilterLogsByComponentPath = Priority_Group_Filter + 2;

        #endregion

        #region Menu item methods

        [MenuItem("CONTEXT/Scope/Saneject/Filter Logs By Scope Type", false, Priority_Item_FilterLogsByScopeType)]
        private static void FilterLogsByScopeType(MenuCommand cmd)
        {
            Scope scope = (Scope)cmd.context;
            string query = $"Scope: {scope.GetType().Name}";
            ConsoleUtility.SetSearch(query);
        }

        [MenuItem("CONTEXT/MonoBehaviour/Saneject/Filter Logs By Component Path", false, Priority_Item_FilterLogsByComponentPath),
         MenuItem("CONTEXT/Scope/Saneject/Filter Logs By Component Path", false, Priority_Item_FilterLogsByComponentPath)]
        private static void FilterLogsByComponentPath(MenuCommand cmd)
        {
            Component component = (Component)cmd.context;
            string query = PathUtility.GetComponentPath(component);
            ConsoleUtility.SetSearch(query);
        }

        #endregion
    }
}