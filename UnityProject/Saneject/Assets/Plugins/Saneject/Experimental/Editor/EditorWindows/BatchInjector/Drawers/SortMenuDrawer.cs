using System;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Extensions;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Persistence;
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
    }
}