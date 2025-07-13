using Plugins.Saneject.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Inspectors
{
    /// <summary>
    /// Default inspector for <see cref="MonoBehaviour" /> that displays interface-based serialized fields in the correct order.
    /// Roslyn source generators emit interface backing fields in a partial class, so by default they appear at the end of the Inspector.
    /// This inspector places those backing fields next to their corresponding interface fields.
    /// You can safely remove this class and call <see cref="SanejectInspectorUtils.DrawAllSerializedFields" />
    /// in your own inspector to preserve correct field ordering.
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public class MonoBehaviourInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SanejectInspectorUtils.DrawScriptField(targets, target);
            SanejectInspectorUtils.DrawAllSerializedFields(serializedObject, target);
            serializedObject.ApplyModifiedProperties();
        }
    }
}