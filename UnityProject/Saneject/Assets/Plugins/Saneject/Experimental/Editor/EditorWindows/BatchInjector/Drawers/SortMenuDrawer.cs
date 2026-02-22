using System;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Extensions;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Drawers
{
    public static class SortMenuDrawer
    {
        public static void DrawSortMenu(
            BatchInjectorData data,
            AssetList list,
            Action repaint)
        {
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

            void AddItem(SortMode sortMode)
            {
                menu.AddItem
                (
                    content: new GUIContent(sortMode.GetDisplayString()),
                    on: list.SortMode == sortMode,
                    func: () =>
                    {
                        list.SortMode = sortMode;
                        list.Sort();
                        data.isDirty = true;
                        repaint?.Invoke();
                    }
                );
            }
        }
    }
}