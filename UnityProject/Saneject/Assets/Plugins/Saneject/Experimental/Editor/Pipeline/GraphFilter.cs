using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Data.Graph;
using Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Extensions;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Pipeline
{
    public static class GraphFilter
    {
        public static IReadOnlyCollection<TransformNode> ApplyWalkFilter(
            InjectionGraph injectionGraph,
            IEnumerable<Transform> startTransforms,
            ContextWalkFilter contextWalkFilter)
        {
            List<TransformNode> nodes;

            switch (contextWalkFilter)
            {
                case ContextWalkFilter.All:
                {
                    nodes = injectionGraph
                        .EnumerateAllTransformNodes()
                        .ToList();

                    break;
                }

                case ContextWalkFilter.SameAsStartObjects:
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

                case ContextWalkFilter.SceneObject:
                {
                    nodes = injectionGraph
                        .EnumerateAllTransformNodes()
                        .Where(node => node.ContextIdentity.Type == ContextType.SceneObject)
                        .ToList();

                    break;
                }

                case ContextWalkFilter.PrefabInstance:
                {
                    nodes = injectionGraph
                        .EnumerateAllTransformNodes()
                        .Where(node => node.ContextIdentity.Type == ContextType.PrefabInstance)
                        .ToList();

                    break;
                }

                case ContextWalkFilter.PrefabAsset:
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

            return nodes;
        }
    }
}