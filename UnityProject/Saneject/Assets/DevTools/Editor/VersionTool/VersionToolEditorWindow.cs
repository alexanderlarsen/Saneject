using UnityEditor;
using UnityEngine;

namespace DevTools.Editor.VersionTool
{
    public class VersionToolEditorWindow : EditorWindow
    {
        private string version;
        private bool shouldFocusVersion = true;

        private void OnEnable()
        {
            version = VersionToolUtility.GetPackageJsonVersion();
        }

        [MenuItem("Saneject Dev/Version Tool")]
        private static void Open()
        {
            VersionToolEditorWindow window = GetWindow<VersionToolEditorWindow>(true, "Version Tool");
            window.minSize = new Vector2(280, 74);
            window.maxSize = new Vector2(280, 74);
            window.ShowUtility();
        }

        private static bool IsEnterPressed()
        {
            Event currentEvent = Event.current;

            return currentEvent.type == EventType.KeyDown &&
                   currentEvent.keyCode is KeyCode.Return or KeyCode.KeypadEnter;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Version");
            GUI.SetNextControlName("ReleaseVersion");
            version = EditorGUILayout.TextField(version);

            using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(version)))
            {
                if (GUILayout.Button("Set Version And Export Package") || IsEnterPressed())
                {
                    VersionToolUtility.SetVersionAndExportUnityPackage(version);
                    Close();
                }
            }

            if (shouldFocusVersion)
            {
                shouldFocusVersion = false;
                EditorGUI.FocusTextInControl("ReleaseVersion");
            }
        }
    }
}
