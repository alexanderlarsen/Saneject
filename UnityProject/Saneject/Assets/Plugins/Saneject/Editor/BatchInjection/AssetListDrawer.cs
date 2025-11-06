using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.BatchInjection
{
    public static class AssetListDrawer
    {
        private static GUIStyle missingPathLabel;

        public static ReorderableList CreateReorderableList(
            AssetList list,
            Action onModified)
        {
            ReorderableList reorderable = new(
                list.Elements,
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
                if (index < 0 || index >= list.TotalCount)
                    return;

                const float toggleWidth = 20f;
                const float objWidth = 220f;

                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                AssetItem element = list.GetElementAt(index);

                bool toggle = EditorGUI.Toggle(
                    new Rect(rect.x + 4, rect.y, toggleWidth, rect.height),
                    element.Enabled);

                if (toggle != element.Enabled)
                {
                    element.Enabled = toggle;
                    list.TrySortByEnabledOrDisabled();
                    list.UpdateEnabledCount();
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
            };

            reorderable.onRemoveCallback = _ =>
            {
                List<int> indices = reorderable.selectedIndices?
                    .Distinct()
                    .Where(i => i >= 0 && i < list.TotalCount)
                    .OrderByDescending(i => i)
                    .ToList() ?? new List<int> { reorderable.index };

                if (!EditorUtility.DisplayDialog("Batch Injector",
                        $"Do you want to delete {indices.Count} item{(indices.Count == 1 ? "" : "s")}?",
                        "Yes", "No"))
                    return;

                foreach (int i in indices)
                    list.RemoveAt(i);

                reorderable.ClearSelection();
                reorderable.index = Mathf.Clamp(reorderable.index, 0, list.TotalCount - 1);
                onModified?.Invoke();
                GUI.changed = true;
            };

            reorderable.onReorderCallback = _ => onModified?.Invoke();

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

        public static void HandleClearSelection(
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

                ClearCurrentListSelection();
                clickedAnyListItem = false;
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
                ReorderableList list = tab == 0 ? sceneList : prefabList;
                list.ClearSelection();
                repaint.Invoke();
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
                        list.SetSortMode(mode);
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
    }
}