using System;
using System.ComponentModel;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Enums;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Extensions;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector.Drawers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
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
            menu.AddSeparator("");
            AddItem(SortMode.StatusSuccessToError);
            AddItem(SortMode.StatusErrorToSuccess);
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
                        batchInjectorData.IsDirty = true;
                        repaint?.Invoke();
                    }
                );
            }
        }
    }
}