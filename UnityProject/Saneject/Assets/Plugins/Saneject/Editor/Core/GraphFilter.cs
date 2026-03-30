using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Data.Graph;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Data.Injection;
using Plugins.Saneject.Editor.Extensions;
using UnityEngine;

namespace Plugins.Saneject.Editor.Core
{
    [EditorBrowsable(EditorBrowsableState.Never)]
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

                case ContextWalkFilter.SameContextsAsSelection:
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

                case ContextWalkFilter.PrefabAssetObjects:
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