using System;
using System.Text;
using Plugins.Saneject.Runtime.Scopes;

namespace Plugins.Saneject.Runtime.Bindings
{
    public static class BindingIdentityHelper
    {
        public static string GetBindingIdentity(this Binding binding)
        {
            return GetBindingIdentity(
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

        public static string GetBindingIdentity(
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
            sb.Append("[");

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

            sb.Append($" | Scope: {scope.GetType().Name}");
            sb.Append("]");

            return sb.ToString();
        }

        public static string GetPartialBindingIdentity(
            bool isCollection,
            Type interfaceType,
            Type concreteType,
            string id,
            Scope scope)
        {
            StringBuilder sb = new();
            sb.Append("[");

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

            sb.Append($" | Scope: {scope.GetType().Name}");
            sb.Append("]");

            return sb.ToString();
        }
    }
}