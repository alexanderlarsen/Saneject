using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Data.Injection;
using Plugins.Saneject.Experimental.Editor.Data.Logging;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;
using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Pipeline
{
    public static class Locator
    {
        public static void LocateDependencyCandidates(
            InjectionContext context,
            BindingNode bindingNode,
            TransformNode injectionTargetNode,
            out Object[] candidates,
            out HashSet<Type> rejectedTypes)
        {
            rejectedTypes = new HashSet<Type>();

            IEnumerable<Object> allCandidates = bindingNode switch
            {
                AssetBindingNode assetBindingNode => LocateAssetCandidates(context, assetBindingNode),
                ComponentBindingNode componentBindingNode => LocateComponentCandidates(context, componentBindingNode, injectionTargetNode),
                _ => throw new ArgumentOutOfRangeException()
            };

            List<Object> validCandidates = new();

            foreach (Object candidate in allCandidates)
            {
                ContextIdentity candidateContext = new(candidate);

                if (!UserSettings.UseContextIsolation || candidateContext.Type == ContextType.Global || candidateContext.Equals(bindingNode.ScopeNode.TransformNode.ContextIdentity))
                    validCandidates.Add(candidate);
                else
                    rejectedTypes.Add(candidate.GetType());
            }

            candidates = validCandidates.ToArray();
        }

        private static IEnumerable<Object> LocateComponentCandidates(
            InjectionContext context,
            ComponentBindingNode bindingNode,
            TransformNode injectionTargetNode)
        {
            if (bindingNode.ResolveFromRuntimeProxy)
            {
                Object proxyAsset = ProxyAssetResolver.Resolve(bindingNode.ConcreteType, context);

                return proxyAsset != null
                    ? new[] { proxyAsset }
                    : Enumerable.Empty<Object>();
            }

            if (bindingNode.SearchOrigin == SearchOrigin.Instance)
                return bindingNode.ResolveFromInstances.OfType<Component>();

            Type targetType = bindingNode.InterfaceType ?? bindingNode.ConcreteType;

            Transform origin = bindingNode.SearchOrigin switch
            {
                SearchOrigin.Scope => bindingNode.ScopeNode.TransformNode.Transform,
                SearchOrigin.Root => bindingNode.ScopeNode.TransformNode.Transform.root,
                SearchOrigin.InjectionTarget => injectionTargetNode.Transform,
                SearchOrigin.CustomTargetTransform => bindingNode.CustomTargetTransform,
                _ => null
            };

            IEnumerable<Component> candidates = bindingNode.SearchDirection switch
            {
                SearchDirection.Self =>
                    origin?
                        .GetComponents(targetType),

                SearchDirection.Parent =>
                    origin?
                        .parent?
                        .GetComponents(targetType),

                SearchDirection.Ancestors =>
                    origin?
                        .GetComponentsInAncestors(targetType, bindingNode.IncludeSelfInSearch),

                SearchDirection.FirstChild =>
                    origin?
                        .GetChild(0)?
                        .GetComponents(targetType),

                SearchDirection.LastChild =>
                    origin?
                        .GetChild(origin.childCount - 1)?
                        .GetComponents(targetType),

                SearchDirection.ChildAtIndex =>
                    origin?
                        .GetChild(bindingNode.ChildIndexForSearch)?
                        .GetComponents(targetType),

                SearchDirection.Descendants =>
                    origin?
                        .GetComponentsInDescendants(targetType, bindingNode.IncludeSelfInSearch),

                SearchDirection.Siblings =>
                    origin?
                        .GetComponentsInSiblings(targetType),

                SearchDirection.Anywhere =>
                    injectionTargetNode.ContextIdentity.Type == ContextType.PrefabAsset
                        ? injectionTargetNode
                            .Transform
                            .root
                            .GetComponentsInChildren(targetType, bindingNode.FindObjectsInactive == FindObjectsInactive.Include)
                        : Object
                            .FindObjectsByType<Component>(bindingNode.FindObjectsInactive, bindingNode.FindObjectsSortMode)
                            .Where(component => targetType.IsAssignableFrom(component.GetType())),

                SearchDirection.None => throw new InvalidOperationException("Search direction must be specified."),

                _ => throw new ArgumentOutOfRangeException()
            };

            if (candidates != null && bindingNode.DependencyFilters.Count > 0)
                try
                {
                    candidates = candidates.Where(component => bindingNode.DependencyFilters.All(f => f.Filter(component))).ToArray();
                }
                catch (Exception e)
                {
                    context.RegisterError(Error.CreateBindingFilterException(bindingNode, e));
                }

            return candidates ?? Enumerable.Empty<Component>();
        }

        private static IEnumerable<Object> LocateAssetCandidates(
            InjectionContext context,
            AssetBindingNode bindingNode)
        {
            Type targetType = bindingNode.InterfaceType ?? bindingNode.ConcreteType;
            string path = bindingNode.Path;

            IEnumerable<Object> candidates = bindingNode.AssetLoadType switch
            {
                AssetLoadType.Resources =>
                    Resources.Load(path, targetType)
                        .ToEnumerable(),

                AssetLoadType.ResourcesAll =>
                    Resources.LoadAll(path, targetType),

                AssetLoadType.AssetLoad =>
                    AssetDatabase.LoadAssetAtPath(path, targetType)
                        .ToEnumerable(),

                AssetLoadType.AssetLoadAll =>
                    AssetDatabase.LoadAllAssetsAtPath(path)
                        .Where(asset => asset.GetType() == targetType),

                AssetLoadType.Folder =>
                    AssetDatabase.FindAssets($"t:{targetType.Name}", new[] { path })
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(assetPath => AssetDatabase.LoadAssetAtPath(assetPath, targetType))
                        .Where(obj => obj != null),

                AssetLoadType.Instance => bindingNode.ResolveFromInstances,

                _ => throw new ArgumentOutOfRangeException()
            };

            if (candidates != null && bindingNode.DependencyFilters.Count > 0)
                try
                {
                    candidates = candidates.Where(asset => bindingNode.DependencyFilters.All(f => f.Filter(asset)));
                }
                catch (Exception e)
                {
                    context.RegisterError(Error.CreateBindingFilterException(bindingNode, e));
                }

            return candidates ?? Enumerable.Empty<Object>();
        }

        private static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            return item != null
                ? new[] { item }
                : Enumerable.Empty<T>();
        }
    }
}