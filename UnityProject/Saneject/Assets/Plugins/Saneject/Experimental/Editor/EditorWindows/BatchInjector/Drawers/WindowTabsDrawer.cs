using System;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Extensions;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Drawers
{
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
            // Tab controls
            batchInjectorData.windowTab = (WindowTab)GUILayout.Toolbar
            (
                selected: (int)batchInjectorData.windowTab,
                texts: new[]
                {
                    $"Scenes ({batchInjectorData.sceneList.TotalCount})",
                    $"Prefabs ({batchInjectorData.prefabList.TotalCount})"
                }
            );

            GUILayout.Space(8);

            // Tab content
            switch (batchInjectorData.windowTab)
            {
                case WindowTab.Scenes:
                {
                    DrawTab
                    (
                        title: "Scenes",
                        batchInjectorData: batchInjectorData,
                        assetList: batchInjectorData.sceneList,
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
                                $"Sort: {batchInjectorData.sceneList.SortMode.GetDisplayString()}",
                                () => SortMenuDrawer.DrawSortMenu(batchInjectorData, batchInjectorData.sceneList, repaint)
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
                        assetList: batchInjectorData.prefabList,
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
                                $"Sort: {batchInjectorData.prefabList.SortMode.GetDisplayString()}",
                                () => SortMenuDrawer.DrawSortMenu(batchInjectorData, batchInjectorData.prefabList, repaint)
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
                list: reorderableList,
                assetList: assetList,
                tab: batchInjectorData.windowTab,
                rect: listRect
            );
        }
    }
}