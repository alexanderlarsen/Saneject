using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class DependencyResolver
    {
        public static void Resolve(InjectionSession session)
        {
            foreach (TransformNode transformNode in session.Graph.EnumerateAllTransformNodes())
            {
                foreach (ComponentNode componentNode in transformNode.ComponentNodes)
                {
                    foreach (FieldNode fieldNode in componentNode.FieldNodes)
                    {
                        BindingNode matchingBindingNode = FindMatchingBindingNode
                        (
                            fieldNode,
                            transformNode.NearestScopeNode,
                            session
                        );

                        if (matchingBindingNode != null)
                        {
                            session.MarkBindingUsed(matchingBindingNode);

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
                                session.AddError(Error.CreateMissingDependencyError
                                (
                                    matchingBindingNode,
                                    fieldNode,
                                    rejectedTypes
                                ));
                            }
                        }
                        else
                        {
                            session.AddError(Error.CreateMissingBindingError(fieldNode));
                        }
                    }

                    // Resolve methods
                }
            }
        }

        private static BindingNode FindMatchingBindingNode(
            FieldNode fieldNode,
            ScopeNode currentScope,
            InjectionSession session)
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

                BindingNode binding = matchingBindings.FirstOrDefault();

                if (binding != null && session.ValidBindings.Contains(binding))
                    return binding;

                currentScope = currentScope.ParentScopeNode;
            }

            return null;
        }
    }
}