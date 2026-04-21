#if UNITY_2022_3_OR_NEWER && !UNITY_6000_3_OR_NEWER
using System;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Menus.Toolbar
{
    [EditorBrowsable(EditorBrowsableState.Never), InitializeOnLoad]
    public static class InjectToolbarButtonsLegacy
    {
        private const string ToolbarZoneName = "ToolbarZoneLeftAlign";
        private const string ContainerName = "Saneject-Inject-MainToolbar";

        private static readonly Type ToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly FieldInfo RootField = ToolbarType?.GetField("m_Root", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly ToolbarButton InjectSceneButton = new(InjectToolbarData.InjectSceneButtonOnClick)
        {
            text = InjectToolbarData.InjectSceneButtonText,
            tooltip = InjectToolbarData.InjectSceneButtonTooltip
        };

        private static readonly ToolbarButton InjectHierarchiesButton = new(InjectToolbarData.InjectHierarchyButtonOnClick)
        {
            text = InjectToolbarData.InjectHierarchyButtonText,
            tooltip = InjectToolbarData.InjectHierarchyButtonTooltip
        };

        private static readonly ToolbarButton InjectPrefabButton = new(InjectToolbarData.InjectPrefabButtonOnClick)
        {
            text = InjectToolbarData.InjectPrefabButtonText,
            tooltip = InjectToolbarData.InjectPrefabButtonTooltip
        };

        private static readonly ToolbarButton BatchInjectButton = new(InjectToolbarData.BatchInjectButtonOnClick)
        {
            text = InjectToolbarData.BatchInjectButtonText,
            tooltip = InjectToolbarData.BatchInjectButtonTooltip
        };

        private static readonly VisualElement Container = CreateInjectToolbar();

        static InjectToolbarButtonsLegacy()
        {
            Selection.selectionChanged -= UpdateToolbar;
            EditorApplication.hierarchyChanged -= UpdateToolbar;
            EditorApplication.projectChanged -= UpdateToolbar;
            EditorApplication.playModeStateChanged -= UpdateToolbar;
            EditorApplication.update -= AttachToolbar;

            Selection.selectionChanged += UpdateToolbar;
            EditorApplication.hierarchyChanged += UpdateToolbar;
            EditorApplication.projectChanged += UpdateToolbar;
            EditorApplication.playModeStateChanged += UpdateToolbar;
            EditorApplication.update += AttachToolbar;

            AttachToolbar();
            UpdateToolbar();
        }

        private static VisualElement CreateInjectToolbar()
        {
            VisualElement root = new() { name = ContainerName };

            root.style.flexDirection = FlexDirection.Row;
            root.style.alignItems = Align.Center;
            root.style.flexShrink = 0;
            root.style.marginRight = 4;
            root.style.display = DisplayStyle.None;

            InjectSceneButton.style.display = DisplayStyle.None;
            InjectHierarchiesButton.style.display = DisplayStyle.None;
            InjectPrefabButton.style.display = DisplayStyle.None;
            BatchInjectButton.style.display = DisplayStyle.None;

            root.Add(InjectSceneButton);
            root.Add(InjectHierarchiesButton);
            root.Add(InjectPrefabButton);
            root.Add(BatchInjectButton);

            return root;
        }

        private static void AttachToolbar()
        {
            if (ToolbarType == null || RootField == null)
                return;

            ScriptableObject toolbar = GetToolbarInstance();

            if (toolbar == null)
                return;

            VisualElement toolbarRoot = RootField.GetValue(toolbar) as VisualElement;
            VisualElement toolbarZone = toolbarRoot?.Q(ToolbarZoneName);

            if (toolbarZone == null)
                return;

            VisualElement existingContainer = toolbarZone.Q(ContainerName);

            if (existingContainer != null && !ReferenceEquals(existingContainer, Container))
                existingContainer.RemoveFromHierarchy();

            if (Container.parent != toolbarZone)
            {
                Container.RemoveFromHierarchy();
                toolbarZone.Add(Container);
            }

            UpdateToolbar();
            EditorApplication.update -= AttachToolbar;
        }

        private static ScriptableObject GetToolbarInstance()
        {
            Object[] toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
            return toolbars.Length > 0 ? toolbars[0] as ScriptableObject : null;
        }

        private static void UpdateToolbar(PlayModeStateChange _)
        {
            UpdateToolbar();
        }

        private static void UpdateToolbar()
        {
            if (Container.parent == null)
                return;

            InjectToolbarState state = InjectToolbarState.Get();

            bool anyEnabled = state.InjectSceneButtonEnabled ||
                              state.InjectHierarchiesButtonEnabled ||
                              state.InjectPrefabButtonEnabled ||
                              state.BatchInjectButtonEnabled;

            Container.style.display =
                anyEnabled
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

            if (!anyEnabled)
                return;

            InjectSceneButton.SetEnabled(!Application.isPlaying);
            InjectHierarchiesButton.SetEnabled(!Application.isPlaying);
            InjectPrefabButton.SetEnabled(!Application.isPlaying);
            BatchInjectButton.SetEnabled(!Application.isPlaying);

            InjectSceneButton.style.display = state.InjectSceneButtonEnabled
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            InjectHierarchiesButton.style.display = state.InjectHierarchiesButtonEnabled
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            InjectPrefabButton.style.display = state.InjectPrefabButtonEnabled
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            BatchInjectButton.style.display = state.BatchInjectButtonEnabled
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
    }
}
#endif