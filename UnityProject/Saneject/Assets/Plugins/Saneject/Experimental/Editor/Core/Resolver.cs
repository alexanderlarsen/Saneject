using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class Resolver
    {
        public static void Resolve(InjectionSession session)
        {
            ResolveGlobals(session);

            IEnumerable<ComponentNode> componentNodes = session.Graph
                .EnumerateAllTransformNodes()
                .SelectMany(transformNode => transformNode.ComponentNodes);

            foreach (ComponentNode componentNode in componentNodes)
            {
                foreach (FieldNode fieldNode in componentNode.FieldNodes)
                    ResolveField(fieldNode, session);

                foreach (MethodNode methodNode in componentNode.MethodNodes)
                    ResolveMethod(methodNode, session);
            }
        }

        private static void ResolveGlobals(InjectionSession session)
        {
            IEnumerable<GlobalComponentBindingNode> globalBindings =
                session.Graph
                    .EnumerateAllBindingNodes()
                    .OfType<GlobalComponentBindingNode>()
                    .Where(session.ValidBindings.Contains);

            foreach (GlobalComponentBindingNode binding in globalBindings)
            {
                Locator.LocateDependencies
                (
                    binding,
                    injectionTargetNode: binding.ScopeNode.TransformNode,
                    out IEnumerable<Object> dependencies,
                    out HashSet<Type> rejectedTypes
                );

                List<Object> resolved = dependencies.ToList();

                if (resolved is not { Count: > 0 })
                    session.RegisterError(Error.CreateMissingGlobalDependencyError
                    (
                        binding,
                        rejectedTypes
                    ));

                session.RegisterUsedBinding(binding);
                session.RegisterGlobalDependency(binding.ScopeNode, resolved.FirstOrDefault());
            }
        }

        private static void ResolveField(
            FieldNode fieldNode,
            InjectionSession session)
        {
            BindingNode bindingNode = FindMatchingBindingNode
            (
                fieldNode,
                fieldNode.ComponentNode.TransformNode.NearestScopeNode,
                session
            );

            if (bindingNode != null)
            {
                Locator.LocateDependencies
                (
                    bindingNode,
                    injectionTargetNode: fieldNode.ComponentNode.TransformNode,
                    out IEnumerable<Object> dependencies,
                    out HashSet<Type> rejectedTypes
                );

                Object[] array = dependencies.ToArray();

                if (array is not { Length: > 0 })
                    session.RegisterError(Error.CreateMissingDependencyError
                    (
                        bindingNode,
                        fieldNode,
                        rejectedTypes
                    ));

                session.RegisterUsedBinding(bindingNode);
                session.RegisterFieldDependencies(fieldNode, array);
            }
            else
            {
                session.RegisterError(Error.CreateMissingBindingError(fieldNode));
            }
        }

        private static void ResolveMethod(
            MethodNode methodNode,
            InjectionSession session)
        {
        }

        private static BindingNode FindMatchingBindingNode(
            FieldNode fieldNode,
            ScopeNode currentScope,
            InjectionSession session)
        {
            while (currentScope != null)
            {
                BindingNode binding = currentScope.BindingNodes
                    .Where(b => b is not GlobalComponentBindingNode)
                    .Where(MatchesRequestedType)
                    .Where(MatchesIsCollection)
                    .Where(MatchesTargetTypeQualifiers)
                    .Where(MatchesMemberNameQualifiers)
                    .Where(MatchesIdQualifiers)
                    .FirstOrDefault(session.ValidBindings.Contains);

                if (binding != null)
                    return binding;

                currentScope = currentScope.ParentScopeNode;
            }

            return null;

            bool MatchesRequestedType(BindingNode bindingNode)
            {
                return fieldNode.IsInterface
                    ? bindingNode.InterfaceType == fieldNode.RequestedType
                    : bindingNode.ConcreteType == fieldNode.RequestedType;
            }

            bool MatchesIsCollection(BindingNode bindingNode)
            {
                return bindingNode.IsCollectionBinding == fieldNode.IsCollection;
            }

            bool MatchesTargetTypeQualifiers(BindingNode bindingNode)
            {
                return bindingNode.TargetTypeQualifiers.Count == 0 ||
                       bindingNode.TargetTypeQualifiers.Contains(fieldNode.DeclaringType);
            }

            bool MatchesMemberNameQualifiers(BindingNode bindingNode)
            {
                return bindingNode.MemberNameQualifiers.Count == 0 ||
                       bindingNode.MemberNameQualifiers.Contains(fieldNode.MemberName);
            }

            bool MatchesIdQualifiers(BindingNode bindingNode)
            {
                return bindingNode.IdQualifiers.Count == 0 ||
                       bindingNode.IdQualifiers.Contains(fieldNode.InjectId);
            }
        }
    }
}