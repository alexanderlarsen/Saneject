using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Drawers
{
    public static class ContextMenuDrawer
    {
        public static void DrawContextMenu(
            BatchInjectorData batchInjectorData,
            ReorderableList reorderableList,
            AssetList assetList,
            WindowTab tab,
            Rect rect)
        {
            Event e = Event.current;

            if (e.type != EventType.ContextClick || !rect.Contains(e.mousePosition))
                return;

            List<int> selected = reorderableList.selectedIndices?.ToList() ?? new List<int>();
            bool hasSelection = selected.Count > 0;

            GenericMenu menu = new();

            menu.AddItem
            (
                label: "Select All",
                isEnabled: selected.Count != reorderableList.count,
                onClick: () => SelectAll(reorderableList)
            );

            menu.AddSeparator("");

            menu.AddItem
            (
                label: $"Inject {(selected.Count > 0 ? selected.Count + " " : "")}Selected {(tab == WindowTab.Scenes ? "Scene" : "Prefab")}{(selected.Count == 1 ? "" : "s")}",
                isEnabled: hasSelection,
                onClick: () => InjectSelection(batchInjectorData, reorderableList, assetList, tab, selected)
            );

            menu.AddSeparator("");

            menu.AddItem
            (
                label: "Enable",
                isEnabled: selected.Count(i => assetList.GetElementAt(i).Enabled) != selected.Count,
                onClick: () => EnableSelected(batchInjectorData, assetList, selected)
            );

            menu.AddItem
            (
                label: "Disable",
                isEnabled: selected.Count(i => assetList.GetElementAt(i).Enabled) != 0,
                onClick: () => DisableSelected(batchInjectorData, assetList, selected)
            );

            menu.AddSeparator("");

            menu.AddItem
            (
                label: "Clear Injection Status",
                isEnabled: hasSelection,
                onClick: () => ClearSelectedInjectStatus(batchInjectorData, assetList, selected)
            );

            menu.AddSeparator("");

            menu.AddItem
            (
                label: "Remove",
                isEnabled: hasSelection,
                onClick: () => RemoveSelected(batchInjectorData, reorderableList, assetList, selected)
            );

            menu.AddSeparator("");

            const string walkFilterBaseLabel = "Set Context Walk Filter";

            if (!hasSelection)
                menu.AddDisabledItem(new GUIContent(walkFilterBaseLabel));
            else
                foreach (ContextWalkFilter value in Enum.GetValues(typeof(ContextWalkFilter)))
                {
                    if (tab == WindowTab.Scenes &&
                        value is ContextWalkFilter.PrefabAssetObjects or ContextWalkFilter.SameContextsAsSelection)
                        continue;

                    if (tab == WindowTab.Prefabs &&
                        value is ContextWalkFilter.SceneObjects or ContextWalkFilter.SameContextsAsSelection)
                        continue;

                    ContextWalkFilter captured = value;

                    menu.AddItem
                    (
                        label: $"{walkFilterBaseLabel}/{ObjectNames.NicifyVariableName(value.ToString())}",
                        isEnabled: true,
                        onClick: () => SetSelectedContextFilter(batchInjectorData, assetList, selected, captured)
                    );
                }

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

        private static void SelectAll(ReorderableList list)
        {
            list.GrabKeyboardFocus();
            list.SelectRange(0, list.count - 1);
        }

        private static void InjectSelection(
            BatchInjectorData batchInjectorData,
            ReorderableList reorderableList,
            AssetList assetList,
            WindowTab tab,
            List<int> selected)
        {
            AssetData[] selectedAssets = selected
                .Select(assetList.GetElementAt)
                .ToArray();

            bool hasInjected = tab switch
            {
                WindowTab.Scenes => InjectionUtility.TryInject(selectedAssets.OfType<SceneAssetData>(), null),
                WindowTab.Prefabs => InjectionUtility.TryInject(null, selectedAssets.OfType<PrefabAssetData>()),
                _ => false
            };

            if (hasInjected)
            {
                assetList.Sort();
                reorderableList.ClearSelection();
                batchInjectorData.IsDirty = true;
            }
        }

        private static void EnableSelected(
            BatchInjectorData batchInjectorData,
            AssetList assetList,
            List<int> selected)
        {
            foreach (int i in selected)
                assetList.GetElementAt(i)
                    .Enabled = true;

            assetList.Sort();
            batchInjectorData.IsDirty = true;
        }

        private static void DisableSelected(
            BatchInjectorData batchInjectorData,
            AssetList assetList,
            List<int> selected)
        {
            foreach (int i in selected)
                assetList.GetElementAt(i)
                    .Enabled = false;

            assetList.Sort();
            batchInjectorData.IsDirty = true;
        }

        private static void ClearSelectedInjectStatus(
            BatchInjectorData batchInjectorData,
            AssetList assetList,
            List<int> selected)
        {
            foreach (int i in selected.OrderByDescending(i => i))
                assetList.GetElementAt(i).Status = InjectionStatus.Unknown;

            assetList.Sort();
            batchInjectorData.IsDirty = true;
            GUI.changed = true;
        }

        private static void RemoveSelected(
            BatchInjectorData batchInjectorData,
            ReorderableList list,
            AssetList assetList,
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
            batchInjectorData.IsDirty = true;
            GUI.changed = true;
        }

        private static void SetSelectedContextFilter(
            BatchInjectorData batchInjectorData,
            AssetList assetList,
            List<int> selected,
            ContextWalkFilter filter)
        {
            foreach (int i in selected)
                assetList.GetElementAt(i).ContextWalkFilter = filter;

            batchInjectorData.IsDirty = true;
        }

        #endregion
    }
}