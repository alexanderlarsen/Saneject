using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Drawers
{
    public static class AssetListDrawer
    {
        public static void DrawListHeader(
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
                        button.onClick.Invoke();
                        repaint?.Invoke();
                    }
            }
        }

        public static void DrawList(
            ReorderableList list,
            ref Rect rect,
            ref bool clickedListAnyItem)
        {
            list.DoLayoutList();
            rect = GUILayoutUtility.GetLastRect();
            clickedListAnyItem |= ClickedListAnyItem(list, rect);
        }

        private static bool ClickedListAnyItem(
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
    }
}