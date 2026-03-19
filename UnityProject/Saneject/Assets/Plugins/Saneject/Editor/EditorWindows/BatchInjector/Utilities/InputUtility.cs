using System;
using System.ComponentModel;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Enums;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector.Utilities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class InputUtility
    {
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
                ReorderableList list = tab == WindowTab.Scenes ? sceneList : prefabList;
                list.GrabKeyboardFocus();
                list.SelectRange(0, list.count - 1);
                repaint.Invoke();
            }

            void ClearSelection()
            {
                ReorderableList list = tab == WindowTab.Scenes ? sceneList : prefabList;
                list.ClearSelection();
                repaint.Invoke();
            }
        }
    }
}