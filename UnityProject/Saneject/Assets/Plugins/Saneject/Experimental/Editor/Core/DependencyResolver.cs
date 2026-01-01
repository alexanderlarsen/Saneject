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
                session.MarkBindingUsed(bindingNode);

                DependencyLocator.LocateDependencies
                (
                    bindingNode,
                    injectionTargetNode: fieldNode.ComponentNode.TransformNode,
                    out IEnumerable<Object> dependencies,
                    out HashSet<Type> rejectedTypes
                );

                Object[] array = dependencies.ToArray();

                if (array is not { Length: > 0 })
                    session.AddError(Error.CreateMissingDependencyError
                    (
                        bindingNode,
                        fieldNode,
                        rejectedTypes
                    ));

                session.RegisterFieldDependencies(fieldNode, array);
            }
            else
            {
                session.AddError(Error.CreateMissingBindingError(fieldNode));
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
                    .Where(MatchesCollection)
                    .Where(MatchesTargetType)
                    .Where(MatchesMemberName)
                    .Where(MatchesId)
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

            bool MatchesCollection(BindingNode bindingNode)
            {
                return bindingNode.IsCollectionBinding == fieldNode.IsCollection;
            }

            bool MatchesTargetType(BindingNode bindingNode)
            {
                return bindingNode.TargetTypeQualifiers.Count == 0 ||
                       bindingNode.TargetTypeQualifiers.Contains(fieldNode.DeclaringType);
            }

            bool MatchesMemberName(BindingNode bindingNode)
            {
                return bindingNode.MemberNameQualifiers.Count == 0 ||
                       bindingNode.MemberNameQualifiers.Contains(fieldNode.MemberName);
            }

            bool MatchesId(BindingNode bindingNode)
            {
                return bindingNode.IdQualifiers.Count == 0 ||
                       bindingNode.IdQualifiers.Contains(fieldNode.InjectId);
            }
        }
    }
}