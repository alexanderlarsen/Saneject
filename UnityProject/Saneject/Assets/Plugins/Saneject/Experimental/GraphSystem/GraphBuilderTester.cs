using System.IO;
using Plugins.Saneject.Experimental.GraphSystem.Serialization;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem
{
    public class GraphBuilderTester : MonoBehaviour
    {
        public Transform startTransform;

        public void BuildGraphAndSaveToJson()
        {
            const string path = @"E:\Unity\Personal\Saneject\UnityProject\Saneject\Assets\Plugins\Saneject\Experimental\GraphSystem\graph.json";

            string json = GraphJsonSerializer.Serialize
            (
                GraphBuilder.Build(startTransform)
            );

            File.WriteAllText(path, json);
            Debug.Log($"Graph saved to {path}");
        }
    }
}