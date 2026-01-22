using Plugins.Saneject.Experimental.Editor.EditorWindows;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public class SettingsMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + MenuPriority.Section * 4;

        private const int Priority_Group_Default = Priority_Base + MenuPriority.Group * 0;
        private const int Priority_Item_ShowSettings = Priority_Group_Default + 1;

        #endregion
        
        #region Menu item methods

        [MenuItem("Saneject/Settings", false, Priority_Item_ShowSettings)]
        private static void ShowSettings()
        {
            UserSettingsEditorWindow editorWindow = EditorWindow.GetWindow<UserSettingsEditorWindow>();
            editorWindow.titleContent = new GUIContent("Saneject User Settings");
            editorWindow.Show();
        }

        #endregion
    }
}