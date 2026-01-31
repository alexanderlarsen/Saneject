using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Utilities
{
    public static class EditorLayoutUtility
    {
        public static bool PersistentFoldout(
            string text,
            string tooltip,
            bool defaultFoldoutState,
            string prefsKey)
        {
            bool isFoldedOut = EditorPrefs.GetBool(prefsKey, defaultFoldoutState);

            isFoldedOut = EditorGUILayout.Foldout
            (
                foldout: isFoldedOut,
                content: new GUIContent(
                    text,
                    tooltip
                ),
                toggleOnLabelClick: true
            );

            EditorPrefs.SetBool(prefsKey, isFoldedOut);
            return isFoldedOut;
        }
    }
}