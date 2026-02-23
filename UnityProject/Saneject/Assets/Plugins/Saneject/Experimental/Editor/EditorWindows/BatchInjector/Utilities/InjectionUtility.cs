using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.Pipeline;
using Plugins.Saneject.Experimental.Editor.Utilities;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities
{
    public static class InjectionUtility
    {
        public static void Inject(
            BatchInjectorData batchInjectorData,
            IEnumerable<SceneAssetData> sceneAssets,
            IEnumerable<PrefabAssetData> prefabAssets)
        {
            sceneAssets = sceneAssets?.ToArray() ?? Array.Empty<SceneAssetData>();
            prefabAssets = prefabAssets?.ToArray() ?? Array.Empty<PrefabAssetData>();

            if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInjector_Inject(sceneAssets.Count(), prefabAssets.Count()))
                return;

            Dictionary<AssetData, BatchItem> map = new();
            List<BatchItem> batchItems = new();

            foreach (SceneAssetData sceneAssetData in sceneAssets)
            {
                SceneBatchItem batchItem = new(sceneAssetData.GetAssetPath(), sceneAssetData.ContextWalkFilter);
                batchItems.Add(batchItem);
                map.Add(sceneAssetData, batchItem);
            }

            foreach (PrefabAssetData prefabAssetData in prefabAssets)
            {
                PrefabBatchItem batchItem = new(prefabAssetData.GetAssetPath(), prefabAssetData.ContextWalkFilter);
                batchItems.Add(batchItem);
                map.Add(prefabAssetData, batchItem);
            }

            InjectionRunner.RunBatch(batchItems.ToArray());

            foreach ((AssetData asset, BatchItem item) in map)
                asset.Status = item.Status;

            batchInjectorData.IsDirty = true;
        }
    }
}