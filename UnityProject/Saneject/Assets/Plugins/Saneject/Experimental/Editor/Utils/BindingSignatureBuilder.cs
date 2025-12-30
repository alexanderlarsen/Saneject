using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.BindingNodes;
using Plugins.Saneject.Runtime.Extensions;

namespace Plugins.Saneject.Experimental.Editor.Utils
{
    // TODO: Refactor this class

    public static class BindingSignatureBuilder
    {
        /// <summary>
        /// Gets the identity string of a known/declared binding.
        /// </summary>
        public static string GetBindingSignature(BaseBindingNode binding)
        { 
            Type interfaceType = binding.InterfaceType;
            Type concreteType = binding.ConcreteType;
            bool isCollection = binding.IsCollectionBinding;
            IReadOnlyList<string> ids = binding.IdQualifiers;
            IReadOnlyList<Type> targetTypes = binding.TargetTypeQualifiers;
            IReadOnlyList<string> memberNames = binding.MemberNameQualifiers;
            bool isPrefab = binding.ScopeNode.TransformNode.ContextNode.IsPrefab;
            string scopeName = binding.ScopeNode.Type.Name;

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

            switch (binding)
            {
                case GlobalComponentBindingNode:
                {
                    sb.Append(", Global");
                    break;
                }

                case AssetBindingNode:
                {
                    sb.Append(", Asset");
                    break;
                }

                case ComponentBindingNode componentBindingNode:
                {
                    sb.Append(", Component");

                    if (componentBindingNode.ResolveFromProxy)
                        sb.Append(", Proxy");

                    break;
                }
            }

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

            sb.Append($" | {(isPrefab ? "Prefab" : "Scene")} scope: {scopeName}");
            sb.Append("]");

            return sb.ToString();
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
    }
}