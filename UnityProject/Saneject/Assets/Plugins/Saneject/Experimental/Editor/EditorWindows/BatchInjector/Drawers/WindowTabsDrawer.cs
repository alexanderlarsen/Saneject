using System;
using System.ComponentModel;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Extensions;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Drawers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class WindowTabsDrawer
    {
        public static void DrawTabs(
            BatchInjectorData batchInjectorData,
            ReorderableList reorderableSceneList,
            Rect sceneListRect,
            ReorderableList reorderablePrefabList,
            Rect prefabListRect,
            ref bool clickedAnyListItem,
            Action repaint)
        {
            WindowTab previousTab = batchInjectorData.WindowTab;

            // Tab controls
            batchInjectorData.WindowTab = (WindowTab)GUILayout.Toolbar
            (
                selected: (int)batchInjectorData.WindowTab,
                texts: new[]
                {
                    $"Scenes ({batchInjectorData.SceneList.TotalCount})",
                    $"Prefabs ({batchInjectorData.PrefabList.TotalCount})"
                }
            );

            if (previousTab != batchInjectorData.WindowTab)
                batchInjectorData.IsDirty = true;

            GUILayout.Space(8);

            // Tab content
            switch (batchInjectorData.WindowTab)
            {
                case WindowTab.Scenes:
                {
                    DrawTab
                    (
                        title: "Scenes",
                        batchInjectorData: batchInjectorData,
                        assetList: batchInjectorData.SceneList,
                        reorderableList: reorderableSceneList,
                        listRect: sceneListRect,
                        clickedListAnyItem: ref clickedAnyListItem,
                        repaint: repaint,
                        buttons: new (string, Action)[]
                        {
                            (
                                "Add Open Scenes",
                                () => SceneListUtility.AddOpenScenes(batchInjectorData)
                            ),
                            (
                                "Add All Project Scenes",
                                () => SceneListUtility.AddAllProjectScenes(batchInjectorData)
                            ),
                            (
                                "Clear All",
                                () => SceneListUtility.ClearScenes(batchInjectorData)
                            ),
                            (
                                $"Sort: {batchInjectorData.SceneList.SortMode.GetDisplayString()}",
                                () => SortMenuDrawer.DrawSortMenu(batchInjectorData, batchInjectorData.SceneList, repaint)
                            )
                        }
                    );

                    break;
                }

                case WindowTab.Prefabs:
                {
                    DrawTab
                    (
                        title: "Prefabs",
                        batchInjectorData: batchInjectorData,
                        assetList: batchInjectorData.PrefabList,
                        reorderableList: reorderablePrefabList,
                        listRect: prefabListRect,
                        clickedListAnyItem: ref clickedAnyListItem,
                        repaint: repaint,
                        buttons: new (string, Action)[]
                        {
                            (
                                "Add All Prefabs In Current Scene",
                                () => PrefabListUtility.AddAllPrefabsInScene(batchInjectorData)
                            ),
                            (
                                "Add All Project Prefabs",
                                () => PrefabListUtility.AddAllProjectPrefabs(batchInjectorData)
                            ),
                            (
                                "Clear All",
                                () => PrefabListUtility.ClearPrefabs(batchInjectorData)
                            ),
                            (
                                $"Sort: {batchInjectorData.PrefabList.SortMode.GetDisplayString()}",
                                () => SortMenuDrawer.DrawSortMenu(batchInjectorData, batchInjectorData.PrefabList, repaint)
                            )
                        }
                    );

                    break;
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private static void DrawTab(
            string title,
            BatchInjectorData batchInjectorData,
            AssetList assetList,
            ReorderableList reorderableList,
            Rect listRect,
            ref bool clickedListAnyItem,
            Action repaint,
            (string label, Action onClick)[] buttons)
        {
            AssetListDrawer.DrawListHeader
            (title: title,
                buttons: buttons,
                repaint: repaint
            );

            GUILayout.Space(5);
            assetList.Scroll = EditorGUILayout.BeginScrollView(assetList.Scroll);

            AssetListDrawer.DrawList
            (
                list: reorderableList,
                rect: ref listRect,
                clickedListAnyItem: ref clickedListAnyItem
            );

            ContextMenuDrawer.DrawContextMenu
            (
                batchInjectorData: batchInjectorData,
                reorderableList: reorderableList,
                assetList: assetList,
                tab: batchInjectorData.WindowTab,
                rect: listRect
            );
        }
    }
}