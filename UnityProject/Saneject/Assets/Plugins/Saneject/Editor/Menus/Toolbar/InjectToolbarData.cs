using System;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Utilities;

namespace Plugins.Saneject.Editor.Menus.Toolbar
{
    public static class InjectToolbarData
    {
        #region Inject scene button

        public const string InjectSceneButtonText = "Inject Scene";
        public const string InjectSceneButtonTooltip = "Injects everything in the current scene, including scene objects and prefab instances.";
        public static readonly Action InjectSceneButtonOnClick = () => InjectionUtility.InjectCurrentScene(ContextWalkFilter.AllContexts);

        #endregion

        #region Inject hierarchy button

        public const string InjectHierarchyButtonText = "Inject Hierarchies";
        public const string InjectHierarchyButtonTooltip = "Inject everything in the selected scene hierarchies, including scene objects and prefab instances.";
        public static readonly Action InjectHierarchyButtonOnClick = () => InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.AllContexts);

        #endregion

        #region Inject prefab button

        public const string InjectPrefabButtonText = "Inject Prefab";
        public const string InjectPrefabButtonTooltip = "Injects everything in the current prefab asset, including prefab asset objects and prefab instances.";
        public static readonly Action InjectPrefabButtonOnClick = () => InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.AllContexts);

        #endregion

        #region Batch inject button

        public const string BatchInjectButtonText = "Batch Inject";
        public const string BatchInjectButtonTooltip = "Batch injects the selected folders, scene assets, and prefab assets, including scene objects, prefab asset objects, and prefab instances.";
        public static readonly Action BatchInjectButtonOnClick = () => InjectionUtility.BatchInjectSelectedAssets(ContextWalkFilter.AllContexts);

        #endregion
    }
}