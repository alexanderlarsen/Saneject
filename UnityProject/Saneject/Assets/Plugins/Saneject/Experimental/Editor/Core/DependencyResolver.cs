using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class DependencyResolver
    {
        public static void Resolve(
            InjectionGraph graph,
            List<Error> errors,
            out InjectionPlan injectionPlan)
        {
            injectionPlan = new InjectionPlan();

            foreach (TransformNode transformNode in graph.EnumerateAllTransformNodes())
            {
                foreach (ComponentNode componentNode in transformNode.ComponentNodes)
                {
                    foreach (FieldNode fieldNode in componentNode.FieldNodes)
                    {
                        BindingNode matchingBindingNode = FindMatchingBindingNode
                        (
                            fieldNode,
                            transformNode.NearestScopeNode
                        );

                        if (matchingBindingNode != null)
                        {
                            matchingBindingNode.IsUsed = true;

                            DependencyLocator.LocateDependencies
                            (
                                matchingBindingNode,
                                out IEnumerable<Object> locatedDependencies,
                                out HashSet<Type> rejectedTypes
                            );

                            if (locatedDependencies.Any())
                            {
                            }
                            else
                            {
                                errors.Add(Error.CreateMissingDependencyError
                                (
                                    matchingBindingNode,
                                    fieldNode,
                                    rejectedTypes
                                ));
                            }
                        }
                        else
                        {
                            errors.Add(Error.CreateMissingBindingError(fieldNode));
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