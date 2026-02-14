using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Data.Injection;
using Plugins.Saneject.Experimental.Runtime.Proxy;
using Plugins.Saneject.Experimental.Runtime.Scopes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Pipeline
{
    public static class ProxySwapTargetCollector
    {
        public static void CollectSwapTargets(
            InjectionContext context,
            InjectionProgressTracker progressTracker)
        {
            foreach (ScopeNode scopeNode in context.ActiveScopeNodes)
                scopeNode.Scope.ClearProxySwapTargets();

            List<FieldNode> fieldNodes = context
                .ActiveComponentNodes
                .SelectMany(componentNode => componentNode.FieldNodes)
                .ToList();

            progressTracker.BeginSegment(stepCount: fieldNodes.Count);

            foreach (FieldNode fieldNode in fieldNodes)
            {
                progressTracker.UpdateInfoText($"Checking for proxy swap targets: {fieldNode.ShortPath}");

                if (!fieldNode.IsInterface)
                {
                    progressTracker.NextStep();
                    continue;
                }

                object fieldValue = fieldNode.FieldInfo.GetValue(fieldNode.Owner);

                if (fieldValue is RuntimeProxyBase && fieldNode.Owner is IRuntimeProxySwapTarget and Component component)
                {
                    Scope nearestScope = fieldNode.ComponentNode.TransformNode.NearestScopeNode.Scope;
                    nearestScope.AddProxySwapTarget(component);
                    context.RegisterProxySwapTarget(fieldNode.ComponentNode);
                }

                progressTracker.NextStep();
            }
        }
    }
}