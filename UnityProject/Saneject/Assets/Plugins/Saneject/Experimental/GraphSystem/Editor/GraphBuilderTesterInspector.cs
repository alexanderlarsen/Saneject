using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem.Editor
{
    [CustomEditor(typeof(GraphBuilderTester))]
    public class GraphBuilderTesterInspector : UnityEditor.Editor
    {
        private GraphBuilderTester tester;

        private void OnEnable()
        {
            tester = (GraphBuilderTester)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Build Graph and Save To Json"))
                tester.BuildGraphAndSaveToJson();
        }
    }
}