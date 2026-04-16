using System.ComponentModel;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;

namespace Plugins.Saneject.Editor.Utilities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HelpBoxUtility
    {
        public static void DrawHelpBox(
            string message,
            MessageType messageType = MessageType.None,
            bool wide = true)
        {
            if (!UserSettings.ShowHelpBoxes)
                return;

            message += "\n\n💡 These help boxes can be disabled at: 'Saneject/Settings/User Settings/Show Help Boxes'";
            
            EditorGUILayout.HelpBox
            (
                message,
                messageType,
                wide
            );
            
            EditorGUILayout.Space(4);
        }
    }
}