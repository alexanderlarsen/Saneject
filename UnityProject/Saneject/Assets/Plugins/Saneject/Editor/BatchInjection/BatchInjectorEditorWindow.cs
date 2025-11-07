using System;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.Saneject.Editor.BatchInjection
{
    public class BatchInjectorEditorWindow : EditorWindow
    {
        private const float WindowPadding = 5f;

        private BatchInjectorData data = new();
        private ReorderableList sceneList;
        private ReorderableList prefabList;
        private Rect sceneListRect;
        private Rect prefabListRect;
        private bool clickedAnyListItem;
        private GUIStyle titleStyle;

        [MenuItem("Saneject/Batch Injector")]
        public static void ShowWindow()
        {
            BatchInjectorEditorWindow window = GetWindow<BatchInjectorEditorWindow>("Saneject Batch Injector");
            window.minSize = new Vector2(420, 400);
            window.Show();
        }

        private void OnEnable()
        {
            data = Storage.LoadData();

            sceneList = AssetListDrawer.CreateReorderableList(
                data: data,
                assetList: data.sceneList,
                onModified: () => Storage.SaveData(data)
            );

            prefabList = AssetListDrawer.CreateReorderableList(
                data: data,
                assetList: data.prefabList,
                onModified: () => Storage.SaveData(data)
            );
        }

        private void OnDisable()
        {
            Storage.SaveData(data);
        }

        private void OnGUI()
        {
            DragAndDropManager.HandleDragAndDrop(
                dropArea: position,
                data: data,
                sceneList: sceneList,
                prefabList: prefabList,
                repaint: Repaint
            );

            GUILayout.BeginArea(screenRect: new Rect(
                x: WindowPadding,
                y: WindowPadding,
                width: position.width - WindowPadding * 2,
                height: position.height - WindowPadding * 2)
            );

            DrawWindowHeader();
            DrawTabs();
            GUILayout.FlexibleSpace();
            DrawInjectButtons();

            AssetListDrawer.HandleInput(
                clickedAnyListItem: ref clickedAnyListItem,
                tab: data.windowTab,
                sceneList: sceneList,
                prefabList: prefabList,
                repaint: Repaint
            );

            GUILayout.EndArea();
        }

        private void DrawWindowHeader()
        {
            titleStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = EditorStyles.label.normal.textColor
                }
            };

            EditorGUILayout.LabelField("Batch Injector", titleStyle);
            EditorGUILayout.LabelField("Drag and drop scenes and prefabs anywhere in the window to add them to each list. Then click one of the Inject-buttons to inject all selected scenes and/or prefabs in one go.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("The status icons show the result of the last injection run from this window.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(8);
        }

        private void DrawTabs()
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
                    AssetListDrawer.DrawListHeader(
                        data: data,
                        list: data.sceneList,
                        title: "Scenes",
                        buttons: new (string, Action)[]
                        {
                            ("Add Open Scenes", () => SceneListManager.AddOpenScenes(data)),
                            ("Add All Project Scenes", () => SceneListManager.AddAllProjectScenes(data)),
                            ("Clear All", () => SceneListManager.ClearScenes(data))
                        },
                        repaint: Repaint
                    );

                    GUILayout.Space(5);
                    data.sceneList.Scroll = EditorGUILayout.BeginScrollView(data.sceneList.Scroll);
                    AssetListDrawer.DrawList(sceneList, ref sceneListRect, ref clickedAnyListItem);
                    break;

                case WindowTab.Prefabs:
                    AssetListDrawer.DrawListHeader(
                        data: data,
                        list: data.prefabList,
                        title: "Prefabs",
                        buttons: new (string, Action)[]
                        {
                            ("Add All Prefabs In Current Scene", () => PrefabListManager.AddAllPrefabsInScene(data)),
                            ("Add All Project Prefabs", () => PrefabListManager.AddAllProjectPrefabs(data)),
                            ("Clear All", () => PrefabListManager.ClearPrefabs(data))
                        },
                        repaint: Repaint
                    );

                    GUILayout.Space(5);
                    data.prefabList.Scroll = EditorGUILayout.BeginScrollView(data.prefabList.Scroll);
                    AssetListDrawer.DrawList(prefabList, ref prefabListRect, ref clickedAnyListItem);
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawInjectButtons()
        {
            int sceneCount = data.sceneList.EnabledCount;
            int prefabCount = data.prefabList.EnabledCount;

            GUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                // Inject All
                using (new EditorGUI.DisabledScope(sceneCount == 0 && prefabCount == 0))
                {
                    if (GUILayout.Button("Inject All"))
                    {
                        if (!Dialogs.BatchInjection.ConfirmInjectAll(sceneCount, prefabCount))
                            return;

                        if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            return;
                        
                        if (UserSettings.ClearLogsOnInjection)
                            ConsoleUtils.ClearLog();
                        
                        

                        DependencyInjector.BatchInjectAllScenesAndPrefabs(
                            sceneAssets: data.sceneList.GetArray(),
                            prefabAssets: data.prefabList.GetArray()
                        );
                    }
                }

                // Inject Scenes
                using (new EditorGUI.DisabledScope(sceneCount == 0))
                {
                    if (GUILayout.Button($"Inject Scenes ({sceneCount})"))
                    {
                        if (!Dialogs.BatchInjection.ConfirmInjectScene(sceneCount))
                            return;

                        if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            return;
                        
                        if (UserSettings.ClearLogsOnInjection)
                            ConsoleUtils.ClearLog();

                        DependencyInjector.BatchInjectScenes(
                            sceneAssets: data.sceneList.GetArray(),
                            logStats: true
                        );
                    }
                }

                // Inject Prefabs
                using (new EditorGUI.DisabledScope(prefabCount == 0))
                {
                    if (GUILayout.Button($"Inject Prefabs ({prefabCount})"))
                    {
                        if (!Dialogs.BatchInjection.ConfirmInjectPrefab(prefabCount))
                            return;

                        if (UserSettings.ClearLogsOnInjection)
                            ConsoleUtils.ClearLog();

                        DependencyInjector.BatchInjectPrefabs(
                            prefabAssets: data.prefabList.GetArray(),
                            logStats: true
                        );
                    }
                }
            }
        }
    }
}