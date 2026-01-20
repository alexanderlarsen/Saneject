using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using UnityEditor;
using UnityEngine;

// ReSharper disable LoopCanBeConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class CreateBatchItemExtensions
    {
        /// <summary>
        /// Creates a sequence of <see cref="BatchItem" />s from a collection of asset paths.
        /// Scene assets are converted to <see cref="SceneBatchItem" /> using the provided
        /// <paramref name="sceneWalkFilter" />, while prefab assets are converted to
        /// <see cref="PrefabBatchItem" />. Unsupported paths are skipped.
        /// </summary>
        /// <param name="paths">
        /// Asset paths to process. Paths that are null, empty, or do not resolve to
        /// supported asset types are ignored.
        /// </param>
        /// <param name="sceneWalkFilter">
        /// The <see cref="ContextWalkFilter" /> to apply to all scene batch items.
        /// This value is ignored for prefab assets.
        /// </param>
        /// <returns>
        /// An enumerable sequence of batch items suitable for passing to
        /// <c>InjectionRunner.RunBatch</c>.
        /// </returns>
        public static IEnumerable<BatchItem> CreateBatchItemsFromPaths(
            IEnumerable<string> paths,
            ContextWalkFilter sceneWalkFilter)
        {
            foreach (string path in paths)
            {
                foreach (BatchItem item in GetBatchItemsFromPath(path, sceneWalkFilter))
                    yield return item;
            }
        }

        /// <summary>
        /// Creates a sequence of <see cref="BatchItem" />s from a collection of Unity objects,
        /// typically coming from editor selection. Objects are resolved to asset paths via
        /// <see cref="AssetDatabase.GetAssetPath(UnityEngine.Object)" />.
        /// </summary>
        /// <param name="objects">
        /// Unity objects to process. Objects that do not resolve to valid asset paths or
        /// supported asset types are ignored.
        /// </param>
        /// <param name="sceneWalkFilter">
        /// The <see cref="ContextWalkFilter" /> to apply to all scene batch items.
        /// This value is ignored for prefab assets.
        /// </param>
        /// <returns>
        /// An enumerable sequence of batch items suitable for passing to
        /// <c>InjectionRunner.RunBatch</c>.
        /// </returns>
        public static IEnumerable<BatchItem> CreateBatchItemsFromObjects(
            this IEnumerable<Object> objects,
            ContextWalkFilter sceneWalkFilter)
        {
            foreach (Object obj in objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                foreach (BatchItem item in GetBatchItemsFromPath(path, sceneWalkFilter))
                    yield return item;
            }
        }

        /// <summary>
        /// Resolves a single asset path into zero or one <see cref="BatchItem" />,
        /// depending on whether the path points to a supported asset type.
        /// </summary>
        /// <param name="path">
        /// Asset path to resolve.
        /// </param>
        /// <param name="sceneContextWalkFilter">
        /// The <see cref="ContextWalkFilter" /> to apply if the path resolves to a scene asset.
        /// </param>
        /// <returns>
        /// A sequence containing a single batch item if the path resolves to a supported
        /// asset type; otherwise, an empty sequence.
        /// </returns>
        private static IEnumerable<BatchItem> GetBatchItemsFromPath(
            string path,
            ContextWalkFilter sceneContextWalkFilter)
        {
            if (string.IsNullOrWhiteSpace(path))
                yield break;

            BatchItem batchItem = AssetDatabase.LoadAssetAtPath<Object>(path) switch
            {
                SceneAsset => new SceneBatchItem(path, sceneContextWalkFilter),
                GameObject => new PrefabBatchItem(path),
                _ => null
            };

            if (batchItem != null)
                yield return batchItem;
        }
    }
}