using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class Injector
    {
        public static void InjectDependencies(InjectionContext context)
        {
            InjectScopeGlobals(context);
            InjectFieldsAndMethods(context);
        }

        private static void InjectScopeGlobals(InjectionContext context)
        {
            IEnumerable<ScopeNode> allScopes = context.Graph
                .GetAllTransformNodes()
                .Select(scopeNode => scopeNode.DeclaredScopeNode)
                .Where(scopeNode => scopeNode != null);

            foreach (ScopeNode scopeNode in allScopes)
            {
                IEnumerable<Component> globalObjects = context.ScopeGlobalResolutionMap.TryGetValue(scopeNode, out IReadOnlyList<object> objects)
                    ? objects.Cast<Component>()
                    : Enumerable.Empty<Component>();

                scopeNode.Scope.UpdateGlobalComponents(globalObjects);
                EditorUtility.SetDirty(scopeNode.Scope);
            }
        }

        private static void InjectFieldsAndMethods(InjectionContext context)
        {
            IEnumerable<ComponentNode> allComponents = context.Graph
                .GetAllTransformNodes()
                .SelectMany(node => node.ComponentNodes);

            foreach (ComponentNode componentNode in allComponents)
            {
                foreach (FieldNode fieldNode in componentNode.FieldNodes)
                    fieldNode.FieldInfo.SetValue
                    (
                        fieldNode.Owner,
                        context.FieldResolutionMap[fieldNode]
                    );

                foreach (MethodNode methodNode in componentNode.MethodNodes)
                    try
                    {
                        methodNode.MethodInfo.Invoke
                        (
                            methodNode.Owner,
                            context.MethodResolutionMap[methodNode].ToArray()
                        );
                    }
                    catch (Exception e)
                    {
                        context.RegisterError(Error.CreateMethodInvocationException(methodNode, e));
                    }

                EditorUtility.SetDirty(componentNode.Component);
            }
        }
    }
}