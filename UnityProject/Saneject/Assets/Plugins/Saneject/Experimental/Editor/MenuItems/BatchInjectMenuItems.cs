using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Pipeline;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public class BatchInjectMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + MenuPriority.Section * 1 + 2;
        
        private const int Priority_Group_SelectedAssets = Priority_Base + MenuPriority.Group * 0;
        private const int Priority_Item_Inject_SelectedAssets_BatchInjection = Priority_Group_SelectedAssets + 1;

        #endregion

        #region Menu item methods

        [MenuItem("Assets/Saneject/Batch Inject/Selected Assets", false, Priority_Item_Inject_SelectedAssets_BatchInjection),
         MenuItem("Saneject/Batch Inject/Selected Assets", false, Priority_Item_Inject_SelectedAssets_BatchInjection)]
        private static void BatchInject_SelectedAssets()
        {
            BatchItem[] batchItems = Selection.GetFiltered<Object>(SelectionMode.DeepAssets)
                .CreateBatchItemsFromObjects(ContextWalkFilter.All)
                .ToArray();

            int sceneCount = batchItems.OfType<SceneBatchItem>().Count();
            int prefabCount = batchItems.OfType<PrefabBatchItem>().Count();

            if (!DialogUtility.BatchInjection.Confirm_BatchInject_SelectedAssets(sceneCount, prefabCount))
                return;

            InjectionRunner.RunBatch
            (
                batchItems
            );
        }

        #endregion

        #region Validation methods

        [MenuItem("Assets/Saneject/Batch Inject/Selected Assets", true),
         MenuItem("Saneject/Batch Inject/Selected Assets", true)]
        private static bool Validate_BatchInject_SelectedAssets()
        {
            return Selection
                .GetFiltered<Object>(SelectionMode.DeepAssets)
                .Any(x => x is GameObject or SceneAsset);
        }

        #endregion
    }
}