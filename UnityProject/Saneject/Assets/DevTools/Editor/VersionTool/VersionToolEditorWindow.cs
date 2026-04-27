using System.IO;
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
                   (currentEvent.keyCode == KeyCode.Return || currentEvent.keyCode == KeyCode.KeypadEnter);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Version");
            GUI.SetNextControlName("ReleaseVersion");
            version = EditorGUILayout.TextField(version);

            using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(version)))
            {
                if (GUILayout.Button("Set Version And Export Package") || IsEnterPressed())
                    SetVersionAndExportUnityPackage();
            }

            if (shouldFocusVersion)
            {
                shouldFocusVersion = false;
                EditorGUI.FocusTextInControl("ReleaseVersion");
            }
        }

        private void SetVersionAndExportUnityPackage()
        {
            string releaseVersion = version.Trim();

            if (string.IsNullOrWhiteSpace(releaseVersion))
                return;

            string defaultDirectory = Directory.GetParent(Application.dataPath)?.FullName ?? Application.dataPath;
            string packagePath = EditorUtility.SaveFilePanel(
                "Export Unity Package",
                defaultDirectory,
                $"Saneject-{releaseVersion}.unitypackage",
                "unitypackage");

            if (string.IsNullOrWhiteSpace(packagePath))
                return;

            VersionToolUtility.SetVersionAndExportUnityPackage(releaseVersion, packagePath);
            Close();
        }
    }
}
