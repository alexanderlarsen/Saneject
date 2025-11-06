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
        private GUIStyle titleStyle;
        private bool clickedListItemThisFrame;

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
            sceneList = CreateList(data.sceneList);
            prefabList = CreateList(data.prefabList);
        }

        private void OnDisable()
        {
            Storage.SaveData(data);
        }

        private void OnGUI()
        {
            HandleWindowDragAndDrop(position);
            Rect paddedRect = new(WindowPadding, WindowPadding, position.width - WindowPadding * 2, position.height - WindowPadding * 2);
            GUILayout.BeginArea(paddedRect);
            DrawHeader();
            GUILayout.Space(8);

            switch (data.windowTab)
            {
                case WindowTab.Scenes:
                    DrawSceneHeader();
                    GUILayout.Space(5);
                    listScroll = EditorGUILayout.BeginScrollView(listScroll);
                    DrawSceneList();
                    break;

                case WindowTab.Prefabs:
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

        private ReorderableList CreateList(AssetList assetList)
        {
            ReorderableList list = new(
                elements: assetList.Elements,
                elementType: typeof(AssetItem),
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
                if (index < 0 || index >= assetList.TotalCount)
                    return;

                const float toggleWidth = 20f;
                const float objWidth = 220f;

                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                bool toggle = EditorGUI.Toggle
                (
                    new Rect(rect.x + 4, rect.y, toggleWidth, rect.height),
                    assetList.GetElementAt(index).Enabled
                );

                if (assetList.GetElementAt(index).Enabled != toggle)
                {
                    assetList.GetElementAt(index).Enabled = toggle;
                    assetList.TrySortByEnabledOrDisabled();
                    assetList.UpdateEnabledCount();
                    Storage.SaveData(data);
                    GUI.changed = true;
                }

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.ObjectField
                    (
                        position: new Rect(rect.x + toggleWidth + 2, rect.y, objWidth, rect.height),
                        obj: assetList.GetElementAt(index).Asset,
                        objType: typeof(Object),
                        allowSceneObjects: false
                    );
                }

                string labelText = assetList.GetElementAt(index).Asset == null
                    ? "(Deleted)"
                    : assetList.GetElementAt(index).Path;

                EditorGUI.LabelField
                (
                    position: new Rect(rect.x + toggleWidth + objWidth + 7, rect.y, rect.width - objWidth - 40, rect.height),
                    label: labelText,
                    style: EditorStyles.miniLabel
                );
            };

            list.onRemoveCallback = _ =>
            {
                List<int> indices = list.selectedIndices?
                    .Distinct()
                    .Where(i => i >= 0 && i < assetList.TotalCount)
                    .OrderByDescending(i => i)
                    .ToList() ?? new List<int> { list.index };

                if (!EditorUtility.DisplayDialog("Batch Injector",
                        $"Do you want to delete {indices.Count} item{(indices.Count == 1 ? "" : "s")}?",
                        "Yes", "No"))
                    return;

                foreach (int i in indices)
                    assetList.RemoveAt(i);

                list.ClearSelection();
                list.index = Mathf.Clamp(list.index, 0, assetList.TotalCount - 1);
                Storage.SaveData(data);
                GUI.changed = true;
            };

            list.onReorderCallback = _ =>
            {
                switch (data.windowTab)
                {
                    case WindowTab.Scenes:
                        data.sceneList.SetSortMode(SortMode.Custom);
                        break;

                    case WindowTab.Prefabs:
                        data.prefabList.SetSortMode(SortMode.Custom);
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

            data.windowTab = (WindowTab)GUILayout.Toolbar
            (
                selected: (int)data.windowTab,
                texts: new[]
                {
                    $"Scenes ({data.sceneList.TotalCount})",
                    $"Prefabs ({data.prefabList.TotalCount})"
                }
            );
        }

        private void DrawSortMenuButton(AssetList list)
        {
            if (!GUILayout.Button($"Sort: {list.SortMode.GetDisplayString()}"))
                return;

            GenericMenu menu = new();
            AddItem(SortMode.NameAtoZ);
            AddItem(SortMode.NameZtoA);
            menu.AddSeparator("");
            AddItem(SortMode.PathAtoZ);
            AddItem(SortMode.PathZtoA);
            menu.AddSeparator("");
            AddItem(SortMode.EnabledToDisabled);
            AddItem(SortMode.DisabledToEnabled);
            menu.ShowAsContext();

            return;

            void AddItem(SortMode mode)
            {
                menu.AddItem
                (
                    content: new GUIContent(mode.GetDisplayString()),
                    on: list.SortMode == mode,
                    func: () =>
                    {
                        list.SetSortMode(mode);
                        list.Sort();
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
                    SceneListManager.AddOpenScenes(data);
                    Repaint();
                }

                if (GUILayout.Button("Add All Project Scenes"))
                {
                    SceneListManager.AddAllProjectScenes(data);
                    Repaint();
                }

                if (GUILayout.Button("Clear All"))
                {
                    SceneListManager.ClearScenes(data);
                    Repaint();
                }

                DrawSortMenuButton(data.sceneList);
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
                    PrefabListManager.AddAllPrefabsInScene(data);
                    Repaint();
                }

                if (GUILayout.Button("Add All Project Prefabs"))
                {
                    PrefabListManager.AddAllProjectPrefabs(data);
                    Repaint();
                }

                if (GUILayout.Button("Clear All"))
                {
                    PrefabListManager.ClearPrefabs(data);
                    Repaint();
                }

                DrawSortMenuButton(data.prefabList);
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
                ReorderableList list = data.windowTab == 0 ? sceneList : prefabList;
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
                data.windowTab = isScene ? WindowTab.Scenes : WindowTab.Prefabs;

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
                        data.sceneList.TryAdd(path);
                    }
                    else if (obj is GameObject && path.EndsWith(".prefab"))
                    {
                        isPrefab = true;
                        data.prefabList.TryAdd(path);
                    }
                }
            }

            void SortObjects(
                bool isScene,
                bool isPrefab)
            {
                if (isScene)
                    data.sceneList.Sort();
                else if (isPrefab)
                    data.prefabList.Sort();
            }

            void FindObjectIndices(
                List<int> ints,
                List<int> addedPrefabIndices)
            {
                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    string path = AssetDatabase.GetAssetPath(obj);

                    if (obj is SceneAsset && path.EndsWith(".unity"))
                        ints.Add(data.sceneList.FindIndexByPath(path));
                    else if (obj is GameObject && path.EndsWith(".prefab"))
                        addedPrefabIndices.Add(data.prefabList.FindIndexByPath(path));
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
            int sceneCount = data.sceneList.EnabledCount;
            int prefabCount = data.prefabList.EnabledCount;

            GUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                // Inject All
                using (new EditorGUI.DisabledScope(sceneCount == 0 && prefabCount == 0))
                {
                    if (GUILayout.Button("Inject All"))
                        DependencyInjector.BatchInjectAllScenesAndPrefabs(
                            sceneAssetPaths: data.sceneList
                                .GetEnabled()
                                .Select(scene => scene.Path)
                                .ToArray(),
                            prefabAssetPaths: data.prefabList
                                .GetEnabled()
                                .Select(prefab => prefab.Path)
                                .ToArray()
                        );
                }

                // Inject Scenes
                using (new EditorGUI.DisabledScope(sceneCount == 0))
                {
                    if (GUILayout.Button($"Inject Scenes ({sceneCount})"))
                        DependencyInjector.BatchInjectScenes(
                            sceneAssetPaths: data.sceneList
                                .GetEnabled()
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
                            prefabAssetPaths: data.prefabList
                                .GetEnabled()
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