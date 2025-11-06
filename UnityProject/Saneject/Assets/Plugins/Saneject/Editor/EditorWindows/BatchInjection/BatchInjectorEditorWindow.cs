using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Editor.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    public class BatchInjectorEditorWindow : EditorWindow
    {
        private const float WindowPadding = 5f;

        private BatchInjectorData data = new();
        private ReorderableList sceneList;
        private ReorderableList prefabList;
        private Rect sceneListRect;
        private Rect prefabListRect;
        private Vector2 listScroll;
        private SelectedTab selectedTab;
        private bool clickedListItemThisFrame;
        private SortMode sceneSortMode = SortMode.Custom;
        private SortMode prefabSortMode = SortMode.Custom;
        private GUIStyle titleStyle;

        private enum SelectedTab
        {
            Scenes,
            Prefabs
        }

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
            sceneList = CreateList(data.scenes);
            prefabList = CreateList(data.prefabs);
        }

        private void OnDisable()
        {
            Storage.SaveData(data);
        }

        private void OnGUI()
        {
            HandleWindowDragAndDrop(position);

            Rect paddedRect = new
            (
                WindowPadding,
                WindowPadding,
                position.width - WindowPadding * 2,
                position.height - WindowPadding * 2
            );

            GUILayout.BeginArea(paddedRect);
            DrawHeader();
            GUILayout.Space(8);

            switch (selectedTab)
            {
                case SelectedTab.Scenes:
                    DrawSceneHeader();
                    GUILayout.Space(5);
                    listScroll = EditorGUILayout.BeginScrollView(listScroll);
                    DrawSceneList();
                    break;

                case SelectedTab.Prefabs:
                    DrawPrefabHeader();
                    GUILayout.Space(5);
                    listScroll = EditorGUILayout.BeginScrollView(listScroll);
                    DrawPrefabList();
                    break;
            }

            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            DrawInjectButtons();
            HandleClearSelection();
            GUILayout.EndArea();
        }

        private ReorderableList CreateList(List<AssetData> source)
        {
            ReorderableList list = new(
                elements: source,
                elementType: typeof(AssetData),
                draggable: true,
                displayHeader: false,
                displayAddButton: false,
                displayRemoveButton: true)
            {
                multiSelect = true
            };

            list.drawElementCallback = (
                rect,
                index,
                _,
                _) =>
            {
                if (index < 0 || index >= source.Count)
                    return;

                AssetData data = source[index];
                Object asset = data.Asset;

                const float toggleWidth = 20f;
                const float objWidth = 220f;

                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                data.Enabled = EditorGUI.Toggle
                (
                    new Rect
                    (
                        rect.x + 4,
                        rect.y,
                        toggleWidth,
                        rect.height
                    ),
                    data.Enabled
                );

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.ObjectField
                    (
                        position: new Rect
                        (
                            rect.x + toggleWidth + 2,
                            rect.y,
                            objWidth,
                            rect.height
                        ),
                        obj: asset,
                        objType: typeof(Object),
                        allowSceneObjects: false
                    );
                }

                string labelText = asset == null
                    ? "(Deleted)"
                    : data.Path;

                EditorGUI.LabelField
                (
                    position: new Rect
                    (
                        rect.x + toggleWidth + objWidth + 7,
                        rect.y,
                        rect.width - objWidth - 40,
                        rect.height
                    ),
                    label: labelText,
                    style: EditorStyles.miniLabel
                );
            };

            list.onRemoveCallback = _ =>
            {
                List<int> indices = list.selectedIndices?
                    .Distinct()
                    .Where(i => i >= 0 && i < source.Count)
                    .OrderByDescending(i => i)
                    .ToList() ?? new List<int> { list.index };

                foreach (int i in indices)
                    source.RemoveAt(i);

                list.ClearSelection();
                list.index = Mathf.Clamp(list.index, 0, source.Count - 1);
                Storage.SaveData(data);
                GUI.changed = true;
            };

            list.onReorderCallback = _ =>
            {
                switch (selectedTab)
                {
                    case SelectedTab.Scenes:
                        sceneSortMode = SortMode.Custom;
                        break;

                    case SelectedTab.Prefabs:
                        prefabSortMode = SortMode.Custom;
                        break;
                }

                Storage.SaveData(data);
            };

            return list;
        }

        private void DrawHeader()
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
            GUILayout.Space(8);

            selectedTab = (SelectedTab)GUILayout.Toolbar
            (
                selected: (int)selectedTab,
                texts: new[]
                {
                    $"Scenes ({data.scenes.Count})",
                    $"Prefabs ({data.prefabs.Count})"
                }
            );
        }

        private void DrawSortMenuButton(
            SortMode sortMode,
            Action<SortMode> setSortMode,
            List<AssetData> list)
        {
            if (!GUILayout.Button($"Sort: {sortMode.GetDisplayString()}"))
                return;

            GenericMenu menu = new();

            AddItem(SortMode.PathAtoZ);
            AddItem(SortMode.PathZtoA);
            menu.AddSeparator("");
            AddItem(SortMode.NameAtoZ);
            AddItem(SortMode.NameZtoA);
            menu.ShowAsContext();

            return;

            void AddItem(SortMode mode)
            {
                menu.AddItem
                (
                    content: new GUIContent(mode.GetDisplayString()),
                    on: sortMode == mode,
                    func: () =>
                    {
                        setSortMode(mode);
                        AssetListSorter.SortList(list, mode);
                        Storage.SaveData(data);
                        Repaint();
                    }
                );
            }
        }

        private void DrawSceneHeader()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Scenes", EditorStyles.boldLabel);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add Open Scenes"))
                {
                    SceneListManager.AddOpenScenes(data, sceneSortMode);
                    Repaint();
                }

                if (GUILayout.Button("Add All Project Scenes"))
                {
                    SceneListManager.AddAllProjectScenes(data, sceneSortMode);
                    Repaint();
                }

                if (GUILayout.Button("Clear All"))
                {
                    SceneListManager.ClearScenes(data);
                    Repaint();
                }

                DrawSortMenuButton
                (
                    sortMode: sceneSortMode,
                    setSortMode: value => sceneSortMode = value,
                    list: data.scenes
                );
            }
        }

        private void DrawSceneList()
        {
            sceneList.DoLayoutList();
            sceneListRect = GUILayoutUtility.GetLastRect();
            clickedListItemThisFrame |= ClickedOnListItem(sceneList, sceneListRect);
        }

        private void DrawPrefabHeader()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add All Prefabs In Current Scene"))
                {
                    PrefabListManager.AddAllPrefabsInScene(data, prefabSortMode);
                    Repaint();
                }

                if (GUILayout.Button("Add All Project Prefabs"))
                {
                    PrefabListManager.AddAllProjectPrefabs(data, prefabSortMode);
                    Repaint();
                }

                if (GUILayout.Button("Clear All"))
                {
                    PrefabListManager.ClearPrefabs(data);
                    Repaint();
                }

                DrawSortMenuButton
                (
                    sortMode: prefabSortMode,
                    setSortMode: v => prefabSortMode = v,
                    list: data.prefabs
                );
            }
        }

        private void DrawPrefabList()
        {
            prefabList.DoLayoutList();
            prefabListRect = GUILayoutUtility.GetLastRect();
            clickedListItemThisFrame |= ClickedOnListItem(prefabList, prefabListRect);
        }

        private static bool ClickedOnListItem(
            ReorderableList list,
            Rect rect)
        {
            Event e = Event.current;

            if (e.rawType != EventType.MouseDown || e.button != 0)
                return false;

            if (!rect.Contains(e.mousePosition))
                return false;

            float y = e.mousePosition.y - rect.y - 6f;

            if (y < 0f)
                return false;

            int index = Mathf.FloorToInt(y / list.elementHeight);
            return index >= 0 && index < list.count;
        }

        private void HandleClearSelection()
        {
            Event e = Event.current;

            // Left mouse click outside list item
            if (e.rawType == EventType.MouseDown && e.button == 0)
            {
                if (clickedListItemThisFrame)
                {
                    clickedListItemThisFrame = false;
                    return;
                }

                ClearCurrentListSelection();
                clickedListItemThisFrame = false;
            }
            // Escape key
            else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                ClearCurrentListSelection();
                e.Use();
            }

            return;

            void ClearCurrentListSelection()
            {
                ReorderableList list = selectedTab == 0 ? sceneList : prefabList;
                list.ClearSelection();
                Repaint();
            }
        }

        private void HandleWindowDragAndDrop(Rect dropArea)
        {
            Event evt = Event.current;

            if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform)
                return;

            Rect windowRect = new(0, 0, dropArea.width, dropArea.height);

            if (!windowRect.Contains(evt.mousePosition))
                return;

            bool hasSceneOrPrefab = DragAndDrop.objectReferences.Any(o => o is SceneAsset or GameObject);
            DragAndDrop.visualMode = hasSceneOrPrefab ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

            List<int> prefabObjectIndices = new();
            List<int> sceneObjectIndices = new();

            if (evt.type == EventType.DragPerform && hasSceneOrPrefab)
            {
                bool isScene = false;
                bool isPrefab = false;

                DragAndDrop.AcceptDrag();
                AddObjectsToList(ref isScene, ref isPrefab);
                SortObjects(isScene, isPrefab);
                FindObjectIndices(sceneObjectIndices, prefabObjectIndices);
                selectedTab = isScene ? SelectedTab.Scenes : SelectedTab.Prefabs;

                SelectListItems
                (
                    list: isScene
                        ? sceneList
                        : prefabList,
                    indices: isScene
                        ? sceneObjectIndices
                        : prefabObjectIndices
                );

                Storage.SaveData(data);
                Repaint();
                return;
            }

            evt.Use();

            return;

            void AddObjectsToList(
                ref bool isScene,
                ref bool isPrefab)
            {
                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    string path = AssetDatabase.GetAssetPath(obj);

                    if (obj is SceneAsset && path.EndsWith(".unity"))
                    {
                        isScene = true;
                        data.scenes.TryAdd(path);
                    }
                    else if (obj is GameObject && path.EndsWith(".prefab"))
                    {
                        isPrefab = true;
                        data.prefabs.TryAdd(path);   
                    }
                }
            }

            void SortObjects(
                bool isScene,
                bool isPrefab)
            {
                if (isScene)
                    data.scenes.Sort(sceneSortMode);
                else if (isPrefab)
                    data.prefabs.Sort(prefabSortMode);
            }

            void FindObjectIndices(
                List<int> ints,
                List<int> addedPrefabIndices1)
            {
                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    string path = AssetDatabase.GetAssetPath(obj);

                    if (obj is SceneAsset && path.EndsWith(".unity"))
                        ints.Add(data.scenes.FindIndex(s => s.Path == path));
                    else if (obj is GameObject && path.EndsWith(".prefab"))
                        addedPrefabIndices1.Add(data.prefabs.FindIndex(p => p.Path == path));
                }
            }

            void SelectListItems(
                ReorderableList list,
                List<int> indices)
            {
                if (indices.Count == 0)
                    return;

                int min = indices.Min();
                int max = indices.Max();

                list.ClearSelection();
                list.SelectRange(min, max);

                for (int i = min; i <= max; i++)
                    if (!indices.Contains(i))
                        list.Deselect(i);
            }
        }

        private void DrawInjectButtons()
        {
            AssetData[] enabledScenes = data.scenes.GetEnabled();
            AssetData[] enabledPrefabs = data.prefabs.GetEnabled();

            int sceneCount = enabledScenes.Length;
            int prefabCount = enabledPrefabs.Length;

            GUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                // Inject All
                using (new EditorGUI.DisabledScope(sceneCount == 0 && prefabCount == 0))
                {
                    if (GUILayout.Button("Inject All"))
                        DependencyInjector.BatchInjectAllScenesAndPrefabs(
                            sceneAssetPaths: enabledScenes
                                .Select(scene => scene.Path)
                                .ToArray(),
                            prefabAssetPaths: enabledPrefabs
                                .Select(prefab => prefab.Path)
                                .ToArray()
                        );
                }

                // Inject Scenes
                using (new EditorGUI.DisabledScope(sceneCount == 0))
                {
                    if (GUILayout.Button($"Inject Scenes ({sceneCount})"))
                        DependencyInjector.BatchInjectScenes(
                            sceneAssetPaths: enabledScenes
                                .Select(scene => scene.Path)
                                .ToArray(),
                            canClearLogs: true,
                            logStats: true
                        );
                }

                // Inject Prefabs
                using (new EditorGUI.DisabledScope(prefabCount == 0))
                {
                    if (GUILayout.Button($"Inject Prefabs ({prefabCount})"))
                        DependencyInjector.BatchInjectPrefabs(
                            enabledPrefabs
                                .Select(prefab => prefab.Path)
                                .ToArray(),
                            canClearLogs: true,
                            logStats: true
                        );
                }
            }
        }

        // private string searchQuery = "";
        //
        // private void DrawSearchBar()
        // {
        //     EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        //     GUI.SetNextControlName("SearchField");
        //     searchQuery = GUILayout.TextField(searchQuery, GUI.skin.FindStyle("ToolbarSeachTextField"));
        //
        //     if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        //     {
        //         searchQuery = "";
        //         GUI.FocusControl(null);
        //     }
        //
        //     EditorGUILayout.EndHorizontal();
        // }
    }
}