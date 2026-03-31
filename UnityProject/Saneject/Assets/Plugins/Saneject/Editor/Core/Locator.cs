using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Data.Injection;
using Plugins.Saneject.Editor.Data.Logging;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Runtime.Bindings.Asset;
using Plugins.Saneject.Runtime.Bindings.Component;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Core
{
    [EditorBrowsable(EditorBrowsableState.Never)]
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
                // Injected assets are global and can be injected into any context.
                if (bindingNode is AssetBindingNode)
                {
                    validCandidates.Add(candidate);
                    continue;
                }

                ContextIdentity candidateContext = new(candidate);
                ContextIdentity scopeContext = bindingNode.ScopeNode.TransformNode.ContextIdentity;

                if ((ProjectSettings.UseContextIsolation && candidateContext.Equals(scopeContext)) ||
                    (candidateContext.ContainerType == scopeContext.ContainerType &&
                     candidateContext.ContainerId == scopeContext.ContainerId))
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
            if (bindingNode.RuntimeProxyConfig != null)
            {
                Object proxyAsset = ProxyAssetResolver.Resolve
                (
                    bindingNode.ConcreteType,
                    bindingNode.RuntimeProxyConfig,
                    context
                );

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
                    origin
                        ? origin.GetComponents(targetType)
                        : null,

                SearchDirection.Parent =>
                    origin &&
                    origin.parent
                        ? origin
                            .parent
                            .GetComponents(targetType)
                        : null,

                SearchDirection.Ancestors =>
                    origin
                        ? origin.GetComponentsInAncestors(targetType, bindingNode.IncludeSelfInSearch)
                        : null,

                SearchDirection.FirstChild =>
                    origin &&
                    origin.childCount > 0
                        ? origin
                            .GetChild(0)
                            .GetComponents(targetType)
                        : null,

                SearchDirection.LastChild =>
                    origin &&
                    origin.childCount > 0
                        ? origin
                            .GetChild(origin.childCount - 1)
                            .GetComponents(targetType)
                        : null,

                SearchDirection.ChildAtIndex =>
                    origin &&
                    bindingNode.ChildIndexForSearch >= 0 &&
                    bindingNode.ChildIndexForSearch < origin.childCount
                        ? origin
                            .GetChild(bindingNode.ChildIndexForSearch)
                            .GetComponents(targetType)
                        : null,

                SearchDirection.Descendants =>
                    origin
                        ? origin.GetComponentsInDescendants(targetType, bindingNode.IncludeSelfInSearch)
                        : null,

                SearchDirection.Siblings =>
                    origin
                        ? origin.GetComponentsInSiblings(targetType)
                        : null,

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
                    candidates = null;
                    context.RegisterError(Error.CreateBindingFilterException(bindingNode, e));
                }

            return candidates ?? Enumerable.Empty<Component>();
        }

        private static IEnumerable<Object> LocateAssetCandidates(
            InjectionContext context,
            AssetBindingNode bindingNode)
        {
            string path = bindingNode.Path;

            IEnumerable<Object> candidates = bindingNode.AssetLoadType switch
            {
                AssetLoadType.Resources =>
                    Resources
                        .Load(path, bindingNode.ConcreteType)
                        .ToEnumerable(),

                AssetLoadType.ResourcesAll =>
                    Resources.LoadAll(path, bindingNode.ConcreteType),

                AssetLoadType.AssetLoad =>
                    AssetDatabase
                        .LoadAssetAtPath(path, bindingNode.ConcreteType)
                        .ToEnumerable(),

                AssetLoadType.Folder =>
                    AssetDatabase
                        .FindAssets($"t:{bindingNode.ConcreteType.Name}", new[] { path })
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(assetPath => AssetDatabase.LoadAssetAtPath(assetPath, bindingNode.ConcreteType))
                        .Where(obj => obj != null),

                AssetLoadType.Instance => bindingNode.ResolveFromInstances,

                _ => throw new ArgumentOutOfRangeException()
            };

            if (bindingNode.InterfaceType != null)
                candidates = candidates.Where(asset => bindingNode.InterfaceType.IsAssignableFrom(asset.GetType()));

            if (candidates != null && bindingNode.DependencyFilters.Count > 0)
                try
                {
                    candidates = candidates.Where(asset => bindingNode.DependencyFilters.All(f => f.Filter(asset))).ToArray();
                }
                catch (Exception e)
                {
                    candidates = null;
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