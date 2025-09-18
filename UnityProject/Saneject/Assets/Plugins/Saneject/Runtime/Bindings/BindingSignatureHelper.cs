using System;
using System.Text;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Runtime.Scopes;

namespace Plugins.Saneject.Runtime.Bindings
{
    public static class BindingSignatureHelper
    {
        /// <summary>
        /// Gets the identity string of a known/declared binding.
        /// </summary>
        public static string GetBindingSignature(this Binding binding)
        {
            return GetBindingSignature(
                binding.IsGlobal,
                binding.IsAssetBinding,
                binding.IsComponentBinding,
                binding.IsProxyBinding,
                binding.IsCollection,
                binding.InterfaceType,
                binding.ConcreteType,
                binding.Ids,
                binding.Scope,
                binding.GetTargetNames(),
                binding.GetMemberNames()
            );
        }

        /// <summary>
        /// Gets the identity from an expected but unknown/missing binding. Constructed in the DependencyInjector based on field info at the current injection step.
        /// </summary>
        public static string GetPartialBindingSignature(
            bool isCollection,
            Type interfaceType,
            Type concreteType,
            Scope scope)
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
            string[] ids,
            Scope scope,
            string[] targetNames,
            string[] memberNames)
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

            if (ids is { Length: > 0 })
            {
                sb.Append(" | ");
                sb.Append(ids.Length == 1 ? "ID: " : "IDs: ");
                sb.Append(string.Join(", ", ids));
            }

            if (targetNames is { Length: > 0 })
            {
                sb.Append(" | ");
                sb.Append(targetNames.Length == 1 ? "To target: " : "To targets: ");
                sb.Append(string.Join(", ", targetNames));
            }

            if (memberNames is { Length: > 0 })
            {
                sb.Append(" | ");
                sb.Append(memberNames.Length == 1 ? "To member: " : "To members: ");
                sb.Append(string.Join(", ", memberNames));
            }

            sb.Append($" | {(scope.gameObject.IsPrefab() ? "Prefab" : "Scene")} scope: {scope.GetType().Name}");
            sb.Append("]");

            return sb.ToString();
        }
    }
}