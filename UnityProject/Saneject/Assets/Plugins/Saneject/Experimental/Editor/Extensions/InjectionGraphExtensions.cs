using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.BindingNodes;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class InjectionGraphExtensions
    {
        public static IEnumerable<TransformNode> GetAllTransformNodes(this InjectionGraph graph)
        {
            return graph.RootTransformNodes.SelectMany(GetTransformNodesRecursive);

            IEnumerable<TransformNode> GetTransformNodesRecursive(TransformNode node)
            {
                yield return node;

                foreach (TransformNode child in node.ChildTransformNodes.SelectMany(GetTransformNodesRecursive))
                    yield return child;
            }
        }

        public static IEnumerable<ScopeNode> GetAllScopeNodes(this InjectionGraph graph)
        {
            return graph.RootTransformNodes.SelectMany(GetScopeNodesRecursive);

            IEnumerable<ScopeNode> GetScopeNodesRecursive(TransformNode node)
            {
                if (node.ScopeNode != null)
                    yield return node.ScopeNode;

                foreach (ScopeNode scope in node.ChildTransformNodes.SelectMany(GetScopeNodesRecursive))
                    yield return scope;
            }
        }

        public static IEnumerable<BaseBindingNode> GetAllBindings(this ScopeNode scopeNode)
        {
            foreach (ComponentBindingNode componentBinding in scopeNode.ComponentBindingNodes)
                yield return componentBinding;

            foreach (AssetBindingNode assetBinding in scopeNode.AssetBindingNodes)
                yield return assetBinding;

            foreach (GlobalComponentBindingNode globalBinding in scopeNode.GlobalComponentBindingNodes)
                yield return globalBinding;
        }

        public static IEnumerable<BaseBindingNode> GetAllBindings(this InjectionGraph graph)
        {
            foreach (TransformNode transform in graph.GetAllTransformNodes())
            {
                if (transform.ScopeNode == null)
                    continue;

                foreach (BaseBindingNode binding in transform.ScopeNode.GetAllBindings())
                    yield return binding;
            }
        }

        public static IEnumerable<BaseBindingNode> GetUnusedBindings(this InjectionGraph graph)
        {
            return graph.GetAllBindings().Where(binding => !binding.IsUsed);
        }
    }
}