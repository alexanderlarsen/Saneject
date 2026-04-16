using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Enums;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector.Utilities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class DragAndDropUtility
    {
        public static void HandleDragAndDrop(
            Rect dropArea,
            BatchInjectorData batchInjectorData,
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
                AddObjectsToList(batchInjectorData, ref isScene, ref isPrefab);
                SortObjects(batchInjectorData, isScene, isPrefab);
                FindObjectIndices(batchInjectorData, sceneObjectIndices, prefabObjectIndices);
                batchInjectorData.WindowTab = isScene ? WindowTab.Scenes : WindowTab.Prefabs;

                SelectListItems
                (
                    list: isScene
                        ? sceneList
                        : prefabList,
                    indices: isScene
                        ? sceneObjectIndices
                        : prefabObjectIndices
                );

                batchInjectorData.IsDirty = true;
                repaint.Invoke();
                return;
            }

            evt.Use();
        }

        private static void AddObjectsToList(
            BatchInjectorData batchInjectorData,
            ref bool isScene,
            ref bool isPrefab)
        {
            foreach (Object obj in DragAndDrop.objectReferences)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (obj is SceneAsset && path.EndsWith(".unity"))
                {
                    isScene = true;
                    batchInjectorData.SceneList.TryAddAssetByPath<SceneAssetData>(path);
                }
                else if (obj is GameObject && path.EndsWith(".prefab"))
                {
                    isPrefab = true;
                    batchInjectorData.PrefabList.TryAddAssetByPath<PrefabAssetData>(path);
                }
            }
        }

        private static void SortObjects(
            BatchInjectorData batchInjectorData,
            bool isScene,
            bool isPrefab)
        {
            if (isScene)
                batchInjectorData.SceneList.Sort();
            else if (isPrefab)
                batchInjectorData.PrefabList.Sort();
        }

        private static void FindObjectIndices(
            BatchInjectorData batchInjectorData,
            List<int> indices,
            List<int> addedPrefabIndices)
        {
            foreach (Object obj in DragAndDrop.objectReferences)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (obj is SceneAsset && path.EndsWith(".unity"))
                    indices.Add(batchInjectorData.SceneList.FindIndexByPath(path));
                else if (obj is GameObject && path.EndsWith(".prefab"))
                    addedPrefabIndices.Add(batchInjectorData.PrefabList.FindIndexByPath(path));
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