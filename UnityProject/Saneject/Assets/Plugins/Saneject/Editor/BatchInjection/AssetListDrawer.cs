using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.BatchInjection
{
    public static class AssetListDrawer
    {
        private static GUIStyle missingPathLabel;

        public static ReorderableList CreateReorderableList(
            BatchInjectorData data,
            AssetList assetList,
            Action onModified)
        {
            ReorderableList reorderable = new(
                assetList.Elements,
                typeof(AssetItem),
                draggable: true,
                displayHeader: false,
                displayAddButton: false,
                displayRemoveButton: true)
            {
                multiSelect = true
            };

            reorderable.drawElementCallback = (
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

                AssetItem element = assetList.GetElementAt(index);

                bool toggle = EditorGUI.Toggle(
                    new Rect(rect.x + 4, rect.y, toggleWidth, rect.height),
                    element.Enabled);

                if (toggle != element.Enabled)
                {
                    element.Enabled = toggle;
                    assetList.TrySortByEnabledOrDisabled();
                    onModified?.Invoke();
                    GUI.changed = true;
                }

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.ObjectField(
                        new Rect(rect.x + toggleWidth + 2, rect.y, objWidth, rect.height),
                        element.Asset,
                        typeof(Object),
                        false);
                }

                bool hasAsset = element.Asset != null;

                GUIStyle pathLabelStyle = hasAsset
                    ? EditorStyles.miniLabel
                    : missingPathLabel ??= new GUIStyle(EditorStyles.miniLabel)
                    {
                        normal =
                        {
                            textColor = new Color(0.9f, 0.45f, 0.4f, 1f)
                        }
                    };

                string labelText = hasAsset
                    ? element.Path
                    : "Deleted";

                EditorGUI.LabelField(
                    new Rect(rect.x + toggleWidth + objWidth + 7, rect.y, rect.width - objWidth - 40, rect.height),
                    labelText,
                    pathLabelStyle);

                InjectionStatus status = assetList.GetElementAt(index).Status;

                EditorGUI.LabelField(
                    new Rect(rect.width, rect.y, 20, 20),
                    status.GetGuiContent()
                );

                DrawContextMenu(reorderable, assetList, data.windowTab, rect, index, onModified);
            };

            reorderable.onRemoveCallback = _ =>
            {
                if (reorderable.selectedIndices.Count == 0)
                    return;

                List<int> indices = reorderable.selectedIndices?
                    .Distinct()
                    .Where(i => i >= 0 && i < assetList.TotalCount)
                    .OrderByDescending(i => i)
                    .ToList() ?? new List<int> { reorderable.index };

                if (!EditorUtility.DisplayDialog("Batch Injector",
                        $"Do you want to delete {indices.Count} item{(indices.Count == 1 ? "" : "s")}?",
                        "Yes", "No"))
                    return;

                foreach (int i in indices)
                    assetList.RemoveAt(i);

                reorderable.ClearSelection();
                reorderable.index = Mathf.Clamp(reorderable.index, 0, assetList.TotalCount - 1);
                onModified?.Invoke();
                GUI.changed = true;
            };

            reorderable.onReorderCallback = _ =>
            {
                assetList.SortMode = SortMode.Custom;
                onModified?.Invoke();
            };

            return reorderable;
        }

        public static void DrawListHeader(
            BatchInjectorData data,
            AssetList list,
            string title,
            (string label, Action onClick)[] buttons,
            Action repaint)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();

                foreach ((string label, Action onClick) button in buttons)
                    if (GUILayout.Button(button.label))
                    {
                        button.onClick();
                        repaint?.Invoke();
                    }

                DrawSortMenuButton(data, list, repaint);
            }
        }

        public static void DrawList(
            ReorderableList list,
            ref Rect rect,
            ref bool clickedListAnyItem)
        {
            list.DoLayoutList();
            rect = GUILayoutUtility.GetLastRect();
            clickedListAnyItem |= CheckListItemClicked(list, rect);
        }

        public static void HandleInput(
            ref bool clickedAnyListItem,
            WindowTab tab,
            ReorderableList sceneList,
            ReorderableList prefabList,
            Action repaint)
        {
            Event e = Event.current;

            // Left mouse click outside list item
            if (e.rawType == EventType.MouseDown && e.button == 0)
            {
                if (clickedAnyListItem)
                {
                    clickedAnyListItem = false;
                    return;
                }

                ClearSelection();
                clickedAnyListItem = false;
                return;
            }

            // Escape key
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                ClearSelection();
                e.Use();
                return;
            }

            // Ctrl + A
            if (e.type == EventType.KeyDown && (e.control || e.command) && e.keyCode == KeyCode.A)
            {
                SelectAll();
                e.Use();
            }

            return;

            void SelectAll()
            {
                ReorderableList list = tab == 0 ? sceneList : prefabList;
                list.GrabKeyboardFocus();
                list.SelectRange(0, list.count - 1);
                repaint.Invoke();
            }

            void ClearSelection()
            {
                ReorderableList list = tab == 0 ? sceneList : prefabList;
                list.ClearSelection();
                repaint.Invoke();
            }
        }

        private static void DrawContextMenu(
            ReorderableList list,
            AssetList assetList,
            WindowTab tab,
            Rect rect,
            int index,
            Action onModified)
        {
            Event e = Event.current;

            if (e.type != EventType.ContextClick || !rect.Contains(e.mousePosition))
                return;

            GenericMenu menu = new();
            List<int> selected = list.selectedIndices?.ToList() ?? new List<int> { index };

            bool hasSelection = selected.Count > 0;

            AddItem(
                label: "Select All",
                isEnabled: selected.Count != list.count,
                onClick: () =>
                {
                    list.GrabKeyboardFocus();
                    list.SelectRange(0, list.count - 1);
                    assetList.TrySortByEnabledOrDisabled();
                    onModified.Invoke();
                }
            );

            menu.AddSeparator("");

            AddItem(
                label: $"Inject {selected.Count} Selected {(tab == WindowTab.Scenes ? "Scene" : "Prefab")}{(selected.Count == 1 ? "" : "s")}",
                isEnabled: hasSelection,
                onClick: () =>
                {
                    AssetItem[] assets = selected
                        .Select(assetList.GetElementAt)
                        .ToArray();

                    switch (tab)
                    {
                        case WindowTab.Scenes:
                            if (!Dialogs.BatchInjection.ConfirmInjectScene(assets.Length))
                                break;

                            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                                break;

                            if (UserSettings.ClearLogsOnInjection)
                                ConsoleUtils.ClearLog();

                            DependencyInjector.BatchInjectScenes(assets, true);
                            break;

                        case WindowTab.Prefabs:
                            if (!Dialogs.BatchInjection.ConfirmInjectPrefab(assets.Length))
                                break;

                            if (UserSettings.ClearLogsOnInjection)
                                ConsoleUtils.ClearLog();

                            DependencyInjector.BatchInjectPrefabs(assets, true);
                            break;
                    }
                });

            menu.AddSeparator("");

            AddItem(
                label: "Find in Project",
                isEnabled: selected.Count == 1,
                onClick: () =>
                {
                    foreach (int i in selected)
                    {
                        AssetItem item = assetList.GetElementAt(i);
                        Object asset = item.Asset;

                        if (asset)
                            EditorGUIUtility.PingObject(asset);
                    }
                }
            );

            menu.AddSeparator("");

            AddItem(
                label: "Enable",
                isEnabled: selected.Count(i => assetList.GetElementAt(i).Enabled) != selected.Count,
                onClick: () =>
                {
                    foreach (int i in selected)
                        assetList.GetElementAt(i)
                            .Enabled = true;

                    assetList.TrySortByEnabledOrDisabled();
                    onModified?.Invoke();
                }
            );

            AddItem(
                label: "Disable",
                isEnabled: selected.Count(i => assetList.GetElementAt(i).Enabled) != 0,
                onClick: () =>
                {
                    foreach (int i in selected)
                        assetList.GetElementAt(i)
                            .Enabled = false;

                    assetList.TrySortByEnabledOrDisabled();
                    onModified?.Invoke();
                }
            );

            menu.AddSeparator("");

            AddItem(
                label: "Clear Injection Status",
                isEnabled: hasSelection,
                onClick: () =>
                {
                    foreach (int i in selected.OrderByDescending(i => i))
                        assetList.GetElementAt(i).Status = InjectionStatus.Unknown;

                    onModified?.Invoke();
                    GUI.changed = true;
                }
            );

            menu.AddSeparator("");

            AddItem(
                label: "Remove Selected",
                isEnabled: hasSelection,
                onClick: () =>
                {
                    if (!EditorUtility.DisplayDialog("Batch Injector",
                            $"Remove {selected.Count} selected item{(selected.Count == 1 ? "" : "s")}?",
                            "Yes",
                            "No"))
                        return;

                    foreach (int i in selected.OrderByDescending(i => i))
                        assetList.RemoveAt(i);

                    list.ClearSelection();
                    onModified?.Invoke();
                    GUI.changed = true;
                }
            );

            menu.ShowAsContext();
            e.Use();

            return;

            void AddItem(
                string label,
                bool isEnabled,
                GenericMenu.MenuFunction onClick)
            {
                if (!isEnabled)
                {
                    menu.AddDisabledItem(new GUIContent(label));
                    return;
                }

                menu.AddItem(new GUIContent(label), false, onClick);
            }
        }

        private static void DrawSortMenuButton(
            BatchInjectorData data,
            AssetList list,
            Action repaint)
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
                        list.SortMode = mode;
                        list.Sort();
                        Storage.SaveData(data);
                        repaint?.Invoke();
                    }
                );
            }
        }

        private static bool CheckListItemClicked(
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

        private static string GetDisplayString(this SortMode mode)
        {
            return mode switch
            {
                SortMode.PathAtoZ => "Path A-Z",
                SortMode.PathZtoA => "Path Z-A",
                SortMode.NameAtoZ => "Name A-Z",
                SortMode.NameZtoA => "Name Z-A",
                SortMode.EnabledToDisabled => "Enabled-Disabled",
                SortMode.DisabledToEnabled => "Disabled-Enabled",
                _ => "Custom"
            };
        }

        private static GUIContent GetGuiContent(this InjectionStatus status)
        {
            return status switch
            {
                InjectionStatus.Unknown => new GUIContent("❔", "Run injection to get a status"),
                InjectionStatus.Success => new GUIContent("✅", "Succesfully injected"),
                InjectionStatus.Warning => new GUIContent("⚠️", "Injected with warnings. See console for details"),
                InjectionStatus.Error => new GUIContent("❌", "Injection failed with errors. See console for details"),
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }
}