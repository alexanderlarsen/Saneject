using System.ComponentModel;
using Plugins.Saneject.Editor.EditorWindows;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Editor.MenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SettingsMenuItems
    {
        #region Menu item methods

        [MenuItem("Saneject/Settings", false, MenuPriority.SanejectMenu.Settings.Show)]
        private static void ShowSettings()
        {
            SettingsEditorWindow editorWindow = EditorWindow.GetWindow<SettingsEditorWindow>();
            editorWindow.titleContent = new GUIContent("Saneject Settings");
            editorWindow.Show();
        }

        #endregion
    }
}
