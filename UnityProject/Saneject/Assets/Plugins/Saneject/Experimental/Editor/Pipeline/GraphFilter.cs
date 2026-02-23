using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Data.Graph;
using Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Data.Injection;
using Plugins.Saneject.Experimental.Editor.Extensions;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Pipeline
{
    public static class GraphFilter
    {
        public static IReadOnlyCollection<TransformNode> ApplyWalkFilter(
            InjectionGraph injectionGraph,
            IEnumerable<Transform> startTransforms,
            ContextWalkFilter contextWalkFilter,
            InjectionProgressTracker progressTracker)
        {
            progressTracker.BeginSegment(stepCount: 1);
            progressTracker.UpdateInfoText($"Filtering graph by: {contextWalkFilter}");

            List<TransformNode> nodes;

            switch (contextWalkFilter)
            {
                case ContextWalkFilter.AllContexts:
                {
                    nodes = injectionGraph
                        .EnumerateAllTransformNodes()
                        .ToList();

                    break;
                }

                case ContextWalkFilter.SameAsSelection:
                {
                    HashSet<ContextIdentity> startIdentities = startTransforms
                        .Select(transform => new ContextIdentity(transform))
                        .ToHashSet();

                    nodes = injectionGraph
                        .EnumerateAllTransformNodes()
                        .Where(node => startIdentities.Contains(node.ContextIdentity))
                        .ToList();

                    break;
                }

                case ContextWalkFilter.SceneObjects:
                {
                    nodes = injectionGraph
                        .EnumerateAllTransformNodes()
                        .Where(node => node.ContextIdentity.Type == ContextType.SceneObject)
                        .ToList();

                    break;
                }

                case ContextWalkFilter.PrefabInstances:
                {
                    nodes = injectionGraph
                        .EnumerateAllTransformNodes()
                        .Where(node => node.ContextIdentity.Type == ContextType.PrefabInstance)
                        .ToList();

                    break;
                }

                case ContextWalkFilter.PrefabAssets:
                {
                    nodes = injectionGraph
                        .EnumerateAllTransformNodes()
                        .Where(node => node.ContextIdentity.Type == ContextType.PrefabAsset)
                        .ToList();

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(contextWalkFilter), contextWalkFilter, "Walk filter type is not supported.");
            }

            progressTracker.NextStep();
            return nodes;
        }
    }
}