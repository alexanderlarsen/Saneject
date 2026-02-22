using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums;
using Plugins.Saneject.Experimental.Editor.Pipeline;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Drawers
{
    public static class ContextMenuDrawer
    {
        public static void DrawContextMenu(
            ReorderableList list,
            AssetList assetList,
            WindowTab tab,
            Rect rect,
            Action onModified)
        {
            Event e = Event.current;

            if (e.type != EventType.ContextClick || !rect.Contains(e.mousePosition))
                return;

            List<int> selected = list.selectedIndices?.ToList() ?? new List<int>();
            bool hasSelection = selected.Count > 0;

            GenericMenu menu = new();

            menu.AddItem
            (
                label: "Select All",
                isEnabled: selected.Count != list.count,
                onClick: () => SelectAll(list, assetList, onModified)
            );

            menu.AddSeparator("");

            menu.AddItem
            (
                label: $"Inject {selected.Count} Selected {(tab == WindowTab.Scenes ? "Scene" : "Prefab")}{(selected.Count == 1 ? "" : "s")}",
                isEnabled: hasSelection,
                onClick: () => InjectSelected(assetList, tab, selected)
            );

            menu.AddItem
            (
                label: "Enable",
                isEnabled: selected.Count(i => assetList.GetElementAt(i).Enabled) != selected.Count,
                onClick: () => EnableSelected(assetList, onModified, selected)
            );

            menu.AddItem
            (
                label: "Disable",
                isEnabled: selected.Count(i => assetList.GetElementAt(i).Enabled) != 0,
                onClick: () => DisableSelected(assetList, onModified, selected)
            );

            menu.AddSeparator("");

            menu.AddItem
            (
                label: "Clear Injection Status",
                isEnabled: hasSelection,
                onClick: () => ClearSelectedInjectStatus(assetList, onModified, selected)
            );

            menu.AddSeparator("");

            menu.AddItem
            (
                label: "Remove Selected",
                isEnabled: hasSelection,
                onClick: () => RemoveSelected(list, assetList, onModified, selected)
            );

            menu.ShowAsContext();
            e.Use();
        }

        #region Extensions

        private static void AddItem(
            this GenericMenu menu,
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

        #endregion

        #region Context menu action methods

        private static void SelectAll(
            ReorderableList list,
            AssetList assetList,
            Action onModified)
        {
            list.GrabKeyboardFocus();
            list.SelectRange(0, list.count - 1);
            assetList.TrySortByEnabledOrDisabled();
            onModified.Invoke();
        }

        private static void InjectSelected(
            AssetList assetList,
            WindowTab tab,
            List<int> selected)
        {
            AssetData[] assets = selected
                .Select(assetList.GetElementAt)
                .ToArray();

            switch (tab)
            {
                case WindowTab.Scenes:
                {
                    SceneBatchItem[] sceneBatchItems = assets
                        .Select(asset => new SceneBatchItem(asset.Path, ContextWalkFilter.All))
                        .ToArray();

                    if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(sceneBatchItems.Length, 0))
                        return;

                    InjectionRunner.RunBatch(sceneBatchItems);
                    break;
                }

                case WindowTab.Prefabs:
                {
                    PrefabBatchItem[] prefabBatchItems = assets
                        .Select(asset => new PrefabBatchItem(asset.Path))
                        .ToArray();

                    if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(0, prefabBatchItems.Length))
                        return;

                    InjectionRunner.RunBatch(prefabBatchItems);
                    break;
                }
            }
        }

        private static void EnableSelected(
            AssetList assetList,
            Action onModified,
            List<int> selected)
        {
            foreach (int i in selected)
                assetList.GetElementAt(i)
                    .Enabled = true;

            assetList.TrySortByEnabledOrDisabled();
            onModified?.Invoke();
        }

        private static void DisableSelected(
            AssetList assetList,
            Action onModified,
            List<int> selected)
        {
            foreach (int i in selected)
                assetList.GetElementAt(i)
                    .Enabled = false;

            assetList.TrySortByEnabledOrDisabled();
            onModified?.Invoke();
        }

        private static void ClearSelectedInjectStatus(
            AssetList assetList,
            Action onModified,
            List<int> selected)
        {
            foreach (int i in selected.OrderByDescending(i => i))
                assetList.GetElementAt(i).Status = InjectionStatus.Unknown;

            onModified?.Invoke();
            GUI.changed = true;
        }

        private static void RemoveSelected(
            ReorderableList list,
            AssetList assetList,
            Action onModified,
            List<int> selected)
        {
            if (!EditorUtility.DisplayDialog
                (
                    title: "Batch Injector",
                    message: $"Remove {selected.Count} selected item{(selected.Count == 1 ? "" : "s")}?",
                    ok: "Yes",
                    cancel: "No"
                ))
                return;

            foreach (int i in selected.OrderByDescending(i => i))
                assetList.RemoveAt(i);

            list.ClearSelection();
            onModified?.Invoke();
            GUI.changed = true;
        }

        #endregion
    }
}