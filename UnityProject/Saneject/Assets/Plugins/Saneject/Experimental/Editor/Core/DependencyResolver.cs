using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class DependencyResolver
    {
        public static void Resolve(
            InjectionGraph graph,
            List<BindingError> bindingErrors,
            out InjectionPlan injectionPlan,
            out List<DependencyError> dependencyErrors)
        {
            injectionPlan = new InjectionPlan();
            dependencyErrors = new List<DependencyError>();

            foreach (TransformNode transformNode in graph.EnumerateAllTransformNodes())
            {
                foreach (ComponentNode componentNode in transformNode.ComponentNodes)
                {
                    foreach (FieldNode fieldNode in componentNode.FieldNodes)
                    {
                        BindingNode match = FindMatchingBindingNode(fieldNode, transformNode.NearestScopeNode);

                        if (match != null)
                        {
                            match.IsUsed = true;
                            // Debug.Log($"Found matching binding for field {fieldNode.MemberName} on type {fieldNode.DeclaringType.Name}");
                        }
                        else
                        {
                            bindingErrors.Add(BindingError.CreateMissingBindingError(fieldNode));
                        }
                    }
                    
                    // Resolve methods
                }
            }
        }

        private static BindingNode FindMatchingBindingNode(
            FieldNode fieldNode,
            ScopeNode currentScope)
        {
            while (currentScope != null)
            {
                List<BindingNode> matchingBindings = currentScope.BindingNodes
                    .Where(binding => binding is not GlobalComponentBindingNode)
                    .Where(binding => binding.InterfaceType == fieldNode.InterfaceType && (binding.ConcreteType == fieldNode.ConcreteType || fieldNode.ConcreteType == null))
                    .Where(binding => binding.IsCollectionBinding == fieldNode.IsCollection)
                    .Where(binding => binding.TargetTypeQualifiers.Count == 0 || binding.TargetTypeQualifiers.Any(type => type == fieldNode.DeclaringType))
                    .Where(binding => binding.MemberNameQualifiers.Count == 0 || binding.MemberNameQualifiers.Any(name => name == fieldNode.MemberName))
                    .Where(binding => binding.IdQualifiers.Count == 0 || binding.IdQualifiers.Any(id => id == fieldNode.InjectId))
                    .ToList();

                if (matchingBindings.Count > 0)
                    return matchingBindings.First();

                currentScope = currentScope.ParentScopeNode;
            }

            return null;
        }
    }
}