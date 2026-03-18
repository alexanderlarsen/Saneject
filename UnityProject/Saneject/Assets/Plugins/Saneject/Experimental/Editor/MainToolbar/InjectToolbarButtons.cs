using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.MainToolbar
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
                text: "Inject Selected",
                tooltip: "Inject everything in the selected scene hierarchies.",
                onClicked: () => InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.AllContexts)
            );

            injectPrefabButton = CreateButton
            (
                text: "Inject Prefab Asset",
                tooltip: "Injects everything in the current prefab asset.",
                onClicked: () => InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.AllContexts)
            );

            root.Add(injectSceneButton);
            root.Add(injectSelectionButton);
            root.Add(injectPrefabButton);

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
            ToolbarMode mode = PrefabStageUtility.GetCurrentPrefabStage() != null
                ? ToolbarMode.Prefab
                : SceneManager.sceneCount > 0
                    ? ToolbarMode.Scene
                    : ToolbarMode.None;

            int sceneObjectSelectionCount = Selection
                .gameObjects
                .Count(gameObject => gameObject.scene.IsValid());

            return new ToolbarState(mode, sceneObjectSelectionCount);
        }

        private static void ApplyToolbarState(ToolbarState toolbarState)
        {
            if (container == null)
                return;

            SetDisplay(container, toolbarState.Mode == ToolbarMode.None ? DisplayStyle.None : DisplayStyle.Flex);
            SetDisplay(injectSceneButton, toolbarState.Mode == ToolbarMode.Scene ? DisplayStyle.Flex : DisplayStyle.None);

            SetDisplay
            (
                injectSelectionButton,
                toolbarState is { Mode: ToolbarMode.Scene, SceneObjectSelectionCount: > 0 }
                    ? DisplayStyle.Flex
                    : DisplayStyle.None
            );

            SetDisplay(injectPrefabButton, toolbarState.Mode == ToolbarMode.Prefab ? DisplayStyle.Flex : DisplayStyle.None); 
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
                int sceneObjectSelectionCount)
            {
                Mode = mode;
                SceneObjectSelectionCount = sceneObjectSelectionCount;
            }

            public ToolbarMode Mode { get; }
            public int SceneObjectSelectionCount { get; }
        }
    }
}