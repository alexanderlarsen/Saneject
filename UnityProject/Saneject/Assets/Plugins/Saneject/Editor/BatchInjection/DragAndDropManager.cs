using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.BatchInjection
{
    public static class DragAndDropManager
    {
        public static void HandleDragAndDrop(
            Rect dropArea,
            BatchInjectorData data,
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
                AddObjectsToList(ref isScene, ref isPrefab);
                SortObjects(isScene, isPrefab);
                FindObjectIndices(sceneObjectIndices, prefabObjectIndices);
                data.windowTab = isScene ? WindowTab.Scenes : WindowTab.Prefabs;

                SelectListItems
                (
                    list: isScene
                        ? sceneList
                        : prefabList,
                    indices: isScene
                        ? sceneObjectIndices
                        : prefabObjectIndices
                );

                Storage.SaveData(data);
                repaint.Invoke();
                return;
            }

            evt.Use();

            return;

            void AddObjectsToList(
                ref bool isScene,
                ref bool isPrefab)
            {
                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    string path = AssetDatabase.GetAssetPath(obj);

                    if (obj is SceneAsset && path.EndsWith(".unity"))
                    {
                        isScene = true;
                        data.sceneList.TryAddByPath(path);
                    }
                    else if (obj is GameObject && path.EndsWith(".prefab"))
                    {
                        isPrefab = true;
                        data.prefabList.TryAddByPath(path);
                    }
                }
            }

            void SortObjects(
                bool isScene,
                bool isPrefab)
            {
                if (isScene)
                    data.sceneList.Sort();
                else if (isPrefab)
                    data.prefabList.Sort();
            }

            void FindObjectIndices(
                List<int> ints,
                List<int> addedPrefabIndices)
            {
                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    string path = AssetDatabase.GetAssetPath(obj);

                    if (obj is SceneAsset && path.EndsWith(".unity"))
                        ints.Add(data.sceneList.FindIndexByPath(path));
                    else if (obj is GameObject && path.EndsWith(".prefab"))
                        addedPrefabIndices.Add(data.prefabList.FindIndexByPath(path));
                }
            }

            void SelectListItems(
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
}