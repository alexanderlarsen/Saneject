using Plugins.Saneject.Legacy.Editor.Inspectors.API;
using Plugins.Saneject.Legacy.Editor.Utility;
using Plugins.Saneject.Legacy.Runtime.Scopes;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Legacy.Editor.Inspectors
{
    /// <summary>
    /// Default inspector for <see cref="MonoBehaviour" /> that displays interface-based serialized fields in the correct order.
    /// Roslyn source generators emit interface backing fields in a partial class, so by default they appear at the end of the Inspector.
    /// This inspector places those backing fields next to their corresponding interface fields.
    /// You can safely remove this class and call <see cref="SanejectInspector" />
    /// in your own inspector to preserve correct field ordering.
    /// </summary>
    
    // [CustomEditor(typeof(MonoBehaviour), true, isFallback = true), CanEditMultipleObjects]
    // public class MonoBehaviourFallbackInspector : UnityEditor.Editor
    // {
    //     public override void OnInspectorGUI()
    //     {
    //         serializedObject.Update();
    //         SanejectInspector.DrawDefault(target, serializedObject);
    //         serializedObject.ApplyModifiedProperties();
    //     }
    //
    //     [MenuItem("CONTEXT/Component/Filter Logs By This Component")]
    //     private static void FilterBindingLogsForComponent(MenuCommand command)
    //     {
    //         Component comp = (Component)command.context;
    //
    //         if (comp is Scope scope)
    //         {
    //             ConsoleUtils.SetSearch($"{scope.GetType().Name}");
    //             return;
    //         }
    //
    //         ConsoleUtils.SetSearch($"{comp.GetComponentPath()}");
    //     }
    // }
}