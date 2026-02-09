using System.IO;
using Plugins.Saneject.Experimental.Editor.Utilities;
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
            string namespaceLine = ProjectSettings.GenerateScopeNamespaceFromFolder
                ? NamespaceUtility.GetNamespaceFromFullPath(Path.GetDirectoryName(filePath))
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