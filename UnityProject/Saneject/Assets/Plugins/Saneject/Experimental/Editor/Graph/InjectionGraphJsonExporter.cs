using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Graph
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
                ["name"] = $"{node.Transform.name} (Instance ID: {node.Transform.GetInstanceID()})",
                ["parent"] = node.Parent != null
                    ? $"{node.Parent.Transform.name} (Instance ID: {node.Parent.Transform.GetInstanceID()})"
                    : null,
                ["context"] = $"{node.Context.Type} (Key: {node.Context.Key})",
                ["scope"] = node.Scope != null
                    ? new JObject
                    {
                        ["type"] = node.Scope?.Type.Name,
                        ["parentScope"] = node.Scope?.ParentScope?.Type.Name,
                        ["componentBindings"] = new JArray(node.Scope?.ComponentBindings?.Select(binding => new JObject
                        {
                            ["interfaceType"] = binding.InterfaceType?.Name,
                            ["concreteType"] = binding.ConcreteType?.Name,
                            ["directInstancesToResolveFrom"] = new JArray(binding.ResolveFromInstances.Select(instance => instance.GetType().Name)),
                            ["idQualifiers"] = new JArray(binding.IdQualifiers),
                            ["injectionTargetTypeQualifiers"] = new JArray(binding.TargetTypeQualifiers.Select(q => q.Name)),
                            ["injectionTargetMemberNameQualifiers"] = new JArray(binding.MemberNameQualifiers),
                            ["searchOrigin"] = binding.SearchOrigin.ToString(),
                            ["searchDirection"] = binding.SearchDirection.ToString(),
                            ["findObjectsInactive"] = binding.FindObjectsInactive.ToString(),
                            ["findObjectsSortMode"] = binding.FindObjectsSortMode.ToString(),
                            ["customTargetTransform"] = binding.CustomTargetTransform?.name,
                            ["includeSelfInSearch"] = binding.IncludeSelfInSearch,
                            ["childIndexForSearch"] = binding.ChildIndexForSearch,
                            ["resolveFromProxy"] = binding.ResolveFromProxy
                        }) ?? Array.Empty<JObject>()),
                        ["assetBindings"] = new JArray(node.Scope?.AssetBindings?.Select(binding => new JObject
                        {
                            ["interfaceType"] = binding.InterfaceType?.Name,
                            ["concreteType"] = binding.ConcreteType?.Name,
                            ["directInstancesToResolveFrom"] = new JArray(binding.ResolveFromInstances.Select(instance => instance.GetType().Name)),
                            ["idQualifiers"] = new JArray(binding.IdQualifiers),
                            ["injectionTargetTypeQualifiers"] = new JArray(binding.TargetTypeQualifiers.Select(q => q.Name)),
                            ["injectionTargetMemberNameQualifiers"] = new JArray(binding.MemberNameQualifiers),
                            ["assetPath"] = binding.AssetPath,
                            ["assetLoadType"] = binding.AssetLoadType.ToString()
                        }) ?? Array.Empty<JObject>()),
                        ["globalBindings"] = new JArray(node.Scope?.GlobalBindings?.Select(binding => new JObject
                        {
                            ["interfaceType"] = binding.InterfaceType?.Name,
                            ["concreteType"] = binding.ConcreteType?.Name,
                            ["directInstancesToResolveFrom"] = new JArray(binding.ResolveFromInstances.Select(instance => instance.GetType().Name)),
                            ["idQualifiers"] = new JArray(binding.IdQualifiers),
                            ["injectionTargetTypeQualifiers"] = new JArray(binding.TargetTypeQualifiers.Select(q => q.Name)),
                            ["injectionTargetMemberNameQualifiers"] = new JArray(binding.MemberNameQualifiers),
                            ["searchOrigin"] = binding.SearchOrigin.ToString(),
                            ["searchDirection"] = binding.SearchDirection.ToString(),
                            ["findObjectsInactive"] = binding.FindObjectsInactive.ToString(),
                            ["findObjectsSortMode"] = binding.FindObjectsSortMode.ToString(),
                            ["customTargetTransform"] = binding.CustomTargetTransform?.name,
                            ["includeSelfInSearch"] = binding.IncludeSelfInSearch,
                            ["childIndexForSearch"] = binding.ChildIndexForSearch,
                            ["resolveFromProxy"] = binding.ResolveFromProxy
                        }) ?? Array.Empty<JObject>())
                    }
                    : null,
                ["components"] = new JArray(node.Components.Select(componentNode => new JObject
                    {
                        ["name"] = $"{componentNode.Component.GetType().Name} (Instance ID: {componentNode.Component.GetInstanceID()})",
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