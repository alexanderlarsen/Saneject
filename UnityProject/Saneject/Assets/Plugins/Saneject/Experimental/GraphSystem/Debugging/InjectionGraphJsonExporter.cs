using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Plugins.Saneject.Experimental.GraphSystem.Data;
using Plugins.Saneject.Experimental.GraphSystem.Data.Nodes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem.Debugging
{
    public static class InjectionGraphJsonExporter
    {
        public static void SaveGraphToJson(
            InjectionGraph graph,
            string path)
        {
            File.WriteAllText(path, GetGraphJson(graph));
            Debug.Log($"Saneject: Injection graph saved to '{path}'");
        }

        public static string GetGraphJson(InjectionGraph graph)
        {
            JObject jGraph = new();
            jGraph["rootNodes"] = new JArray(graph.RootNodes.Select(BuildNode));
            return jGraph.ToString();
        }

        private static JObject BuildNode(TransformNode node)
        {
            JObject jNode = new()
            {
                ["name"] = $"{node.Transform.name} ({node.Transform.GetInstanceID()})",
                ["parent"] = node.Parent != null
                    ? $"{node.Parent.Transform.name} ({node.Parent.Transform.GetInstanceID()})"
                    : null,
                ["context"] = $"{node.Context.Type} ({node.Context.Key})",
                ["scope"] = node.Scope != null
                    ? new JObject
                    {
                        ["type"] = node.Scope?.Type.Name,
                        ["parentScope"] = node.Scope?.ParentScope?.Type.Name
                    }
                    : null,
                ["components"] = new JArray(node.Components.Select(componentNode => new JObject
                    {
                        ["name"] = $"{componentNode.Component.GetType().Name} ({componentNode.Component.GetInstanceID()})",
                        ["fields"] = new JArray(componentNode.Fields.Select
                        (fieldNode => new JObject
                        {
                            ["name"] = fieldNode.FieldInfo.Name,
                            ["type"] = fieldNode.FieldInfo.FieldType.Name,
                            ["injectId"] = fieldNode.InjectId,
                            ["suppressMissingErrors"] = fieldNode.SuppressMissingErrors,
                            ["isCollection"] = fieldNode.IsCollection
                        })),
                        ["methods"] = new JArray(componentNode.Methods.Select(methodNode => new JObject
                        {
                            ["name"] = methodNode.MethodInfo.Name,
                            ["parameters"] = new JArray(methodNode.MethodInfo.GetParameters().Select(p => p.ParameterType.Name)),
                            ["injectId"] = methodNode.InjectId,
                            ["suppressMissingErrors"] = methodNode.SuppressMissingErrors
                        }))
                    }
                )),
                ["children"] = new JArray(node.Children.Select(BuildNode))
            };

            return jNode;
        }
    }
}