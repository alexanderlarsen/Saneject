using System;
using Plugins.Saneject.Editor.BatchInjection.Data;
using Plugins.Saneject.Editor.BatchInjection.Enums;
using Plugins.Saneject.Editor.BatchInjection.Persistence;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.Saneject.Editor.BatchInjection.Drawers
{
    public static class AssetListDrawer
    {
        public static void DrawListHeader(
            BatchInjectorData injectorData,
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

                DrawSortMenuButton(injectorData, list, repaint);
            }
        }

        public static void DrawList(
            ReorderableList list,
            ref Rect rect,
            ref bool clickedListAnyItem)
        {
            list.DoLayoutList();
            rect = GUILayoutUtility.GetLastRect();
            clickedListAnyItem |= WasListItemClicked(list, rect);
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

        private static void DrawSortMenuButton(
            BatchInjectorData injectorData,
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
                        Storage.SaveData(injectorData);
                        repaint?.Invoke();
                    }
                );
            }
        }

        private static bool WasListItemClicked(
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