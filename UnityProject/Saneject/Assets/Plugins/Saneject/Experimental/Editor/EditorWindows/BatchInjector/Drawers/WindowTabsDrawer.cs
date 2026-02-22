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
            BatchInjectorData data,
            ReorderableList reorderableSceneList,
            Rect sceneListRect,
            ReorderableList reorderablePrefabList,
            Rect prefabListRect,
            ref bool clickedAnyListItem,
            Action repaint)
        {
            // Tab controls
            data.windowTab = (WindowTab)GUILayout.Toolbar
            (
                selected: (int)data.windowTab,
                texts: new[]
                {
                    $"Scenes ({data.sceneList.TotalCount})",
                    $"Prefabs ({data.prefabList.TotalCount})"
                }
            );

            GUILayout.Space(8);

            // Tab content
            switch (data.windowTab)
            {
                case WindowTab.Scenes:
                {
                    DrawTab
                    (
                        title: "Scenes",
                        data: data,
                        assetList: data.sceneList,
                        reorderableList: reorderableSceneList,
                        listRect: sceneListRect,
                        clickedListAnyItem: ref clickedAnyListItem,
                        repaint: repaint,
                        buttons: new (string, Action)[]
                        {
                            (
                                "Add Open Scenes",
                                () => SceneListUtility.AddOpenScenes(data)
                            ),
                            (
                                "Add All Project Scenes",
                                () => SceneListUtility.AddAllProjectScenes(data)
                            ),
                            (
                                "Clear All",
                                () => SceneListUtility.ClearScenes(data)
                            ),
                            (
                                $"Sort: {data.sceneList.SortMode.GetDisplayString()}",
                                () => SortMenuDrawer.DrawSortMenu(data, data.sceneList, repaint)
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
                        data: data,
                        assetList: data.prefabList,
                        reorderableList: reorderablePrefabList,
                        listRect: prefabListRect,
                        clickedListAnyItem: ref clickedAnyListItem,
                        repaint: repaint,
                        buttons: new (string, Action)[]
                        {
                            (
                                "Add All Prefabs In Current Scene",
                                () => PrefabListUtility.AddAllPrefabsInScene(data)
                            ),
                            (
                                "Add All Project Prefabs",
                                () => PrefabListUtility.AddAllProjectPrefabs(data)
                            ),
                            (
                                "Clear All",
                                () => PrefabListUtility.ClearPrefabs(data)
                            ),
                            (
                                $"Sort: {data.prefabList.SortMode.GetDisplayString()}",
                                () => SortMenuDrawer.DrawSortMenu(data, data.prefabList, repaint)
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
            BatchInjectorData data,
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
                list: reorderableList,
                assetList: assetList,
                tab: data.windowTab,
                rect: listRect,
                onModified: () => data.isDirty = true
            );
        }
    }
}