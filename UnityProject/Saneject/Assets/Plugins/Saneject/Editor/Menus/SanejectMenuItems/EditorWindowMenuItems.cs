using System.ComponentModel;
using Plugins.Saneject.Editor.EditorWindows;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Editor.Menus.SanejectMenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class EditorWindowMenuItems
    {
        #region Menu item methods

        [MenuItem("Saneject/Open Batch Injector Window", false, SanejectMenuPriority.EditorWindows.ShowBatchInjectorWindow)]
        private static void OpenBatchInjectorWindow()
        {
            BatchInjectorEditorWindow.ShowWindow();
        }

        [MenuItem("Saneject/Settings", false, SanejectMenuPriority.EditorWindows.ShowSettingsWindow)]
        private static void ShowSettings()
        {
            SettingsEditorWindow editorWindow = EditorWindow.GetWindow<SettingsEditorWindow>();
            editorWindow.titleContent = new GUIContent("Saneject Settings");
            editorWindow.Show();
        }

        #endregion
    }
}