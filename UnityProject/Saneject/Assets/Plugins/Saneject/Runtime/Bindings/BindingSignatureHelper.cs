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
                binding.Id,
                binding.Scope
            );
        }

        /// <summary>
        /// Gets the identity from an expected but unknown/missing binding. Constructed in the DependencyInjector based on field info at the current injection step.
        /// </summary>
        public static string GetPartialBindingSignature(
            bool isCollection,
            Type interfaceType,
            Type concreteType,
            string id,
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

            if (id != null)
                sb.Append($" | Id: {id} ");

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
            string id,
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

            if (isGlobal)
                sb.Append(", Global");
            else if (isAsset)
                sb.Append(", Asset");
            else if (isComponent)
                sb.Append(", Component");

            if (isProxy)
                sb.Append(", Proxy");

            if (id != null)
                sb.Append($" | Id: {id}");

            sb.Append($" | {(scope.gameObject.IsPrefab() ? "Prefab" : "Scene")} scope: {scope.GetType().Name}");
            sb.Append("]");

            return sb.ToString();
        }
    }
}