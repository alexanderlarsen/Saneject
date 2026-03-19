using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Utilities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class EditorLayoutUtility
    {
        public static bool PersistentFoldout(
            string text,
            string tooltip,
            bool defaultFoldoutState,
            string prefsKey)
        {
            bool isFoldedOut = EditorPrefs.GetBool(prefsKey, defaultFoldoutState);

            isFoldedOut = EditorGUILayout.BeginFoldoutHeaderGroup(
                isFoldedOut,
                new GUIContent(text, tooltip)
            );

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorPrefs.SetBool(prefsKey, isFoldedOut);
            return isFoldedOut;
        }
    }
}