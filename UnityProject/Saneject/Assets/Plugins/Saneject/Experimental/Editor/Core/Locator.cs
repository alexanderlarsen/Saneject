using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;
using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class Locator
    {
        public static void LocateDependencies(
            BindingNode bindingNode,
            TransformNode injectionTargetNode,
            out IEnumerable<Object> dependencies,
            out HashSet<Type> rejectedTypes)
        {
            rejectedTypes = new HashSet<Type>();

            IEnumerable<Object> allDependencies = bindingNode switch
            {
                AssetBindingNode assetBindingNode => LocateAssetDependencies(assetBindingNode),
                ComponentBindingNode componentBindingNode => LocateComponentDependencies(componentBindingNode, injectionTargetNode),
                _ => throw new ArgumentOutOfRangeException()
            };

            List<Object> validDependencies = new();

            foreach (Object dependency in allDependencies)
            {
                ContextIdentity dependencyContext = new(dependency);

                if (!UserSettings.UseContextIsolation || dependencyContext.Equals(bindingNode.ScopeNode.TransformNode.ContextIdentity))
                    validDependencies.Add(dependency);
                else
                    rejectedTypes.Add(dependency.GetType());
            }

            dependencies = validDependencies;
        }

        private static IEnumerable<Object> LocateComponentDependencies(
            ComponentBindingNode bindingNode,
            TransformNode injectionTargetNode)
        {
            if (bindingNode.ResolveFromProxy)
                return new[] { ResolveProxyAsset(bindingNode.ConcreteType) };

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

            IEnumerable<Component> dependencies = bindingNode.SearchDirection switch
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
                    Object.FindObjectsByType(targetType, bindingNode.FindObjectsInactive, bindingNode.FindObjectsSortMode)
                        .Where(obj => obj.GetType() == targetType)
                        .Cast<Component>(),

                SearchDirection.None => throw new InvalidOperationException("Search direction must be specified."),

                _ => throw new ArgumentOutOfRangeException()
            };

            return dependencies ?? Enumerable.Empty<Component>();
        }
        
        public static Object ResolveProxyAsset(Type targetType)
        {
            Type proxyType = ProxyProcessor.FindProxyStubType(targetType);

            return AssetDatabase
                .FindAssets($"t:{proxyType.Name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath(path, proxyType))
                .FirstOrDefault(asset => asset && asset.GetType() == proxyType);
        }

        private static IEnumerable<Object> LocateAssetDependencies(AssetBindingNode bindingNode)
        {
            Type targetType = bindingNode.InterfaceType ?? bindingNode.ConcreteType;
            string path = bindingNode.Path;

            IEnumerable<Object> dependencies = bindingNode.AssetLoadType switch
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

            return dependencies ?? Enumerable.Empty<Object>();
        }

        private static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            return item != null
                ? new[] { item }
                : Enumerable.Empty<T>();
        }
    }
}