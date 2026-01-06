using System;
using System.Linq;
using System.Text;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;
using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Utils
{
    // TODO: Refactor this class

    public static class SignatureBuilder
    {
        public static string GetBindingSignature(BindingNode binding)
        {
            StringBuilder sb = new();
            sb.Append("<b>");

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
                    if (componentBinding.ResolveFromProxy)
                    {
                        sb.Append(".FromProxy()");
                        break;
                    }

                    if (componentBinding.SearchOrigin == SearchOrigin.Scene && componentBinding.SearchDirection == SearchDirection.Anywhere)
                    {
                        sb.Append($".FromAnywhere(FindObjectsInactive.{componentBinding.FindObjectsInactive}, FindObjectsSortMode.{componentBinding.FindObjectsSortMode})");
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

            if (binding.Filters.Count > 0)
                sb.Append(".Where(...)");

            sb.Append("</b>");
            sb.Append($" [Scope: {binding.ScopeNode.Type.Name}]");

            return sb.ToString();
        }

        public static string GetLossyBindingSignature(
            Type requestedType,
            bool isCollection,
            ScopeNode scopeNode)
        {
            StringBuilder sb = new();

            bool isComponentType = typeof(Component).IsAssignableFrom(requestedType);
            bool isAssetType = typeof(Object).IsAssignableFrom(requestedType);

            sb.Append("<b>");

            if (isComponentType)
                sb.Append("BindComponent");
            else if (isAssetType)
                sb.Append("BindAsset");

            if (isCollection)
                sb.Append("s");

            sb.Append($"<{requestedType.Name}>()</b>");

            if (isComponentType && !isCollection)
                sb.Append($" or <b>BindGlobal<{requestedType.Name}>()</b>");

            sb.Append($" [Nearest scope: {scopeNode.Type.Name}]");

            return sb.ToString();
        }

        public static string GetFieldSignature(FieldNode fieldNode)
        {
            StringBuilder sb = new();
            sb.Append("[Injected ");
            sb.Append(fieldNode.IsPropertyBackingField ? "property" : "field");
            sb.Append($": {fieldNode.DisplayPath}");

            if (!string.IsNullOrWhiteSpace(fieldNode.InjectId))
            {
                sb.Append(" | ID: ");
                sb.Append(fieldNode.InjectId);
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}