using System;
using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.Pipeline;
using Plugins.Saneject.Experimental.Editor.Utilities;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities
{
    public static class InjectionUtility
    {
        public static void Inject( 
            AssetData[] sceneAssets,
            AssetData[] prefabAssets,
            Action onInjectionComplete = null)
        {
            sceneAssets ??= Array.Empty<AssetData>();
            prefabAssets ??= Array.Empty<AssetData>();

            if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(sceneAssets.Length, prefabAssets.Length))
                return;

            Dictionary<AssetData, BatchItem> map = new();
            List<BatchItem> batchItems = new();

            foreach (AssetData assetData in sceneAssets)
            {
                SceneBatchItem batchItem = new(assetData.GetAssetPath(), ContextWalkFilter.All);
                batchItems.Add(batchItem);
                map.Add(assetData, batchItem);
            }

            foreach (AssetData assetData in prefabAssets)
            {
                PrefabBatchItem batchItem = new(assetData.GetAssetPath());
                batchItems.Add(batchItem);
                map.Add(assetData, batchItem);
            }

            InjectionRunner.RunBatch(batchItems.ToArray());

            foreach ((AssetData asset, BatchItem item) in map)
                asset.Status = item.Status;

            onInjectionComplete?.Invoke();
        }
    }
} 