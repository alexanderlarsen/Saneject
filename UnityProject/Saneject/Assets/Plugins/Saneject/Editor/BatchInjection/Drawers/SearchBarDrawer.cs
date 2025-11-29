using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.BatchInjection.Drawers
{
    public static class SearchBarDrawer
    {
        public static void DrawSearchBar(ref string searchQuery)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUI.SetNextControlName("SearchField");
            searchQuery = GUILayout.TextField(searchQuery, GUI.skin.FindStyle("ToolbarSeachTextField"));

            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                searchQuery = "";
                GUI.FocusControl(null);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}