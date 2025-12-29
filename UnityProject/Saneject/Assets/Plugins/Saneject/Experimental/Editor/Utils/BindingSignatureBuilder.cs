using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Runtime.Bindings;
using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;
using Plugins.Saneject.Experimental.Runtime.Bindings.Component;

namespace Plugins.Saneject.Experimental.Editor.Utils
{
    public static class BindingSignatureBuilder
    {
         /// <summary>
        /// Gets the identity string of a known/declared binding.
        /// </summary>
        public static string GetBindingSignature(BaseBinding binding)
        {
            return GetBindingSignature(
                isGlobal: binding is GlobalComponentBinding,
                isAsset: binding is AssetBinding,
                isComponent: binding is ComponentBinding,
                isProxy: binding is ComponentBinding { ResolveFromProxy: true },
                isCollection: binding.IsCollectionBinding,
                interfaceType: binding.InterfaceType,
                concreteType: binding.ConcreteType,
                ids: binding.IdQualifiers,
                // scope: binding.Scope,
                targetTypes: binding.TargetTypeQualifiers,
                memberNames: binding.MemberNameQualifiers
            );
        }

        /// <summary>
        /// Gets the identity from an expected but unknown/missing binding. Constructed in the DependencyInjector based on field info at the current injection step.
        /// </summary>
        public static string GetPartialBindingSignature(
            bool isCollection,
            Type interfaceType,
            Type concreteType,
            ScopeNode scope)
        {
            StringBuilder sb = new();
            sb.Append("[Binding: ");

            if (interfaceType != null && concreteType != null)
                sb.Append($"{interfaceType.Name}/{concreteType.Name}");
            else if (interfaceType == null && concreteType != null)
                sb.Append($"{concreteType.Name}");
            else if (interfaceType != null)
                sb.Append($"{interfaceType.Name}");
            else
                sb.Append("null/null");

            sb.Append(" | ");
            sb.Append(isCollection ? "Collection" : "Single");

            sb.Append($" | Nearest scope: {scope.GetType().Name}");
            sb.Append("]");

            return sb.ToString();
        }

        private static string GetBindingSignature(
            bool isGlobal,
            bool isAsset,
            bool isComponent,
            bool isProxy,
            bool isCollection,
            Type interfaceType,
            Type concreteType,
            IReadOnlyList<string> ids,
            // ScopeNode scope,
            IReadOnlyList<Type> targetTypes,
            IReadOnlyList<string> memberNames)
        {
            StringBuilder sb = new();
            sb.Append("[Binding: ");

            if (interfaceType != null && concreteType != null)
                sb.Append($"{interfaceType.Name}/{concreteType.Name}");
            else if (interfaceType == null && concreteType != null)
                sb.Append($"{concreteType.Name}");
            else if (interfaceType != null)
                sb.Append($"{interfaceType.Name}");
            else
                sb.Append("null/null");

            sb.Append(" | ");
            sb.Append(isCollection ? "Collection" : "Single");

            if (isGlobal)
                sb.Append(", Global");
            else if (isAsset)
                sb.Append(", Asset");
            else if (isComponent)
                sb.Append(", Component");

            if (isProxy)
                sb.Append(", Proxy");

            if (ids is { Count: > 0 })
            {
                sb.Append(" | ");
                sb.Append(ids.Count == 1 ? "ID: " : "IDs: ");
                sb.Append(string.Join(", ", ids));
            }

            if (targetTypes is { Count: > 0 })
            {
                sb.Append(" | ");
                sb.Append(targetTypes.Count == 1 ? "To target: " : "To targets: ");
                sb.Append(string.Join(", ", targetTypes.Select(t => t.Name)));
            }

            if (memberNames is { Count: > 0 })
            {
                sb.Append(" | ");
                sb.Append(memberNames.Count == 1 ? "To member: " : "To members: ");
                sb.Append(string.Join(", ", memberNames));
            }

            // sb.Append($" | {(scope.gameObject.IsPrefab() ? "Prefab" : "Scene")} scope: {scope.GetType().Name}");
            sb.Append("]");

            return sb.ToString();
        }
    }
}