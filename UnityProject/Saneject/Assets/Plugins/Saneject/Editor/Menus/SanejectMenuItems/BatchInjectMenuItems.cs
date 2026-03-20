using System.ComponentModel;
using Plugins.Saneject.Editor.Utilities;
using UnityEditor;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Editor.Menus.SanejectMenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class BatchInjectMenuItems
    {
        #region Menu item methods

        [MenuItem("Assets/Saneject/Batch Inject/Selected Assets (All Contexts)", false, SanejectMenuPriority.BatchInject.SelectedAssetsAllContexts),
         MenuItem("Saneject/Batch Inject/Selected Assets (All Contexts)", false, SanejectMenuPriority.BatchInject.SelectedAssetsAllContexts)]
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
