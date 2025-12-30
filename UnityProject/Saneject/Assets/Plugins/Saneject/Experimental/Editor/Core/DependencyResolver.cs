using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Runtime.Settings;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class DependencyResolver
    {
        public static void Resolve(
            InjectionGraph graph,
            out InjectionPlan injectionPlan,
            out IReadOnlyList<DependencyError> dependencyErrors)
        {
            injectionPlan = new InjectionPlan();
            dependencyErrors = null;

            foreach (FieldNode fieldNode in graph.EnumerateAllFieldNodes())
            {
                ScopeNode scopeNode = FindNearestScope(fieldNode);
                BindingNode matchingBindingNode = FindMatchingBindingNode(scopeNode, fieldNode);
            }
        }

        private static BindingNode FindMatchingBindingNode(
            ScopeNode scopeNode,
            FieldNode fieldNode)
        {
            // var matchingBindings = scopeNode.

            return null;
        }

        private static ScopeNode FindNearestScope(FieldNode fieldNode)
        {
            ContextNode fieldContext = fieldNode.ComponentNode.TransformNode.ContextNode;
            TransformNode current = fieldNode.ComponentNode.TransformNode;

            while (current != null)
            {
                ScopeNode scope = current.ScopeNode;

                if (scope != null && (!UserSettings.UseContextIsolation || current.ContextNode == fieldContext))
                    return scope;

                current = current.ParentTransformNode;
            }

            return null;
        }
    }
}