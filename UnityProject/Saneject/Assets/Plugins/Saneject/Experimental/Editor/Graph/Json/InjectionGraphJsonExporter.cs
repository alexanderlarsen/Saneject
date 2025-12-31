using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Utils;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Graph.Json
{
    public static class InjectionGraphJsonExporter
    {
        private const string Path = @"E:\Unity\Personal\Saneject\UnityProject\Saneject\Assets\Plugins\Saneject\Experimental\JsonOutput\Graph.json";

        public static void SaveGraphToJson(InjectionGraph graph)
        {
            DirectoryUtils.EnsureDirectoryExists(Path);
            File.WriteAllText(Path, GetGraphJson(graph));
            Debug.Log($"Saneject: Injection graph saved to '{Path}'");
        }

        public static string GetGraphJson(InjectionGraph graph)
        {
            JObject jGraph = new();
            jGraph["rootNodes"] = new JArray(graph.RootTransformNodes.Select(BuildNode));
            return jGraph.ToString();
        }

        private static JObject BuildNode(TransformNode node)
        {
            JObject jNode = new()
            {
                ["name"] = $"{node.Transform.name} (Instance ID: {node.Transform.GetInstanceID()})",
                ["parent"] = node.ParentTransformNode != null
                    ? $"{node.ParentTransformNode.Transform.name} (Instance ID: {node.ParentTransformNode.Transform.GetInstanceID()})"
                    : null,
                ["context"] = $"{node.ContextNode.ContextType} (Key: {node.ContextNode.ContextKey})",
                ["scope"] = node.DeclaredScopeNode != null
                    ? new JObject
                    {
                        ["type"] = node.DeclaredScopeNode?.Type.Name,
                        ["parentScope"] = node.DeclaredScopeNode?.ParentScopeNode?.Type.Name,
                        ["componentBindings"] = new JArray(node.DeclaredScopeNode?.BindingNodes?.OfType<ComponentBindingNode>().Select(binding => new JObject
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
                        ["assetBindings"] = new JArray(node.DeclaredScopeNode?.BindingNodes?.OfType<AssetBindingNode>().Select(binding => new JObject
                        {
                            ["interfaceType"] = binding.InterfaceType?.Name,
                            ["concreteType"] = binding.ConcreteType?.Name,
                            ["directInstancesToResolveFrom"] = new JArray(binding.ResolveFromInstances.Select(instance => instance.GetType().Name)),
                            ["idQualifiers"] = new JArray(binding.IdQualifiers),
                            ["injectionTargetTypeQualifiers"] = new JArray(binding.TargetTypeQualifiers.Select(q => q.Name)),
                            ["injectionTargetMemberNameQualifiers"] = new JArray(binding.MemberNameQualifiers),
                            ["assetPath"] = binding.Path,
                            ["assetLoadType"] = binding.AssetLoadType.ToString()
                        }) ?? Array.Empty<JObject>()),
                        ["globalBindings"] = new JArray(node.DeclaredScopeNode?.BindingNodes?.OfType<GlobalComponentBindingNode>().Select(binding => new JObject
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
                ["components"] = new JArray(node.ComponentNodes.Select(componentNode => new JObject
                    {
                        ["name"] = $"{componentNode.Component.GetType().Name} (Instance ID: {componentNode.Component.GetInstanceID()})",
                        ["fields"] = new JArray(componentNode.FieldNodes.Select
                        (fieldNode => new JObject
                        {
                            ["name"] = fieldNode.FieldInfo.Name,
                            ["type"] = fieldNode.FieldInfo.FieldType.Name,
                            ["injectId"] = fieldNode.InjectId,
                            ["suppressMissingErrors"] = fieldNode.SuppressMissingErrors,
                            ["isCollection"] = fieldNode.IsCollection
                        })),
                        ["methods"] = new JArray(componentNode.MethodNodes.Select(methodNode => new JObject
                        {
                            ["name"] = methodNode.MethodInfo.Name,
                            ["parameters"] = new JArray(methodNode.MethodInfo.GetParameters().Select(p => p.ParameterType.Name)),
                            ["injectId"] = methodNode.InjectId,
                            ["suppressMissingErrors"] = methodNode.SuppressMissingErrors
                        }))
                    }
                )),
                ["children"] = new JArray(node.ChildTransformNodes.Select(BuildNode))
            };

            return jNode;
        }
    }
}