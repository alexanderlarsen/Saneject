using System.ComponentModel;
using Plugins.Saneject.Editor.Utilities;
using UnityEditor;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Editor.MenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class BatchInjectMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + 115;

        private const int Priority_Group_SelectedAssets = Priority_Base + 0;
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
            return MenuValidator.HasValidBatchSelection();
        }

        #endregion
    }
}
