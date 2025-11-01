using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector
{
    public class BatchInjectorEditorWindow : EditorWindow
    {
        private const float WindowPaddingX = 5f;
        private const float WindowPaddingY = 10f;

        private GUIStyle titleStyle;

        private BatchInjectData data = new();
        private ReorderableList sceneList;
        private ReorderableList prefabList;
        private Vector2 listScroll;
        private SelectedTab selectedTab;

        private Rect sceneListRect;
        private Rect prefabListRect;
        private bool clickedListItemThisFrame;

        private SortMode sceneSortMode = SortMode.Custom;
        private SortMode prefabSortMode = SortMode.Custom;

        private string searchQuery = "";

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
            sceneList = CreateList(data.scenes, AssetDatabase.LoadAssetAtPath<SceneAsset>);
            prefabList = CreateList(data.prefabs, AssetDatabase.LoadAssetAtPath<GameObject>);
        }

        private void OnDisable()
        {
            Storage.SaveData(data);
        }

        private ReorderableList CreateList(
            List<AssetEntry> source,
            Func<string, Object> loader)
        {
            ReorderableList list = new(source, typeof(AssetEntry), true, false, false, true)
            {
                multiSelect = true
            };

            list.drawElementCallback = (
                rect,
                index,
                active,
                _) =>
            {
                if (index < 0 || index >= source.Count) return;
                AssetEntry entry = source[index];
                Object asset = loader(entry.path);
                bool missing = asset == null;

                const float toggleWidth = 20f;
                const float objWidth = 220f;

                if (active)
                    EditorGUI.DrawRect(rect, new Color(0.24f, 0.48f, 0.90f, 0.25f));

                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                entry.enabled = EditorGUI.Toggle(new Rect(rect.x + 4, rect.y, toggleWidth, rect.height), entry.enabled);

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.ObjectField(new Rect(rect.x + toggleWidth + 2, rect.y, objWidth, rect.height),
                        asset, typeof(Object), false);
                }

                string labelText = missing ? "(Deleted)" : entry.path;

                EditorGUI.LabelField(new Rect(rect.x + toggleWidth + objWidth + 7, rect.y,
                    rect.width - objWidth - 40, rect.height), labelText, EditorStyles.miniLabel);
            };

            list.onRemoveCallback = l =>
            {
                List<int> indices = l.selectedIndices?.Distinct().Where(i => i >= 0 && i < source.Count)
                    .OrderByDescending(i => i).ToList() ?? new List<int> { l.index };

                foreach (int i in indices)
                    source.RemoveAt(i);

                l.ClearSelection();
                l.index = Mathf.Clamp(l.index, 0, source.Count - 1);

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

        private void OnGUI()
        {
            HandleWindowDragAndDrop(position);

            Rect paddedRect = new(WindowPaddingX, WindowPaddingY,
                position.width - WindowPaddingX * 2,
                position.height - WindowPaddingY * 2);

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
                    DrawPrefabSection();
                    GUILayout.Space(5);
                    listScroll = EditorGUILayout.BeginScrollView(listScroll);
                    DrawPrefabList();
                    break;
            }

            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            DrawFooter();
            HandleGlobalClickToClearSelection();
            HandleGlobalEscapeToClearSelection();
            GUILayout.EndArea();
        }

        private void DrawHeader()
        {
            titleStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = EditorStyles.label.normal.textColor }
            };

            EditorGUILayout.LabelField("Batch Injector", titleStyle);
            EditorGUILayout.LabelField("Drag and drop scenes and prefabs anywhere in the window to add them to each list.");
            GUILayout.Space(8);

            selectedTab = (SelectedTab)GUILayout.Toolbar((int)selectedTab, new[]
            {
                $"Scenes ({data.scenes.Count})",
                $"Prefabs ({data.prefabs.Count})"
            });
        }

        private void DrawSortMenuButton(
            Func<SortMode> getSortMode,
            Action<SortMode> setSortMode,
            List<AssetEntry> list,
            Action onSorted)
        {
            SortMode currentMode = getSortMode();

            if (!GUILayout.Button($"Sort: {currentMode.GetDisplayString()}"))
                return;

            GenericMenu menu = new();

            AddItem("Path A–Z", SortMode.PathAtoZ, false);
            AddItem("Path Z–A", SortMode.PathZtoA, false);
            menu.AddSeparator("");
            AddItem("Name A–Z", SortMode.NameAtoZ, true);
            AddItem("Name Z–A", SortMode.NameZtoA, true);

            menu.ShowAsContext();
            return;

            void AddItem(
                string label,
                SortMode mode,
                bool isNameSort)
            {
                menu.AddItem(new GUIContent(label), currentMode == mode, () =>
                {
                    setSortMode(mode);
                    AssetListSorter.SortList(list, mode);
                    onSorted?.Invoke();
                });
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
                    SceneActions.AddOpenScenes(data, sceneSortMode);
                    Repaint();
                }

                if (GUILayout.Button("Add All Project Scenes"))
                {
                    SceneActions.AddAllProjectScenes(data, sceneSortMode);
                    Repaint();
                }

                if (GUILayout.Button("Clear All"))
                {
                    SceneActions.ClearScenes(data);
                    Repaint();
                }

                DrawSortMenuButton(
                    getSortMode: () => sceneSortMode,
                    setSortMode: v => sceneSortMode = v,
                    list: data.scenes,
                    onSorted: () =>
                    {
                        Storage.SaveData(data);
                        Repaint();
                    });
            }
        }

        private void DrawSceneList()
        {
            sceneList.DoLayoutList();
            sceneListRect = GUILayoutUtility.GetLastRect();
            clickedListItemThisFrame |= ClickedOnListItem(sceneList, sceneListRect);
        }

        private void DrawPrefabSection()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add All Prefabs In Current Scene"))
                {
                    PrefabActions.AddAllPrefabsInScene(data, prefabSortMode);
                    Repaint();
                }

                if (GUILayout.Button("Add All Project Prefabs"))
                {
                    PrefabActions.AddAllProjectPrefabs(data, prefabSortMode);
                    Repaint();
                }

                if (GUILayout.Button("Clear All"))
                {
                    PrefabActions.ClearPrefabs(data);
                    Repaint();
                }

                DrawSortMenuButton(
                    getSortMode: () => prefabSortMode,
                    setSortMode: v => prefabSortMode = v,
                    list: data.prefabs,
                    onSorted: () =>
                    {
                        Storage.SaveData(data);
                        Repaint();
                    });
            }
        }

        private void DrawPrefabList()
        {
            prefabList.DoLayoutList();
            prefabListRect = GUILayoutUtility.GetLastRect();
            clickedListItemThisFrame |= ClickedOnListItem(prefabList, prefabListRect);
        }

        // Detect click on list item
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

        // Handle global click outside any list item
        private void HandleGlobalClickToClearSelection()
        {
            Event e = Event.current;

            if (e.rawType != EventType.MouseDown || e.button != 0)
                return;

            if (!clickedListItemThisFrame)
            {
                (selectedTab == 0 ? sceneList : prefabList).ClearSelection();
                Repaint();
            }

            clickedListItemThisFrame = false;
        }

        private void HandleGlobalEscapeToClearSelection()
        {
            Event e = Event.current;

            if (e.type != EventType.KeyDown)
                return;

            // Escape clears selection on both lists
            if (e.keyCode == KeyCode.Escape)
            {
                (selectedTab == 0 ? sceneList : prefabList).ClearSelection();
                Repaint();
                e.Use(); // consume the event so it doesn’t propagate further
            }
        }

        // Drag & drop handling
        private void HandleWindowDragAndDrop(Rect dropArea)
        {
            Event evt = Event.current;

            if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform)
                return;

            Rect windowRect = new(0, 0, dropArea.width, dropArea.height);
            if (!windowRect.Contains(evt.mousePosition)) return;

            bool hasSceneOrPrefab = DragAndDrop.objectReferences.Any(o => o is SceneAsset or GameObject);
            DragAndDrop.visualMode = hasSceneOrPrefab ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

            List<int> addedPrefabIndices = new();
            List<int> addedSceneIndices = new();

            if (evt.type == EventType.DragPerform && hasSceneOrPrefab)
            {
                DragAndDrop.AcceptDrag();

                bool containsScene = false;
                bool containsPrefab = false;

                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    string path = AssetDatabase.GetAssetPath(obj);

                    if (obj is SceneAsset && path.EndsWith(".unity"))
                    {
                        containsScene = true;

                        if (data.scenes.All(s => s.path != path))
                            data.scenes.Add(new AssetEntry(path, true));
                    }
                    else if (obj is GameObject && path.EndsWith(".prefab"))
                    {
                        containsPrefab = true;

                        if (data.prefabs.All(p => p.path != path))
                            data.prefabs.Add(new AssetEntry(path, true));
                    }
                }

                if (containsScene)
                    AssetListSorter.SortList(data.scenes, sceneSortMode);
                else if (containsPrefab)
                    AssetListSorter.SortList(data.prefabs, prefabSortMode);

                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    string path = AssetDatabase.GetAssetPath(obj);

                    if (obj is SceneAsset && path.EndsWith(".unity"))
                        addedSceneIndices.Add(data.scenes.FindIndex(s => s.path == path));
                    else if (obj is GameObject && path.EndsWith(".prefab"))
                        addedPrefabIndices.Add(data.prefabs.FindIndex(p => p.path == path));
                }

                // Switch tab regardless of duplicates
                if (containsScene)
                {
                    selectedTab = SelectedTab.Scenes;
                    SelectIndices(sceneList, addedSceneIndices);
                }
                else if (containsPrefab)
                {
                    selectedTab = SelectedTab.Prefabs;
                    SelectIndices(prefabList, addedPrefabIndices);
                }

                Storage.SaveData(data);
                Repaint();
                return;

                void SelectIndices(
                    ReorderableList list,
                    List<int> indices)
                {
                    if (indices.Count == 0)
                        return;

                    list.ClearSelection();

                    int min = indices.Min();
                    int max = indices.Max();

                    list.SelectRange(min, max);

                    for (int i = min; i <= max; i++)
                        if (!indices.Contains(i))
                            list.Deselect(i);
                }
            }

            evt.Use();
        }

        private void DrawFooter()
        {
            GUILayout.Space(6);
            EditorGUILayout.BeginHorizontal();

            int sceneCount = data.scenes.Count;
            int prefabCount = data.prefabs.Count;

            int selectedScenes = sceneList.selectedIndices?.Count ?? 0;
            int selectedPrefabs = prefabList.selectedIndices?.Count ?? 0;

            // Inject All
            if (GUILayout.Button("Inject All"))
                Debug.Log("Injecting all scenes and prefabs...");

            // Inject All Scenes
            if (GUILayout.Button($"Inject All Scenes ({sceneCount})"))
                Debug.Log($"Injecting {sceneCount} scenes...");

            // Inject All Prefabs
            if (GUILayout.Button($"Inject All Prefabs ({prefabCount})"))
                Debug.Log($"Injecting {prefabCount} prefabs...");

            // Inject Selected Scene(s)
            using (new EditorGUI.DisabledScope(selectedScenes == 0))
            {
                if (GUILayout.Button($"Inject Selected Scene{(selectedScenes != 1 ? "s" : "")} ({selectedScenes})"))
                    Debug.Log($"Injecting {selectedScenes} selected scene{(selectedScenes != 1 ? "s" : "")}...");
            }

            // Inject Selected Prefab(s)
            using (new EditorGUI.DisabledScope(selectedPrefabs == 0))
            {
                if (GUILayout.Button($"Inject Selected Prefab{(selectedPrefabs != 1 ? "s" : "")} ({selectedPrefabs})"))
                    Debug.Log($"Injecting {selectedPrefabs} selected prefab{(selectedPrefabs != 1 ? "s" : "")}...");
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUI.SetNextControlName("SearchField");
            searchQuery = GUILayout.TextField(searchQuery, GUI.skin.FindStyle("ToolbarSeachTextField"));

            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                searchQuery = "";
                GUI.FocusControl(null);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}