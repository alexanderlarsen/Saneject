using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
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
            ReorderableList list,
            AssetList assetList,
            WindowTab tab,
            Rect rect)
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
                onClick: () => SelectAll(batchInjectorData, list, assetList)
            );

            menu.AddSeparator("");

            menu.AddItem
            (
                label: $"Inject {selected.Count} Selected {(tab == WindowTab.Scenes ? "Scene" : "Prefab")}{(selected.Count == 1 ? "" : "s")}",
                isEnabled: hasSelection,
                onClick: () => InjectSelected(batchInjectorData, assetList, tab, selected)
            );

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
                label: "Remove Selected",
                isEnabled: hasSelection,
                onClick: () => RemoveSelected(batchInjectorData, list, assetList, selected)
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
            BatchInjectorData batchInjectorData,
            ReorderableList list,
            AssetList assetList)
        {
            list.GrabKeyboardFocus();
            list.SelectRange(0, list.count - 1);
            assetList.TrySortByEnabledOrDisabled();
            batchInjectorData.isDirty = true;
        }

        private static void InjectSelected(
            BatchInjectorData batchInjectorData,
            AssetList assetList,
            WindowTab tab,
            List<int> selected)
        {
            AssetData[] selectedAssets = selected
                .Select(assetList.GetElementAt)
                .ToArray();

            switch (tab)
            {
                case WindowTab.Scenes:
                {
                    InjectionUtility.Inject
                    (
                        sceneAssets: selectedAssets,
                        prefabAssets: null,
                        onInjectionComplete: () => batchInjectorData.isDirty = true
                    );

                    break;
                }

                case WindowTab.Prefabs:
                {
                    InjectionUtility.Inject
                    (
                        sceneAssets: null,
                        prefabAssets: selectedAssets,
                        onInjectionComplete: () => batchInjectorData.isDirty = true
                    );

                    break;
                }
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

            assetList.TrySortByEnabledOrDisabled();
            batchInjectorData.isDirty = true;
        }

        private static void DisableSelected(
            BatchInjectorData batchInjectorData,
            AssetList assetList,
            List<int> selected)
        {
            foreach (int i in selected)
                assetList.GetElementAt(i)
                    .Enabled = false;

            assetList.TrySortByEnabledOrDisabled();
            batchInjectorData.isDirty = true;
        }

        private static void ClearSelectedInjectStatus(
            BatchInjectorData batchInjectorData,
            AssetList assetList,
            List<int> selected)
        {
            foreach (int i in selected.OrderByDescending(i => i))
                assetList.GetElementAt(i).Status = InjectionStatus.Unknown;

            batchInjectorData.isDirty = true;
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
            batchInjectorData.isDirty = true;
            GUI.changed = true;
        }

        #endregion
    }
}