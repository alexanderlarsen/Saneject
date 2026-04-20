using System.ComponentModel;
using Plugins.Saneject.Runtime.Scopes;
using UnityEditor;

namespace Plugins.Saneject.Editor.Lifecycle
{
    [InitializeOnLoad, EditorBrowsable(EditorBrowsableState.Never)]
    internal static class GlobalScopePlayModeReset
    {
        static GlobalScopePlayModeReset()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode)
                return;

            GlobalScope.ClearInternal();
        }
    }
} 