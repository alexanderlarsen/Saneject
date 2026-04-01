using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Data.Errors;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Data.Injection;
using UnityEditor;
using Component = UnityEngine.Component;

namespace Plugins.Saneject.Editor.Core
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Injector
    {
        public static void InjectDependencies(
            InjectionContext context,
            InjectionProgressTracker progressTracker)
        {
            InjectScopeGlobals(context, progressTracker);
            InjectFields(context, progressTracker);
            InjectMethods(context, progressTracker);
            SetComponentsDirty(context);
        }

        private static void InjectScopeGlobals(
            InjectionContext context,
            InjectionProgressTracker progressTracker)
        {
            progressTracker.BeginSegment(stepCount: context.ActiveScopeNodes.Count);

            foreach (ScopeNode scopeNode in context.ActiveScopeNodes)
            {
                progressTracker.UpdateInfoText($"Injecting global dependency: {scopeNode.ScopeType.Name}");

                IEnumerable<Component> globalObjects = context
                    .ScopeNodeGlobalResolutionMap
                    .TryGetValue(scopeNode, out IReadOnlyList<object> objects)
                    ? objects.Cast<Component>()
                    : Enumerable.Empty<Component>();

                scopeNode.Scope.UpdateGlobalComponents(globalObjects);
                EditorUtility.SetDirty(scopeNode.Scope);
                progressTracker.NextStep();
            }
        }

        private static void InjectFields(
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
                progressTracker.UpdateInfoText($"Injecting field: {fieldNode.ShortPath}");
                object injectObject = context.FieldNodeResolutionMap[fieldNode];

                fieldNode.FieldInfo.SetValue
                (
                    fieldNode.Owner,
                    injectObject
                );

                progressTracker.NextStep();
            }
        }

        private static void InjectMethods(
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
                progressTracker.UpdateInfoText($"Injecting method: {methodNode.ShortPath}");

                try
                {
                    object[] dependencies = context.MethodNodeResolutionMap[methodNode].ToArray();

                    if (dependencies.All(d => d != null))
                        methodNode.MethodInfo.Invoke
                        (
                            methodNode.Owner,
                            dependencies
                        );
                }
                catch (Exception e)
                {
                    context.RegisterError(new InjectMethodInvocationError(methodNode, e));
                }

                progressTracker.NextStep();
            }
        }

        private static void SetComponentsDirty(InjectionContext context)
        {
            foreach (ComponentNode componentNode in context.ActiveComponentNodes)
                EditorUtility.SetDirty(componentNode.Component);
        }
    }
}