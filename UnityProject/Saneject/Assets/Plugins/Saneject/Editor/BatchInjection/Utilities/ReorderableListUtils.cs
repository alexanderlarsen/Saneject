using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Editor.BatchInjection.Data;
using Plugins.Saneject.Editor.BatchInjection.Drawers;
using Plugins.Saneject.Editor.BatchInjection.Enums;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.BatchInjection.Utilities
{
    public static class ReorderableListUtils
    {
        private static GUIStyle missingPathLabel;

        public static ReorderableList CreateReorderableList(
            BatchInjectorData injectorData,
            AssetList assetList,
            Action onModified)
        {
            ReorderableList reorderable = new(
                assetList.Elements,
                typeof(AssetData),
                draggable: true,
                displayHeader: false,
                displayAddButton: false,
                displayRemoveButton: true)
            {
                multiSelect = true
            };

            reorderable.drawElementCallback = OnDraw;
            reorderable.onRemoveCallback = OnRemove;
            reorderable.onReorderCallback = OnReorder;

            return reorderable;

            void OnRemove(ReorderableList _)
            {
                if (reorderable.selectedIndices.Count == 0)
                    return;

                List<int> indices = reorderable.selectedIndices?.Distinct()
                    .Where(i => i >= 0 && i < assetList.TotalCount)
                    .OrderByDescending(i => i)
                    .ToList() ?? new List<int>
                {
                    reorderable.index
                };

                if (!EditorUtility.DisplayDialog("Batch Injector", $"Do you want to delete {indices.Count} item{(indices.Count == 1 ? "" : "s")}?", "Yes", "No"))
                    return;

                foreach (int i in indices)
                    assetList.RemoveAt(i);

                reorderable.ClearSelection();
                reorderable.index = Mathf.Clamp(reorderable.index, 0, assetList.TotalCount - 1);
                onModified?.Invoke();
                GUI.changed = true;
            }

            void OnReorder(ReorderableList _)
            {
                assetList.SortMode = SortMode.Custom;
                onModified?.Invoke();
            }

            void OnDraw(
                Rect rect,
                int index,
                bool b1,
                bool b2)
            {
                if (index < 0 || index >= assetList.TotalCount)
                    return;

                const float toggleWidth = 20f;
                const float objWidth = 220f;

                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                AssetData element = assetList.GetElementAt(index);

                bool toggle = EditorGUI.Toggle(new Rect(rect.x + 4, rect.y, toggleWidth, rect.height), element.Enabled);

                if (toggle != element.Enabled)
                {
                    element.Enabled = toggle;
                    assetList.TrySortByEnabledOrDisabled();
                    onModified?.Invoke();
                    GUI.changed = true;
                }

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.ObjectField(new Rect(rect.x + toggleWidth + 2, rect.y, objWidth, rect.height), element.Asset, typeof(Object), false);
                }

                bool hasAsset = element.Asset != null;

                GUIStyle pathLabelStyle = hasAsset
                    ? EditorStyles.miniLabel
                    : missingPathLabel ??= new GUIStyle(EditorStyles.miniLabel)
                    {
                        normal =
                        {
                            textColor = new Color(0.9f, 0.45f, 0.4f, 1f)
                        }
                    };

                string labelText = hasAsset
                    ? element.Path
                    : "Deleted";

                EditorGUI.LabelField(new Rect(rect.x + toggleWidth + objWidth + 7, rect.y, rect.width - objWidth - 40, rect.height), labelText, pathLabelStyle);
                InjectionStatus status = assetList.GetElementAt(index).Status;
                EditorGUI.LabelField(new Rect(rect.width, rect.y, 20, 20), status.GetGuiContent());
            }
        }

        private static GUIContent GetGuiContent(this InjectionStatus status)
        {
            return status switch
            {
                InjectionStatus.Unknown => new GUIContent("❔", "Run injection to get a status"),
                InjectionStatus.Success => new GUIContent("✅", "Succesfully injected"),
                InjectionStatus.Warning => new GUIContent("⚠️", "Injected with warnings. See console for details"),
                InjectionStatus.Error => new GUIContent("❌", "Injection failed with errors. See console for details"),
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }
}