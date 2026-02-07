using System.IO;
using System.Linq;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class CreateScopeMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + MenuPriority.Section * 0;

        private const int Priority_Group_Current = Priority_Base + MenuPriority.Group * 0;
        private const int Priority_Item_CreateNewScope = Priority_Group_Current + 1;

        #endregion

        #region Menu item methods

        [MenuItem("Saneject/Create New Scope", false, Priority_Item_CreateNewScope),
         MenuItem("Assets/Saneject/Create New Scope", false, Priority_Item_CreateNewScope)]
        private static void CreateNewScope()
        {
            string folderPath = GetSelectedFolder();

            // Ask user for class name
            string filePath = EditorUtility.SaveFilePanel
            (
                title: "Create New Scope",
                directory: folderPath,
                defaultName: "NewScope",
                extension: "cs"
            );

            if (string.IsNullOrEmpty(filePath))
                return;

            string className = Path.GetFileNameWithoutExtension(filePath);

            // Derive namespace if enabled
            string namespaceLine = UserSettings.GenerateScopeNamespaceFromFolder
                ? GetNamespaceFromPath(Path.GetDirectoryName(filePath))
                : string.Empty;

            File.WriteAllText(filePath, GetCodeString(className, namespaceLine));
            AssetDatabase.Refresh();
        }

        #endregion

        #region Helper methods

        private static string GetSelectedFolder()
        {
            string path = "Assets";

            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);

                if (File.Exists(path))
                    path = Path.GetDirectoryName(path);

                break;
            }

            return Path.GetFullPath(path ?? string.Empty);
        }

        private static string GetNamespaceFromPath(string fullPath)
        {
            string assetsPath = Path.GetFullPath(Application.dataPath);
            string relativePath = fullPath.Replace(assetsPath, "").Trim(Path.DirectorySeparatorChar);

            // Convert to dotted namespace
            string[] parts = relativePath.Split(Path.DirectorySeparatorChar);

            for (int i = 0; i < parts.Length; i++)
            {
                string clean = "";

                // keep only letters, digits, underscore
                foreach (char c in parts[i])
                    if (char.IsLetterOrDigit(c) || c == '_')
                        clean += c;

                // if empty after cleaning, skip
                if (string.IsNullOrEmpty(clean))
                {
                    parts[i] = null;
                    continue;
                }

                // prepend underscore if first char is digit
                if (char.IsDigit(clean[0]))
                    clean = "_" + clean;

                parts[i] = clean;
            }

            string path = string.Join(".", parts.Where(p => !string.IsNullOrEmpty(p)));

            return !string.IsNullOrWhiteSpace(path)
                ? $"namespace {path}"
                : null;
        }

        private static string GetCodeString(
            string className,
            string namespaceLine)
        {
            if (string.IsNullOrEmpty(namespaceLine))
                return
$@"using Plugins.Saneject.Experimental.Runtime.Scopes;

public class {className} : Scope
{{
    protected override void DeclareBindings()
    {{
    }}
}}";

            return
$@"using Plugins.Saneject.Experimental.Runtime.Scopes;

{namespaceLine}
{{
    public class {className} : Scope
    {{
        protected override void DeclareBindings()
        {{
        }}
    }}
}}";
        }

        #endregion
    }
}