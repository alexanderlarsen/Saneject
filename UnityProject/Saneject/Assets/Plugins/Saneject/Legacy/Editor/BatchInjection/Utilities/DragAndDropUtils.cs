using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Data;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Enums;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Persistence;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Legacy.Editor.BatchInjection.Utilities
{
    public static class DragAndDropUtils
    {
        public static void HandleDragAndDrop(
            Rect dropArea,
            BatchInjectorData injectorData,
            ReorderableList sceneList,
            ReorderableList prefabList,
            Action repaint)
        {
            Event evt = Event.current;

            if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform)
                return;

            Rect windowRect = new(0, 0, dropArea.width, dropArea.height);

            if (!windowRect.Contains(evt.mousePosition))
                return;

            bool hasSceneOrPrefab = DragAndDrop.objectReferences.Any(o => o is SceneAsset or GameObject);
            DragAndDrop.visualMode = hasSceneOrPrefab ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

            List<int> prefabObjectIndices = new();
            List<int> sceneObjectIndices = new();

            if (evt.type == EventType.DragPerform && hasSceneOrPrefab)
            {
                bool isScene = false;
                bool isPrefab = false;

                DragAndDrop.AcceptDrag();
                AddObjectsToList(injectorData, ref isScene, ref isPrefab);
                SortObjects(injectorData, isScene, isPrefab);
                FindObjectIndices(injectorData, sceneObjectIndices, prefabObjectIndices);
                injectorData.windowTab = isScene ? WindowTab.Scenes : WindowTab.Prefabs;

                SelectListItems
                (
                    list: isScene
                        ? sceneList
                        : prefabList,
                    indices: isScene
                        ? sceneObjectIndices
                        : prefabObjectIndices
                );

                Storage.SaveData(injectorData);
                repaint.Invoke();
                return;
            }

            evt.Use();
        }

        private static void AddObjectsToList(
            BatchInjectorData injectorData,
            ref bool isScene,
            ref bool isPrefab)
        {
            foreach (Object obj in DragAndDrop.objectReferences)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (obj is SceneAsset && path.EndsWith(".unity"))
                {
                    isScene = true;
                    injectorData.sceneList.TryAddByPath(path);
                }
                else if (obj is GameObject && path.EndsWith(".prefab"))
                {
                    isPrefab = true;
                    injectorData.prefabList.TryAddByPath(path);
                }
            }
        }

        private static void SortObjects(
            BatchInjectorData injectorData,
            bool isScene,
            bool isPrefab)
        {
            if (isScene)
                injectorData.sceneList.Sort();
            else if (isPrefab)
                injectorData.prefabList.Sort();
        }

        private static void FindObjectIndices(
            BatchInjectorData injectorData,
            List<int> ints,
            List<int> addedPrefabIndices)
        {
            foreach (Object obj in DragAndDrop.objectReferences)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (obj is SceneAsset && path.EndsWith(".unity"))
                    ints.Add(injectorData.sceneList.FindIndexByPath(path));
                else if (obj is GameObject && path.EndsWith(".prefab"))
                    addedPrefabIndices.Add(injectorData.prefabList.FindIndexByPath(path));
            }
        }

        private static void SelectListItems(
            ReorderableList list,
            List<int> indices)
        {
            if (indices.Count == 0)
                return;

            int min = indices.Min();
            int max = indices.Max();

            list.ClearSelection();
            list.SelectRange(min, max);
            list.GrabKeyboardFocus();

            for (int i = min; i <= max; i++)
                if (!indices.Contains(i))
                    list.Deselect(i);
        }
    }
}