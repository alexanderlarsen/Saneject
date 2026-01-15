using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Graph.Json
{
    public static class InjectionGraphJsonProjector
    {
        private const string DefaultPath = @"E:\Unity\Personal\Saneject\UnityProject\Saneject\Assets\Dev\JsonOutput\Graph.json";

        public static void SaveToDisk(
            InjectionGraph graph,
            string path = null)
        {
            path ??= DefaultPath;
            DirectoryUtility.EnsureDirectoryExists(path);
            File.WriteAllText(path, BuildInjectionGraphJObject(graph).ToString());
            Debug.Log($"Saneject: Injection graph JSON saved to '{path}'");
        }

        #region Helpers

        private static string GetNameAndInstanceId(Object obj)
        {
            return obj != null
                ? $"{obj.name} (Instance ID: {obj.GetInstanceID()})"
                : null;
        }

        #endregion

        #region Graph/Node to JObject converters

        public static JObject BuildInjectionGraphJObject(InjectionGraph graph)
        {
            return new JObject
            {
                [nameof(InjectionGraph.RootTransformNodes)] = new JArray(graph.RootTransformNodes.Select(BuildTransformNodeJObject))
            };
        }

        private static JObject BuildTransformNodeJObject(TransformNode node)
        {
            return new JObject
            {
                [nameof(TransformNode.ParentTransformNode)] = GetNameAndInstanceId(node.ParentTransformNode?.Transform),
                [nameof(TransformNode.ContextIdentity)] = $"{node.ContextIdentity?.Type} (Key: {node.ContextIdentity?.Key})",
                [nameof(TransformNode.DeclaredScopeNode)] = BuildScopeNodeJObject(node.DeclaredScopeNode),
                [nameof(TransformNode.NearestScopeNode)] = node.NearestScopeNode?.ScopeType?.Name,
                [nameof(TransformNode.Transform)] = GetNameAndInstanceId(node.Transform),
                [nameof(TransformNode.ComponentNodes)] = new JArray(node.ComponentNodes.Select(BuildComponentNodeJObject)),
                [nameof(TransformNode.ChildTransformNodes)] = new JArray(node.ChildTransformNodes.Select(BuildTransformNodeJObject))
            };
        }

        private static JObject BuildComponentNodeJObject(ComponentNode node)
        {
            return new JObject
            {
                [nameof(ComponentNode.Component)] = GetNameAndInstanceId(node.Component),
                [nameof(ComponentNode.FieldNodes)] = new JArray(node.FieldNodes.Select(BuildFieldNodeJObject)),
                [nameof(ComponentNode.MethodNodes)] = new JArray(node.MethodNodes.Select(BuildMethodNodeJObject))
            };
        }

        private static JObject BuildMemberNodeJObject(MemberNode node)
        {
            return new JObject
            {
                [nameof(MemberNode.Owner)] = node.Owner?.GetType().Name,
                [nameof(MemberNode.ComponentNode)] = GetNameAndInstanceId(node.ComponentNode?.Component),
                [nameof(MemberNode.DeclaringType)] = node.DeclaringType?.Name,
                [nameof(MemberNode.QualifyingName)] = node.QualifyingName,
                [nameof(MemberNode.InjectId)] = node.InjectId,
                [nameof(MemberNode.SuppressMissingErrors)] = node.SuppressMissingErrors,
                [nameof(MemberNode.DisplayPath)] = node.DisplayPath
            };
        }

        private static JObject BuildFieldNodeJObject(FieldNode node)
        {
            JObject member = BuildMemberNodeJObject(node);

            member[nameof(FieldNode.FieldType)] = node.FieldType?.Name;
            member[nameof(FieldNode.RequestedType)] = node.RequestedType?.Name;
            member[nameof(FieldNode.TypeShape)] = node.TypeShape.ToString();
            member[nameof(FieldNode.IsCollection)] = node.IsCollection;
            member[nameof(FieldNode.IsInterface)] = node.IsInterface;
            member[nameof(FieldNode.IsPropertyBackingField)] = node.IsPropertyBackingField;

            return member;
        }

        private static JObject BuildMethodNodeJObject(MethodNode node)
        {
            JObject member = BuildMemberNodeJObject(node);

            member[nameof(MethodNode.MethodName)] = node.MethodName;
            member[nameof(MethodNode.ParameterNodes)] = new JArray(node.ParameterNodes.Select(BuildMethodParameterNodeJObject));

            return member;
        }

        private static JObject BuildMethodParameterNodeJObject(MethodParameterNode node)
        {
            return new JObject
            {
                [nameof(MethodParameterNode.ParameterName)] = node.ParameterName,
                [nameof(MethodParameterNode.ParameterType)] = node.ParameterType?.Name,
                [nameof(MethodParameterNode.RequestedType)] = node.RequestedType?.Name,
                [nameof(MethodParameterNode.TypeShape)] = node.TypeShape.ToString(),
                [nameof(MethodParameterNode.IsCollection)] = node.IsCollection,
                [nameof(MethodParameterNode.IsInterface)] = node.IsInterface
            };
        }

        private static JObject BuildScopeNodeJObject(ScopeNode node)
        {
            if (node == null)
                return null;

            return new JObject
            {
                [nameof(ScopeNode.Scope)] = GetNameAndInstanceId(node.Scope),
                [nameof(ScopeNode.TransformNode)] = GetNameAndInstanceId(node.TransformNode?.Transform),
                [nameof(ScopeNode.ParentScopeNode)] = GetNameAndInstanceId(node.ParentScopeNode?.Scope),
                [nameof(ScopeNode.ScopeType)] = node.ScopeType?.Name,
                [nameof(ScopeNode.BindingNodes)] = new JArray(node.BindingNodes.Select(BuildTypedBindingNodeJObject))
            };
        }

        private static JObject BuildTypedBindingNodeJObject(BindingNode node)
        {
            return node switch
            {
                GlobalComponentBindingNode globalComponentBindingNode => BuildGlobalComponentBindingNodeJObject(globalComponentBindingNode),
                ComponentBindingNode componentBindingNode => BuildComponentBindingNodeJObject(componentBindingNode),
                AssetBindingNode assetBindingNode => BuildAssetBindingNodeJObject(assetBindingNode),
                _ => throw new ArgumentException($"Unsupported binding node type: {node.GetType().Name}", nameof(node))
            };
        }

        private static JObject BuildBindingNodeJObject(BindingNode node)
        {
            return new JObject
            {
                [nameof(BindingNode.ScopeNode)] = node.ScopeNode?.ScopeType?.Name,
                [nameof(BindingNode.InterfaceType)] = node.InterfaceType?.Name,
                [nameof(BindingNode.ConcreteType)] = node.ConcreteType?.Name,
                [nameof(BindingNode.IsCollectionBinding)] = node.IsCollectionBinding,
                [nameof(BindingNode.LocatorStrategySpecified)] = node.LocatorStrategySpecified,
                [nameof(BindingNode.DependencyFilters)] = node.DependencyFilters?.Count,
                [nameof(BindingNode.ResolveFromInstances)] = new JArray(node.ResolveFromInstances.Select(GetNameAndInstanceId)),
                [nameof(BindingNode.IdQualifiers)] = new JArray(node.IdQualifiers),
                [nameof(BindingNode.MemberNameQualifiers)] = new JArray(node.MemberNameQualifiers),
                [nameof(BindingNode.TargetTypeQualifiers)] = new JArray(node.TargetTypeQualifiers.Select(t => t.Name))
            };
        }

        private static JObject BuildComponentBindingNodeJObject(ComponentBindingNode node)
        {
            JObject binding = BuildBindingNodeJObject(node);
            binding[nameof(ComponentBindingNode.SearchOrigin)] = node.SearchOrigin.ToString();
            binding[nameof(ComponentBindingNode.SearchDirection)] = node.SearchDirection.ToString();
            binding[nameof(ComponentBindingNode.FindObjectsInactive)] = node.FindObjectsInactive.ToString();
            binding[nameof(ComponentBindingNode.FindObjectsSortMode)] = node.FindObjectsSortMode.ToString();
            binding[nameof(ComponentBindingNode.CustomTargetTransform)] = node.CustomTargetTransform?.name;
            binding[nameof(ComponentBindingNode.IncludeSelfInSearch)] = node.IncludeSelfInSearch;
            binding[nameof(ComponentBindingNode.ChildIndexForSearch)] = node.ChildIndexForSearch;
            binding[nameof(ComponentBindingNode.ResolveFromRuntimeProxy)] = node.ResolveFromRuntimeProxy;
            return binding;
        }

        private static JObject BuildAssetBindingNodeJObject(AssetBindingNode node)
        {
            JObject binding = BuildBindingNodeJObject(node);
            binding[nameof(AssetBindingNode.Path)] = node.Path;
            binding[nameof(AssetBindingNode.AssetLoadType)] = node.AssetLoadType.ToString();
            return binding;
        }

        private static JObject BuildGlobalComponentBindingNodeJObject(GlobalComponentBindingNode node)
        {
            JObject binding = BuildComponentBindingNodeJObject(node);
            binding["IsGlobalComponentBinding"] = true;
            return binding;
        }

        #endregion
    }
}