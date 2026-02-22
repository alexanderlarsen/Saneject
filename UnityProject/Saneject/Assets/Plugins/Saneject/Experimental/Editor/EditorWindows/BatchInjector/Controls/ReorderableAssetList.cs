using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Controls
{
    public class ReorderableAssetList : ReorderableList
    {
        private static GUIStyle missingPathLabel;

        private readonly AssetList assetList;
        private readonly Action onModified;

        public ReorderableAssetList(
            AssetList assetList,
            Action onModified) :
            base
            (
                assetList.Elements,
                typeof(AssetData),
                draggable: true,
                displayHeader: false,
                displayAddButton: false,
                displayRemoveButton: true
            )
        {
            multiSelect = true;

            this.assetList = assetList;
            this.onModified = onModified;

            drawElementCallback = OnDraw;
            onRemoveCallback = OnRemove;
            onReorderCallback = OnReorder;
        }

        #region Static helpers

        private static GUIContent GetInjectionStatusGUIContent(InjectionStatus status)
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

        #endregion

        #region Callbacks

        private void OnDraw(
            Rect rect,
            int index,
            bool b1,
            bool b2)
        {
            if (index < 0 || index >= assetList.TotalCount)
                return;

            const float toggleWidth = 20f;
            const float objWidth = 220f;

            AssetData element = assetList.GetElementAt(index);

            rect.y += 2;
            rect.height = EditorGUIUtility.singleLineHeight;

            bool toggle = EditorGUI.Toggle
            (
                position: new Rect
                (
                    x: rect.x + 4,
                    y: rect.y,
                    width: toggleWidth,
                    height: rect.height
                ),
                value: element.Enabled
            );

            if (toggle != element.Enabled)
            {
                element.Enabled = toggle;
                assetList.TrySortByEnabledOrDisabled();
                onModified?.Invoke();
                GUI.changed = true;
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.ObjectField
                (
                    position: new Rect
                    (
                        x: rect.x + toggleWidth + 2,
                        y: rect.y,
                        width: objWidth,
                        height: rect.height
                    ),
                    obj: element.Asset,
                    objType: typeof(Object),
                    allowSceneObjects: false
                );
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

            EditorGUI.LabelField
            (
                position: new Rect
                (
                    x: rect.x + toggleWidth + objWidth + 7,
                    y: rect.y,
                    width: rect.width - objWidth - 40,
                    height: rect.height
                ),
                label: labelText,
                style: pathLabelStyle
            );

            InjectionStatus status = assetList.GetElementAt(index).Status;

            EditorGUI.LabelField
            (
                position: new Rect
                (
                    x: rect.width,
                    y: rect.y,
                    width: 20,
                    height: 20
                ),
                label: GetInjectionStatusGUIContent(status)
            );
        }

        private void OnRemove(ReorderableList _)
        {
            if (selectedIndices.Count == 0)
                return;

            List<int> indices = selectedIndices?.Distinct()
                .Where(i => i >= 0 && i < assetList.TotalCount)
                .OrderByDescending(i => i)
                .ToList() ?? new List<int>
            {
                index
            };

            if (!EditorUtility.DisplayDialog
                (
                    title: "Batch Injector",
                    message: $"Remove {indices.Count} selected item{(indices.Count == 1 ? "" : "s")}?",
                    ok: "Yes",
                    cancel: "No")
               )
                return;

            indices.ForEach(i => assetList.RemoveAt(i));
            ClearSelection();
            index = Mathf.Clamp(index, 0, assetList.TotalCount - 1);
            onModified?.Invoke();
            GUI.changed = true;
        }

        private void OnReorder(ReorderableList _)
        {
            assetList.SortMode = SortMode.Custom;
            onModified?.Invoke();
        }

        #endregion
    }
}