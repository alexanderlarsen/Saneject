using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.GraphSystem.Bindings;
using Plugins.Saneject.Experimental.GraphSystem.Data;
using Plugins.Saneject.Experimental.GraphSystem.Data.Nodes;

namespace Plugins.Saneject.Experimental.GraphSystem.Editor
{
    public static class InjectionGraphExtensions
    {
        public static IEnumerable<TransformNode> GetAllTransformNodes(this InjectionGraph graph)
        {
            return graph.RootNodes.SelectMany(GetTransformNodesRecursive);

            IEnumerable<TransformNode> GetTransformNodesRecursive(TransformNode node)
            {
                yield return node;

                foreach (TransformNode child in node.Children.SelectMany(GetTransformNodesRecursive))
                    yield return child;
            }
        }

        public static IEnumerable<ScopeNode> GetAllScopeNodes(this InjectionGraph graph)
        {
            return graph.RootNodes.SelectMany(GetScopeNodesRecursive);

            IEnumerable<ScopeNode> GetScopeNodesRecursive(TransformNode node)
            {
                if (node.Scope != null)
                    yield return node.Scope;

                foreach (ScopeNode scope in node.Children.SelectMany(GetScopeNodesRecursive))
                    yield return scope;
            }
        }

        public static IEnumerable<NewBinding> GetAllBindings(this ScopeNode scopeNode)
        {
            foreach (NewComponentBinding componentBinding in scopeNode.ComponentBindings)
                yield return componentBinding;

            foreach (NewAssetBinding assetBinding in scopeNode.AssetBindings)
                yield return assetBinding;

            foreach (NewComponentBinding globalBinding in scopeNode.GlobalBindings)
                yield return globalBinding;
        }

        public static IEnumerable<BindingContext> GetAllBindingContexts(this InjectionGraph graph)
        {
            foreach (TransformNode transform in graph.GetAllTransformNodes())
            {
                if (transform.Scope == null)
                    continue;

                foreach (NewBinding binding in transform.Scope.GetAllBindings())
                    yield return new BindingContext(binding, transform.Transform);
            }
        }
    }
}