using System.ComponentModel;
using Plugins.Saneject.Experimental.Editor.EditorWindows;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SettingsMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + 400;

        private const int Priority_Group_Default = Priority_Base + 0;
        private const int Priority_Item_ShowSettings = Priority_Group_Default + 1;

        #endregion
        
        #region Menu item methods

        [MenuItem("Saneject/Settings", false, Priority_Item_ShowSettings)]
        private static void ShowSettings()
        {
            SettingsEditorWindow editorWindow = EditorWindow.GetWindow<SettingsEditorWindow>();
            editorWindow.titleContent = new GUIContent("Saneject Settings");
            editorWindow.Show();
        }

        #endregion
    }
}
