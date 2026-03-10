using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class BatchInjectMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + MenuPriority.Section * 1 + 10;

        private const int Priority_Group_SelectedAssets = Priority_Base + MenuPriority.Group * 0;
        private const int Priority_Item_Inject_SelectedAssets_BatchInjection = Priority_Group_SelectedAssets + 1;

        #endregion
        
        #region Menu item methods

        [MenuItem("Assets/Saneject/Batch Inject/Selected Assets (All Contexts)", false, Priority_Item_Inject_SelectedAssets_BatchInjection),
         MenuItem("Saneject/Batch Inject/Selected Assets (All Contexts)", false, Priority_Item_Inject_SelectedAssets_BatchInjection)]
        private static void BatchInject_SelectedAssets(MenuCommand cmd)
        {
            InjectionUtility.BatchInjectSelectedAssets();
        }

        #endregion

        #region Validation methods

        [MenuItem("Assets/Saneject/Batch Inject/Selected Assets (All Contexts)", true),
         MenuItem("Saneject/Batch Inject/Selected Assets (All Contexts)", true)]
        private static bool Validate_BatchInject_SelectedAssets()
        {
            return Selection
                .GetFiltered<Object>(SelectionMode.DeepAssets)
                .Any(x => x is GameObject or SceneAsset);
        }

        #endregion
    }
}