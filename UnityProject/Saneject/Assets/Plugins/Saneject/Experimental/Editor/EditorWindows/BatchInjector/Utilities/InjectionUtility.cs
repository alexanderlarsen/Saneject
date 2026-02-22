using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.Pipeline;
using Plugins.Saneject.Experimental.Editor.Utilities;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities
{
    public static class InjectionUtility
    {
        public static void InjectAll(BatchInjectorData data)
        {
            SceneBatchItem[] sceneBatchItems = data
                .sceneList
                .GetEnabled()
                .Select(asset => new SceneBatchItem(asset.Path, ContextWalkFilter.All))
                .ToArray();

            PrefabBatchItem[] prefabBatchItems = data
                .prefabList
                .GetEnabled()
                .Select(asset => new PrefabBatchItem(asset.Path))
                .ToArray();

            if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(sceneBatchItems.Length, prefabBatchItems.Length))
                return;

            List<BatchItem> batchItems = new();
            batchItems.AddRange(sceneBatchItems);
            batchItems.AddRange(prefabBatchItems);
            InjectionRunner.RunBatch(batchItems.ToArray());
        }

        public static void InjectScenes(BatchInjectorData data)
        {
            SceneBatchItem[] sceneBatchItems = data
                .sceneList
                .GetEnabled()
                .Select(asset => new SceneBatchItem(asset.Path, ContextWalkFilter.All))
                .ToArray();

            if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(sceneBatchItems.Length, 0))
                return;
 
            InjectionRunner.RunBatch(sceneBatchItems.ToArray());
        }
        
        public static void InjectPrefabs(BatchInjectorData data)
        {
            PrefabBatchItem[] prefabBatchItems = data
                .prefabList
                .GetEnabled()
                .Select(asset => new PrefabBatchItem(asset.Path))
                .ToArray();

            if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(0, prefabBatchItems.Length))
                return;
 
            InjectionRunner.RunBatch(prefabBatchItems.ToArray());
        }
    }
}