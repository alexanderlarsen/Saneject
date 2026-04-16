using System.ComponentModel;
using Plugins.Saneject.Editor.Inspectors.Models;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Inspectors
{
    /// <summary>
    /// Default inspector for <see cref="MonoBehaviour" /> that displays interface-based serialized fields in the correct order.
    /// Roslyn source generators emit interface backing fields in a partial class, so by default they appear at the end of the Inspector.
    /// This inspector places those backing fields next to their corresponding interface fields.
    /// You can safely remove this class and call <see cref="SanejectInspector" />
    /// in your own inspector to preserve correct field ordering.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), CustomEditor(typeof(MonoBehaviour), true, isFallback = false), CanEditMultipleObjects]
    public class MonoBehaviourInspector : UnityEditor.Editor
    {
        private ComponentModel componentModel;

        private void OnEnable()
        {
            componentModel = new ComponentModel(target, serializedObject);
        }

        public override void OnInspectorGUI()
        {
            SanejectInspector.OnInspectorGUI(componentModel);
        }
    }
}