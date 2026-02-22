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
            BatchInjectorData batchInjectorData,
            AssetList assetList,
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
                    on: assetList.SortMode == sortMode,
                    func: () =>
                    {
                        assetList.SortMode = sortMode;
                        assetList.Sort();
                        batchInjectorData.isDirty = true;
                        repaint?.Invoke();
                    }
                );
            }
        }
    }
}