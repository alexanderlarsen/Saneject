using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Bindings.Asset;
using Plugins.Saneject.Runtime.Bindings.Component;
using Plugins.Saneject.Runtime.Proxy;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Utilities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SignatureUtility
    {
        public static string GetBindingSignature(BindingNode binding)
        {
            StringBuilder sb = new();
            sb.Append("[Binding: ");

            sb.Append(binding switch
            {
                GlobalComponentBindingNode => "BindGlobal",
                AssetBindingNode => "BindAsset",
                ComponentBindingNode => "BindComponent",
                _ => throw new ArgumentOutOfRangeException(nameof(binding), binding, null)
            });

            if (binding.IsCollectionBinding)
                sb.Append("s");

            if (binding.InterfaceType != null && binding.ConcreteType != null)
                sb.Append($"<{binding.InterfaceType.Name}, {binding.ConcreteType.Name}>");
            else if (binding.InterfaceType != null && binding.ConcreteType == null)
                sb.Append($"<{binding.InterfaceType.Name}>");
            else if (binding.InterfaceType == null && binding.ConcreteType != null)
                sb.Append($"<{binding.ConcreteType.Name}>");
            else
                sb.Append("<T>");

            sb.Append("()");

            if (binding.IdQualifiers.Count > 0)
                sb.Append($".ToId({string.Join(", ", binding.IdQualifiers.Select(id => $"\"{id}\""))})");

            if (binding.TargetTypeQualifiers.Count > 0)
                sb.Append($".ToTarget<{string.Join(", ", binding.TargetTypeQualifiers.Select(t => t.Name))}>()");

            if (binding.MemberNameQualifiers.Count > 0)
                sb.Append($".ToMember({string.Join(", ", binding.MemberNameQualifiers.Select(memberName => $"\"{memberName}\""))})");

            switch (binding)
            {
                case ComponentBindingNode componentBinding:
                {
                    if (componentBinding.RuntimeProxyConfig != null)
                    {
                        sb.Append(".FromRuntimeProxy()");

                        sb.Append(componentBinding.RuntimeProxyConfig.ResolveMethod switch
                        {
                            RuntimeProxyResolveMethod.FromGlobalScope => ".FromGlobalScope()",
                            RuntimeProxyResolveMethod.FromAnywhereInLoadedScenes => ".FromAnywhereInLoadedScenes()",
                            RuntimeProxyResolveMethod.FromComponentOnPrefab => ".FromComponentOnPrefab()",
                            RuntimeProxyResolveMethod.FromNewComponentOnNewGameObject => ".FromNewComponentOnNewGameObject()",
                            _ => ""
                        });

                        break;
                    }

                    if (componentBinding.SearchOrigin == SearchOrigin.Scene && componentBinding.SearchDirection == SearchDirection.Anywhere)
                    {
                        sb.Append(".FromAnywhere()");
                        break;
                    }

                    if (componentBinding.SearchOrigin == SearchOrigin.CustomTargetTransform)
                    {
                        sb.Append(componentBinding.SearchDirection switch
                        {
                            SearchDirection.Self => $".From({componentBinding.CustomTargetTransform.name})",
                            SearchDirection.Parent => $".FromParentOf({componentBinding.CustomTargetTransform.name})",
                            SearchDirection.Ancestors => $".FromAncestorOf({componentBinding.CustomTargetTransform.name})",
                            SearchDirection.FirstChild => $".FromFirstChildOf({componentBinding.CustomTargetTransform.name})",
                            SearchDirection.LastChild => $".FromLastChildOf({componentBinding.CustomTargetTransform.name})",
                            SearchDirection.ChildAtIndex => $".FromChildWithIndexOf({componentBinding.CustomTargetTransform.name}, {componentBinding.ChildIndexForSearch})",
                            SearchDirection.Descendants => $".FromDescendantOf({componentBinding.CustomTargetTransform.name})",
                            SearchDirection.Siblings => $".FromSiblingOf({componentBinding.CustomTargetTransform.name})",
                            _ => ""
                        });

                        break;
                    }

                    if (!componentBinding.LocatorStrategySpecified)
                        break;

                    sb.Append(componentBinding.SearchOrigin switch
                    {
                        SearchOrigin.Scope => ".From",
                        SearchOrigin.Root => ".FromRoot",
                        SearchOrigin.InjectionTarget => ".FromTarget",
                        _ => ""
                    });

                    sb.Append(componentBinding.SearchDirection switch
                    {
                        SearchDirection.Self => "Self()",
                        SearchDirection.Parent => "Parent()",
                        SearchDirection.Ancestors => "Ancestors()",
                        SearchDirection.FirstChild => "FirstChild()",
                        SearchDirection.LastChild => "LastChild()",
                        SearchDirection.ChildAtIndex => $"ChildAtIndex({componentBinding.ChildIndexForSearch})",
                        SearchDirection.Descendants => "Descendants()",
                        SearchDirection.Siblings => "Siblings()",
                        _ => ""
                    });

                    break;
                }

                case AssetBindingNode assetBinding:
                {
                    if (!assetBinding.LocatorStrategySpecified)
                        break;

                    sb.Append(assetBinding.AssetLoadType switch
                    {
                        AssetLoadType.Resources => $".FromResources(\"{assetBinding.Path}\")",
                        AssetLoadType.ResourcesAll => $".FromResourcesAll(\"{assetBinding.Path}\")",
                        AssetLoadType.AssetLoad => $".FromAssetLoad(\"{assetBinding.Path}\")",
                        AssetLoadType.AssetLoadAll => $".FromAssetLoadAll(\"{assetBinding.Path}\")",
                        AssetLoadType.Folder => $".FromFolder(\"{assetBinding.Path}\")",
                        AssetLoadType.Instance => $".FromInstance(instance: {assetBinding.ResolveFromInstances.FirstOrDefault()?.GetType().Name ?? "null"})",
                        _ => throw new ArgumentOutOfRangeException()
                    });

                    break;
                }
            }

            foreach (DependencyFilter filter in binding.DependencyFilters)
                switch (filter)
                {
                    case ComponentFilter componentFilter:
                    {
                        if (componentFilter.FilterType != ComponentFilterType.None)
                            sb.Append($".{componentFilter.FilterType}(...)");

                        break;
                    }

                    case AssetFilter assetFilter:
                    {
                        if (assetFilter.FilterType != AssetFilterType.None)
                            sb.Append($".{assetFilter.FilterType}(...)");

                        break;
                    }
                }

            sb.Append($" | Scope: {binding.ScopeNode.ScopeType.Name}]");

            return sb.ToString();
        }

        public static string GetHypotheticalBindingSignature(
            Type requestedType,
            bool isCollection,
            ScopeNode scopeNode)
        {
            bool isComponent = typeof(Component).IsAssignableFrom(requestedType);
            bool isAsset = typeof(Object).IsAssignableFrom(requestedType);

            string bindComponent = isCollection
                ? $"BindComponents<{requestedType.Name}>()"
                : $"BindComponent<{requestedType.Name}>()";

            string bindAsset = isCollection
                ? $"BindAssets<{requestedType.Name}>()"
                : $"BindAsset<{requestedType.Name}>()";

            StringBuilder sb = new("[Binding: ");

            if (isComponent)
                sb.Append(bindComponent);
            else if (isAsset)
                sb.Append(bindAsset);
            else
                sb.Append(bindComponent)
                    .Append(" or ")
                    .Append(bindAsset);

            sb.Append(" | Nearest scope: ")
                .Append(scopeNode?.ScopeType?.Name ?? "null")
                .Append(']');

            return sb.ToString();
        }

        public static string GetFieldSignature(FieldNode fieldNode)
        {
            StringBuilder sb = new();
            sb.Append("[");
            sb.Append(fieldNode.IsPropertyBackingField ? "Property" : "Field");
            sb.Append($": {fieldNode.DisplayPath}");

            if (!string.IsNullOrWhiteSpace(fieldNode.InjectId))
            {
                sb.Append(" | Inject ID: ");
                sb.Append(fieldNode.InjectId);
            }

            sb.Append("]");
            return sb.ToString();
        }

        public static string GetMethodSignature(MethodNode methodNode)
        {
            StringBuilder sb = new();
            sb.Append($"[Method: {methodNode.DisplayPath}");

            if (!string.IsNullOrWhiteSpace(methodNode.InjectId))
            {
                sb.Append(" | Inject ID: ");
                sb.Append(methodNode.InjectId);
            }

            sb.Append("]");
            return sb.ToString();
        }

        public static string GetMethodParameterSignature(MethodParameterNode parameterNode)
        {
            StringBuilder sb = new();
            sb.Append($"[Method: {parameterNode.MethodNode.DisplayPath}");

            if (!string.IsNullOrWhiteSpace(parameterNode.MethodNode.InjectId))
            {
                sb.Append(" | Inject ID: ");
                sb.Append(parameterNode.MethodNode.InjectId);
            }

            sb.Append($" | Parameter: {parameterNode.ParameterName}");

            sb.Append("]");
            return sb.ToString();
        }
    }
}