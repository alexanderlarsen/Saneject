using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.Graph;
using Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Data.Injection;
using Plugins.Saneject.Experimental.Editor.Data.Logging;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Pipeline
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Resolver
    {
        public static void Resolve(
            InjectionContext context,
            InjectionProgressTracker progressTracker)
        {
            ResolveGlobals(context, progressTracker);
            ResolveFields(context, progressTracker);
            ResolveMethods(context, progressTracker);
        }

        private static void ResolveGlobals(
            InjectionContext context,
            InjectionProgressTracker progressTracker)
        {
            List<GlobalComponentBindingNode> globalBindings = context
                .ValidBindingNodes
                .OfType<GlobalComponentBindingNode>()
                .ToList();

            progressTracker.BeginSegment(stepCount: globalBindings.Count);
            Dictionary<ScopeNode, HashSet<object>> globalMap = new();

            foreach (GlobalComponentBindingNode binding in globalBindings)
            {
                progressTracker.UpdateInfoText($"Resolving global dependency: {binding.ScopeNode.ScopeType.Name}.{binding.ConcreteType}");

                Locator.LocateDependencyCandidates
                (
                    context,
                    binding,
                    injectionTargetNode: binding.ScopeNode.TransformNode,
                    out Object[] candidates,
                    out HashSet<Type> rejectedTypes
                );

                if (candidates is not { Length: > 0 })
                    context.RegisterError(Error.CreateMissingGlobalDependencyError
                    (
                        binding,
                        rejectedTypes
                    ));

                if (!globalMap.TryGetValue(binding.ScopeNode, out HashSet<object> dependencySet))
                {
                    dependencySet = new HashSet<object>();
                    globalMap.Add(binding.ScopeNode, dependencySet);
                }

                object resolved = candidates.FirstOrDefault();
                dependencySet.Add(resolved);
                context.RegisterUsedBinding(binding);
                progressTracker.NextStep();
            }

            foreach ((ScopeNode scopeNode, HashSet<object> dependencies) in globalMap)
                context.RegisterGlobalDependencies(scopeNode, dependencies);
        }

        private static void ResolveFields(
            InjectionContext context,
            InjectionProgressTracker progressTracker)
        {
            List<FieldNode> fieldNodes = context
                .ActiveComponentNodes
                .SelectMany(componentNode => componentNode.FieldNodes)
                .ToList();

            progressTracker.BeginSegment(stepCount: fieldNodes.Count);

            foreach (FieldNode fieldNode in fieldNodes)
            {
                progressTracker.UpdateInfoText($"Resolving field: {fieldNode.ShortPath}");
                ResolveField(fieldNode, context);
                progressTracker.NextStep();
            }
        }

        private static void ResolveMethods(
            InjectionContext context,
            InjectionProgressTracker progressTracker)
        {
            List<MethodNode> methodNodes = context
                .ActiveComponentNodes
                .SelectMany(componentNode => componentNode.MethodNodes)
                .ToList();

            progressTracker.BeginSegment(stepCount: methodNodes.Count);

            foreach (MethodNode methodNode in methodNodes)
            {
                progressTracker.UpdateInfoText($"Resolving method: {methodNode.ShortPath}");
                ResolveMethod(methodNode, context);
                progressTracker.NextStep();
            }
        }

        private static void ResolveField(
            FieldNode fieldNode,
            InjectionContext context)
        {
            BindingNode bindingNode = FindMatchingBindingNode
            (
                currentScope: fieldNode.ComponentNode.TransformNode.NearestScopeNode,
                context: context,
                declaringType: fieldNode.DeclaringType,
                requestedType: fieldNode.RequestedType,
                isInterface: fieldNode.IsInterface,
                isCollection: fieldNode.IsCollection,
                qualifyingMemberName: fieldNode.QualifyingName,
                injectId: fieldNode.InjectId
            );

            object resolved = null;

            if (bindingNode == null)
            {
                context.RegisterError(Error.CreateMissingBindingError(fieldNode));
            }
            else
            {
                Locator.LocateDependencyCandidates
                (
                    context,
                    bindingNode,
                    injectionTargetNode: fieldNode.ComponentNode.TransformNode,
                    out Object[] candidates,
                    out HashSet<Type> rejectedTypes
                );

                if (candidates is not { Length: > 0 })
                    context.RegisterError(Error.CreateMissingDependencyError
                    (
                        bindingNode,
                        fieldNode,
                        rejectedTypes
                    ));

                resolved = ResolveCandidates
                (
                    fieldNode.FieldType,
                    fieldNode.RequestedType,
                    fieldNode.TypeShape,
                    candidates
                );

                context.RegisterUsedBinding(bindingNode);
            }

            context.RegisterFieldDependency(fieldNode, resolved);
        }

        private static void ResolveMethod(
            MethodNode methodNode,
            InjectionContext context)
        {
            List<object> resolvedParameters = new();

            foreach (MethodParameterNode parameterNode in methodNode.ParameterNodes)
            {
                BindingNode bindingNode = FindMatchingBindingNode
                (
                    currentScope: methodNode.ComponentNode.TransformNode.NearestScopeNode,
                    context: context,
                    declaringType: methodNode.DeclaringType,
                    requestedType: parameterNode.RequestedType,
                    isInterface: parameterNode.IsInterface,
                    isCollection: parameterNode.IsCollection,
                    qualifyingMemberName: methodNode.QualifyingName,
                    injectId: methodNode.InjectId
                );

                object resolved = null;

                if (bindingNode == null)
                {
                    context.RegisterError(Error.CreateMissingBindingError(parameterNode));
                }
                else
                {
                    Locator.LocateDependencyCandidates
                    (
                        context,
                        bindingNode,
                        injectionTargetNode: methodNode.ComponentNode.TransformNode,
                        out Object[] candidates,
                        out HashSet<Type> rejectedTypes
                    );

                    if (candidates is not { Length: > 0 })
                        context.RegisterError(Error.CreateMissingDependencyError
                        (
                            bindingNode,
                            parameterNode,
                            rejectedTypes
                        ));

                    resolved = ResolveCandidates
                    (
                        parameterNode.ParameterType,
                        parameterNode.RequestedType,
                        parameterNode.TypeShape,
                        candidates
                    );
                }

                resolvedParameters.Add(resolved);
                context.RegisterUsedBinding(bindingNode);
            }

            context.RegisterMethodDependencies(methodNode, resolvedParameters);
        }

        private static BindingNode FindMatchingBindingNode(
            ScopeNode currentScope,
            InjectionContext context,
            Type declaringType,
            Type requestedType,
            bool isInterface,
            bool isCollection,
            string qualifyingMemberName,
            string injectId)
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
                    .FirstOrDefault(context.ValidBindingNodes.Contains);

                if (binding != null)
                    return binding;

                currentScope = currentScope.ParentScopeNode;
            }

            return null;

            bool MatchesRequestedType(BindingNode bindingNode)
            {
                return isInterface
                    ? bindingNode.InterfaceType == requestedType
                    : bindingNode.ConcreteType == requestedType;
            }

            bool MatchesIsCollection(BindingNode bindingNode)
            {
                return bindingNode.IsCollectionBinding == isCollection;
            }

            bool MatchesTargetTypeQualifiers(BindingNode bindingNode)
            {
                return bindingNode.TargetTypeQualifiers.Count == 0 ||
                       bindingNode.TargetTypeQualifiers.Contains(declaringType);
            }

            bool MatchesMemberNameQualifiers(BindingNode bindingNode)
            {
                return bindingNode.MemberNameQualifiers.Count == 0 ||
                       bindingNode.MemberNameQualifiers.Contains(qualifyingMemberName);
            }

            bool MatchesIdQualifiers(BindingNode bindingNode)
            {
                return bindingNode.IdQualifiers.Count == 0 ||
                       bindingNode.IdQualifiers.Contains(injectId);
            }
        }

        private static object ResolveCandidates(
            Type type,
            Type requestedType,
            TypeShape typeShape,
            Object[] candidates)
        {
            if (candidates is not { Length: > 0 })
                return null;

            switch (typeShape)
            {
                case TypeShape.Single:
                {
                    return candidates.First();
                }

                case TypeShape.Array:
                {
                    Array array = Array.CreateInstance(requestedType, candidates.Length);

                    for (int i = 0; i < candidates.Length; i++)
                        array.SetValue(candidates[i], i);

                    return array;
                }

                case TypeShape.List:
                {
                    IList list = (IList)Activator.CreateInstance(type);

                    foreach (Object d in candidates)
                        list.Add(d);

                    return list;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}