using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
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
        private static GUIStyle toggleStyle;
        private static GUIStyle defaultPathLabelStyle;
        private static GUIStyle missingPathLabelStyle;
        private static GUIStyle statusLabelStyle;

        private readonly AssetList assetList;
        private readonly Action onModified;

        public ReorderableAssetList(
            AssetList assetList,
            Action onModified) :
            base
            (
                assetList.List,
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

        #region Callbacks

        private void OnDraw(
            Rect rect,
            int index,
            bool b1,
            bool b2)
        {
            CreateStyles();

            if (index < 0 || index >= assetList.TotalCount)
                return;

            AssetData assetData = assetList.GetElementAt(index);
            SceneAssetData sceneAssetData = assetData as SceneAssetData;
            Object elementAsset = assetData.GetAsset();
            bool isSceneList = sceneAssetData != null;

            rect.y += 1.5f;
            rect.height = EditorGUIUtility.singleLineHeight;

            const float space = 6f;
            const float toggleWidth = 14f;
            const float objectFieldWidth = 220f;
            const float contextWalkFilterWidth = 118f;
            const float statusWidth = 18f;

            float x = rect.x - 4 + space;
            float y = rect.y;
            float height = rect.height;

            DrawToggle(assetData, x, y, toggleWidth, height);
            x += toggleWidth + space;

            DrawObjectField(elementAsset, x, y, objectFieldWidth, height);
            x += objectFieldWidth + space;

            float rightReserved = statusWidth;

            if (isSceneList)
                rightReserved += space + contextWalkFilterWidth;

            rightReserved += space;

            float pathLabelWidth = rect.xMax - x - rightReserved;

            if (pathLabelWidth < 0f)
                pathLabelWidth = 0f;

            DrawPathLabel(assetData, elementAsset, x, y, pathLabelWidth, height);
            x += pathLabelWidth + space;

            if (isSceneList)
            {
                DrawContextWalkFilter(sceneAssetData, x, y, contextWalkFilterWidth, height);
                x += contextWalkFilterWidth + space;
            }

            DrawStatusLabel(assetData, x, y, statusWidth, height);
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

        #region Draw methods

        private void DrawToggle(
            AssetData assetData,
            float x,
            float y,
            float width,
            float height)
        {
            bool enabled = EditorGUI.Toggle
            (
                position: new Rect(x, y, width, height),
                value: assetData.Enabled,
                style: toggleStyle
            );

            if (enabled == assetData.Enabled)
                return;

            assetData.Enabled = enabled;
            assetList.TrySortByEnabledOrDisabled();
            onModified?.Invoke();
            GUI.changed = true;
        }

        private static void DrawObjectField(
            Object asset,
            float x,
            float y,
            float width,
            float height)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.ObjectField
                (
                    position: new Rect(x, y, width, height),
                    obj: asset,
                    objType: typeof(Object),
                    allowSceneObjects: false
                );
            }
        }

        private static void DrawPathLabel(
            AssetData assetData,
            Object asset,
            float x,
            float y,
            float width,
            float height)
        {
            bool hasAsset = asset != null;

            GUIStyle pathLabelStyle = hasAsset
                ? defaultPathLabelStyle
                : missingPathLabelStyle;

            string pathLabelText = hasAsset
                ? assetData.GetAssetPath()
                : "Deleted";

            EditorGUI.LabelField
            (
                position: new Rect(x, y, width, height),
                label: pathLabelText,
                style: pathLabelStyle
            );
        }

        private void DrawContextWalkFilter(
            SceneAssetData sceneAssetData,
            float x,
            float y,
            float width,
            float height)
        {
            if (sceneAssetData == null)
                return;

            Rect popupRect = new(x, y, width, height);

            GUIContent content = new
            (
                ObjectNames.NicifyVariableName(sceneAssetData.ContextWalkFilter.ToString()),
                "Context walk filter: Controls which contexts are included in the walk when injecting this scene"
            );

            // EditorGUI.EnumPopup() does not reliably show tooltips in this context,
            // so a custom DropdownButton + GenericMenu is used instead.
            if (EditorGUI.DropdownButton(popupRect, content, FocusType.Passive))
            {
                GenericMenu menu = new();

                foreach (ContextWalkFilter value in Enum.GetValues(typeof(ContextWalkFilter)))
                {
                    if (value is ContextWalkFilter.SameContextsAsSelection or ContextWalkFilter.PrefabAssetObjects)
                        continue;

                    ContextWalkFilter selected = value;

                    menu.AddItem
                    (
                        content: new GUIContent(ObjectNames.NicifyVariableName(value.ToString())),
                        on: value == sceneAssetData.ContextWalkFilter,
                        func: () =>
                        {
                            sceneAssetData.ContextWalkFilter = selected;
                            onModified?.Invoke();
                        }
                    );
                }

                menu.DropDown(popupRect);
            }
        }

        private static void DrawStatusLabel(
            AssetData assetData,
            float x,
            float y,
            float width,
            float height)
        {
            InjectionStatus status = assetData.Status;

            EditorGUI.LabelField
            (
                position: new Rect(x, y, width, height),
                label: GetInjectionStatusGUIContent(status),
                style: statusLabelStyle
            );
        }

        #endregion

        #region Static helpers

        private static void CreateStyles()
        {
            toggleStyle ??= new GUIStyle(EditorStyles.toggle)
            {
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            defaultPathLabelStyle ??= new GUIStyle(EditorStyles.miniLabel)
            {
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            missingPathLabelStyle ??= new GUIStyle(EditorStyles.miniLabel)
            {
                normal =
                {
                    textColor = new Color(0.9f, 0.45f, 0.4f, 1f)
                },
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            statusLabelStyle ??= new GUIStyle(EditorStyles.label)
            {
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };
        }

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
    }
}