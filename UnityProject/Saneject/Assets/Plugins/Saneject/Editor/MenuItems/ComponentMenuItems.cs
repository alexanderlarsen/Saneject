using System.ComponentModel;
using Plugins.Saneject.Editor.Utilities;
using Plugins.Saneject.Runtime.Scopes;
using UnityEditor;
using Component = UnityEngine.Component;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Editor.MenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ComponentMenuItems
    {
        #region Menu item methods

        [MenuItem("CONTEXT/Scope/Saneject/Filter Logs By Scope Type", false, MenuPriority.ComponentMenu.Filter.LogsByScopeType)]
        private static void FilterLogsByScopeType(MenuCommand cmd)
        {
            Scope scope = (Scope)cmd.context;
            string query = $"Scope: {scope.GetType().Name}";
            ConsoleUtility.SetSearch(query);
        }

        [MenuItem("CONTEXT/MonoBehaviour/Saneject/Filter Logs By Component Path", false, MenuPriority.ComponentMenu.Filter.LogsByComponentPath),
         MenuItem("CONTEXT/Scope/Saneject/Filter Logs By Component Path", false, MenuPriority.ComponentMenu.Filter.LogsByComponentPath)]
        private static void FilterLogsByComponentPath(MenuCommand cmd)
        {
            Component component = (Component)cmd.context;
            string query = PathUtility.GetComponentPath(component);
            ConsoleUtility.SetSearch(query);
        }

        #endregion
    }
}
