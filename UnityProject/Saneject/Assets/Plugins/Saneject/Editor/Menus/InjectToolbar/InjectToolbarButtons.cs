using System;
using System.ComponentModel;
using System.Reflection;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Menus.InjectToolbar
{
    [EditorBrowsable(EditorBrowsableState.Never), InitializeOnLoad]
    public static class InjectMainToolbarButton
    {
        private const string ToolbarZoneName = "ToolbarZoneLeftAlign";
        private const string ContainerName = "Saneject-Inject-MainToolbar";

        private static readonly Type ToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly FieldInfo RootField = ToolbarType?.GetField("m_Root", BindingFlags.Instance | BindingFlags.NonPublic);

        private static VisualElement container;
        private static ToolbarButton injectSceneButton;
        private static ToolbarButton injectSelectionButton;
        private static ToolbarButton injectPrefabButton;
        private static ToolbarButton batchInjectSelectedAssetsButton;

        static InjectMainToolbarButton()
        {
            EditorApplication.update += UpdateToolbar;
        }

        private enum ToolbarMode
        {
            None,
            Scene,
            Prefab
        }

        private static void UpdateToolbar()
        {
            if (ToolbarType == null || RootField == null)
                return;

            if (!TryGetToolbarZone(out VisualElement toolbarZone))
                return;

            EnsureAttached(toolbarZone);
            ApplyToolbarState(GetToolbarState());
        }

        private static bool TryGetToolbarZone(out VisualElement toolbarZone)
        {
            toolbarZone = null;
            ScriptableObject toolbar = GetToolbarInstance();

            if (toolbar == null)
                return false;

            VisualElement toolbarRoot = RootField.GetValue(toolbar) as VisualElement;
            toolbarZone = toolbarRoot?.Q(ToolbarZoneName);
            return toolbarZone != null;
        }

        private static ScriptableObject GetToolbarInstance()
        {
            Object[] toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
            return toolbars.Length > 0 ? toolbars[0] as ScriptableObject : null;
        }

        private static void EnsureAttached(VisualElement toolbarZone)
        {
            VisualElement existingContainer = toolbarZone.Q(ContainerName);

            if (existingContainer != null && !ReferenceEquals(existingContainer, container))
                existingContainer.RemoveFromHierarchy();

            container ??= CreateButtonContainer();

            if (container.parent != toolbarZone)
            {
                container.RemoveFromHierarchy();
                toolbarZone.Add(container);
            }
        }

        private static VisualElement CreateButtonContainer()
        {
            VisualElement root = new()
            {
                name = ContainerName
            };

            root.style.flexDirection = FlexDirection.Row;
            root.style.alignItems = Align.Center;
            root.style.flexShrink = 0;
            root.style.marginRight = 4;

            injectSceneButton = CreateButton
            (
                text: "Inject Scene",
                tooltip: "Injects everything in the current scene.",
                onClicked: () => InjectionUtility.InjectCurrentScene(ContextWalkFilter.AllContexts)
            );

            injectSelectionButton = CreateButton
            (
                text: "Inject Selected Scene Hierarchies",
                tooltip: "Inject everything in the selected scene hierarchies.",
                onClicked: () => InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.AllContexts)
            );

            injectPrefabButton = CreateButton
            (
                text: "Inject Prefab Asset",
                tooltip: "Injects everything in the current prefab asset.",
                onClicked: () => InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.AllContexts)
            );

            batchInjectSelectedAssetsButton = CreateButton
            (
                text: "Batch Inject Selected Assets",
                tooltip: "Batch injects the selected folders, scene assets, and prefab assets.",
                onClicked: InjectionUtility.BatchInjectSelectedAssets
            );

            root.Add(injectSceneButton);
            root.Add(injectSelectionButton);
            root.Add(injectPrefabButton);
            root.Add(batchInjectSelectedAssetsButton);

            return root;
        }

        private static ToolbarButton CreateButton(
            string text,
            string tooltip,
            Action onClicked)
        {
            ToolbarButton button = new()
            {
                text = text,
                tooltip = tooltip
            };

            button.clicked += onClicked;
            return button;
        }

        private static ToolbarState GetToolbarState()
        {
            ToolbarMode mode =
                MenuValidator.IsPrefabStage()
                    ? ToolbarMode.Prefab
                    : MenuValidator.IsScene()
                        ? ToolbarMode.Scene
                        : ToolbarMode.None;

            int sceneObjectSelectionCount = MenuValidator.GetSceneObjectSelectionCount();
            bool hasBatchInjectAssetSelection = MenuValidator.HasValidBatchSelection();

            return new ToolbarState(mode, sceneObjectSelectionCount, hasBatchInjectAssetSelection);
        }

        private static void ApplyToolbarState(ToolbarState toolbarState)
        {
            if (container == null)
                return;

            bool hasVisibleControls =
                toolbarState.Mode != ToolbarMode.None ||
                toolbarState.HasBatchInjectAssetSelection;

            SetDisplay
            (
                container,
                hasVisibleControls
                    ? DisplayStyle.Flex
                    : DisplayStyle.None
            );

            SetDisplay
            (
                injectSceneButton,
                toolbarState.Mode == ToolbarMode.Scene
                    ? DisplayStyle.Flex
                    : DisplayStyle.None
            );

            SetDisplay
            (
                injectSelectionButton,
                toolbarState is { Mode: ToolbarMode.Scene, SceneObjectSelectionCount: > 0 }
                    ? DisplayStyle.Flex
                    : DisplayStyle.None
            );

            SetDisplay
            (
                injectPrefabButton,
                toolbarState.Mode == ToolbarMode.Prefab
                    ? DisplayStyle.Flex
                    : DisplayStyle.None
            );

            SetDisplay
            (
                batchInjectSelectedAssetsButton,
                toolbarState.HasBatchInjectAssetSelection
                    ? DisplayStyle.Flex
                    : DisplayStyle.None
            );
        }

        private static void SetDisplay(
            VisualElement element,
            DisplayStyle displayStyle)
        {
            if (element != null)
                element.style.display = displayStyle;
        }

        private readonly struct ToolbarState
        {
            public ToolbarState(
                ToolbarMode mode,
                int sceneObjectSelectionCount,
                bool hasBatchInjectAssetSelection)
            {
                Mode = mode;
                SceneObjectSelectionCount = sceneObjectSelectionCount;
                HasBatchInjectAssetSelection = hasBatchInjectAssetSelection;
            }

            public ToolbarMode Mode { get; }
            public int SceneObjectSelectionCount { get; }
            public bool HasBatchInjectAssetSelection { get; }
        }
    }
}