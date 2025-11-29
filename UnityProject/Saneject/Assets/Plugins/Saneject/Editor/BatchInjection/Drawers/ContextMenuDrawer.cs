using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Editor.BatchInjection.Core;
using Plugins.Saneject.Editor.BatchInjection.Data;
using Plugins.Saneject.Editor.BatchInjection.Enums;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.BatchInjection.Drawers
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

            GenericMenu menu = new();
            List<int> selected = list.selectedIndices?.ToList() ?? new List<int>();

            bool hasSelection = selected.Count > 0;

            AddItem
            (
                menu: menu,
                label: "Select All",
                isEnabled: selected.Count != list.count,
                onClick: () =>
                {
                    list.GrabKeyboardFocus();
                    list.SelectRange(0, list.count - 1);
                    assetList.TrySortByEnabledOrDisabled();
                    onModified.Invoke();
                }
            );

            menu.AddSeparator("");

            AddItem
            (
                menu: menu,
                label: $"Inject {selected.Count} Selected {(tab == WindowTab.Scenes ? "Scene" : "Prefab")}{(selected.Count == 1 ? "" : "s")}",
                isEnabled: hasSelection,
                onClick: () =>
                {
                    AssetData[] assets = selected
                        .Select(assetList.GetElementAt)
                        .ToArray();

                    switch (tab)
                    {
                        case WindowTab.Scenes:
                            if (!Dialogs.BatchInjection.ConfirmInjectScene(assets.Length))
                                break;

                            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                                break;

                            if (UserSettings.ClearLogsOnInjection)
                                ConsoleUtils.ClearLog();

                            BatchInjector.BatchInjectScenes(assets, true);
                            break;

                        case WindowTab.Prefabs:
                            if (!Dialogs.BatchInjection.ConfirmInjectPrefab(assets.Length))
                                break;

                            if (UserSettings.ClearLogsOnInjection)
                                ConsoleUtils.ClearLog();

                            BatchInjector.BatchInjectPrefabs(assets, true);
                            break;
                    }
                });

            menu.AddSeparator("");

            AddItem
            (
                menu: menu,
                label: "Find in Project",
                isEnabled: selected.Count == 1,
                onClick: () =>
                {
                    foreach (int i in selected)
                    {
                        AssetData data = assetList.GetElementAt(i);
                        Object asset = data.Asset;

                        if (asset)
                            EditorGUIUtility.PingObject(asset);
                    }
                }
            );

            menu.AddSeparator("");

            AddItem
            (
                menu: menu,
                label: "Enable",
                isEnabled: selected.Count(i => assetList.GetElementAt(i).Enabled) != selected.Count,
                onClick: () =>
                {
                    foreach (int i in selected)
                        assetList.GetElementAt(i)
                            .Enabled = true;

                    assetList.TrySortByEnabledOrDisabled();
                    onModified?.Invoke();
                }
            );

            AddItem
            (
                menu: menu,
                label: "Disable",
                isEnabled: selected.Count(i => assetList.GetElementAt(i).Enabled) != 0,
                onClick: () =>
                {
                    foreach (int i in selected)
                        assetList.GetElementAt(i)
                            .Enabled = false;

                    assetList.TrySortByEnabledOrDisabled();
                    onModified?.Invoke();
                }
            );

            menu.AddSeparator("");

            AddItem
            (
                menu: menu,
                label: "Clear Injection Status",
                isEnabled: hasSelection,
                onClick: () =>
                {
                    foreach (int i in selected.OrderByDescending(i => i))
                        assetList.GetElementAt(i).Status = InjectionStatus.Unknown;

                    onModified?.Invoke();
                    GUI.changed = true;
                }
            );

            menu.AddSeparator("");

            AddItem
            (
                menu: menu,
                label: "Remove Selected",
                isEnabled: hasSelection,
                onClick: () =>
                {
                    if (!EditorUtility.DisplayDialog("Batch Injector",
                            $"Remove {selected.Count} selected item{(selected.Count == 1 ? "" : "s")}?",
                            "Yes",
                            "No"))
                        return;

                    foreach (int i in selected.OrderByDescending(i => i))
                        assetList.RemoveAt(i);

                    list.ClearSelection();
                    onModified?.Invoke();
                    GUI.changed = true;
                }
            );

            menu.ShowAsContext();
            e.Use();
        }

        private static void AddItem(
            GenericMenu menu,
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
    }
}