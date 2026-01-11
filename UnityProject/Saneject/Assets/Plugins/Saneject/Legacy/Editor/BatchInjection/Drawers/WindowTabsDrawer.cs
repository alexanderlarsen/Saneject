using System;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Data;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Enums;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Persistence;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.Saneject.Legacy.Editor.BatchInjection.Drawers
{
    public static class WindowTabsDrawer
    {
        public static void DrawTabs(
            BatchInjectorData injectorData,
            ReorderableList sceneList,
            Rect sceneListRect,
            ReorderableList prefabList,
            Rect prefabListRect,
            ref bool clickedAnyListItem,
            Action repaint)
        {
            // Tab controls
            injectorData.windowTab = (WindowTab)GUILayout.Toolbar
            (
                selected: (int)injectorData.windowTab,
                texts: new[]
                {
                    $"Scenes ({injectorData.sceneList.TotalCount})",
                    $"Prefabs ({injectorData.prefabList.TotalCount})"
                }
            );

            GUILayout.Space(8);

            // Tab content
            switch (injectorData.windowTab)
            {
                case WindowTab.Scenes:
                    AssetListDrawer.DrawListHeader
                    (
                        injectorData: injectorData,
                        list: injectorData.sceneList,
                        title: "Scenes",
                        buttons: new (string, Action)[]
                        {
                            ("Add Open Scenes", () => SceneListUtils.AddOpenScenes(injectorData)),
                            ("Add All Project Scenes", () => SceneListUtils.AddAllProjectScenes(injectorData)),
                            ("Clear All", () => SceneListUtils.ClearScenes(injectorData))
                        },
                        repaint: repaint
                    );

                    GUILayout.Space(5);
                    injectorData.sceneList.Scroll = EditorGUILayout.BeginScrollView(injectorData.sceneList.Scroll);

                    AssetListDrawer.DrawList
                    (
                        list: sceneList,
                        rect: ref sceneListRect,
                        clickedListAnyItem: ref clickedAnyListItem
                    );

                    ContextMenuDrawer.DrawContextMenu
                    (
                        list: sceneList,
                        assetList: injectorData.sceneList,
                        tab: injectorData.windowTab,
                        rect: sceneListRect,
                        onModified: () => Storage.SaveData(injectorData)
                    );

                    break;

                case WindowTab.Prefabs:
                    AssetListDrawer.DrawListHeader
                    (
                        injectorData: injectorData,
                        list: injectorData.prefabList,
                        title: "Prefabs",
                        buttons: new (string, Action)[]
                        {
                            ("Add All Prefabs In Current Scene", () => PrefabListUtils.AddAllPrefabsInScene(injectorData)),
                            ("Add All Project Prefabs", () => PrefabListUtils.AddAllProjectPrefabs(injectorData)),
                            ("Clear All", () => PrefabListUtils.ClearPrefabs(injectorData))
                        },
                        repaint: repaint
                    );

                    GUILayout.Space(5);
                    injectorData.prefabList.Scroll = EditorGUILayout.BeginScrollView(injectorData.prefabList.Scroll);

                    AssetListDrawer.DrawList
                    (
                        list: prefabList,
                        rect: ref prefabListRect,
                        clickedListAnyItem: ref clickedAnyListItem
                    );

                    ContextMenuDrawer.DrawContextMenu
                    (
                        list: prefabList,
                        assetList: injectorData.prefabList,
                        tab: injectorData.windowTab,
                        rect: prefabListRect,
                        onModified: () => Storage.SaveData(injectorData)
                    );

                    break;
            }

            EditorGUILayout.EndScrollView();
        }
    }
}