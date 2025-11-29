using System.IO;
using System.Linq;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Menus
{
    public static class CreateScopeMenu
    {
        [MenuItem("Saneject/Create New Scope", false, -10), MenuItem("Assets/Create/Saneject/Create New Scope", false, -10)]
        private static void CreateScope()
        {
            string folderPath = GetSelectedFolder();

            // Ask user for class name
            string filePath = EditorUtility.SaveFilePanel(
                "Create New Scope",
                folderPath,
                "NewScope",
                "cs");

            if (string.IsNullOrEmpty(filePath))
                return;

            string className = Path.GetFileNameWithoutExtension(filePath);

            // Derive namespace if enabled
            string namespaceLine = string.Empty;

            if (UserSettings.GenerateScopeNamespaceFromFolder)
            {
                string relativePath = GetPathRelativeToAssets(Path.GetDirectoryName(filePath));

                if (!string.IsNullOrEmpty(relativePath))
                    namespaceLine = $"namespace {relativePath}";
            }

            string template;

            if (string.IsNullOrEmpty(namespaceLine))
                // No namespace
                template =
                    $@"using Plugins.Saneject.Runtime.Scopes;

public class {className} : Scope
{{
    public override void ConfigureBindings()
    {{
    }}
}}";
            else
                // With namespace
                template =
                    $@"using Plugins.Saneject.Runtime.Scopes;

{namespaceLine}
{{
    public class {className} : Scope
    {{
        public override void ConfigureBindings()
        {{
        }}
    }}
}}";

            File.WriteAllText(filePath, template);
            AssetDatabase.Refresh();
        }

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

            return Path.GetFullPath(path);
        }

        private static string GetPathRelativeToAssets(string fullPath)
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

            return string.Join(".", parts.Where(p => !string.IsNullOrEmpty(p)));
        }
    }
}