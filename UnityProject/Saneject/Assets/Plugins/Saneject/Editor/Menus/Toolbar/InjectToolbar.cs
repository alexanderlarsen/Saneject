#if UNITY_6000_3_OR_NEWER
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

// ReSharper disable UnusedMember.Local

namespace Plugins.Saneject.Editor.Menus.Toolbar
{
    [EditorBrowsable(EditorBrowsableState.Never), InitializeOnLoad]
    public static class MainToolbarInjectButtons
    {
        private const string Path = "Saneject Inject Toolbar";

        private static readonly MainToolbarButton InjectSceneButton = new
        (
            content: new MainToolbarContent
            {
                text = InjectToolbarData.InjectSceneButtonText,
                tooltip = InjectToolbarData.InjectSceneButtonTooltip
            },
            action: InjectToolbarData.InjectSceneButtonOnClick
        );

        private static readonly MainToolbarButton InjectHierarchiesButton = new
        (
            content: new MainToolbarContent
            {
                text = InjectToolbarData.InjectHierarchyButtonText,
                tooltip = InjectToolbarData.InjectHierarchyButtonTooltip
            },
            action: InjectToolbarData.InjectHierarchyButtonOnClick
        );

        private static readonly MainToolbarButton InjectPrefabButton = new
        (
            content: new MainToolbarContent
            {
                text = InjectToolbarData.InjectPrefabButtonText,
                tooltip = InjectToolbarData.InjectPrefabButtonTooltip
            },
            action: InjectToolbarData.InjectPrefabButtonOnClick
        );

        private static readonly MainToolbarButton BatchInjectButton = new
        (
            content: new MainToolbarContent
            {
                text = InjectToolbarData.BatchInjectButtonText,
                tooltip = InjectToolbarData.BatchInjectButtonTooltip
            },
            action: InjectToolbarData.BatchInjectButtonOnClick
        );

        static MainToolbarInjectButtons()
        {
            Selection.selectionChanged -= UpdateToolbar;
            EditorApplication.hierarchyChanged -= UpdateToolbar;
            EditorApplication.projectChanged -= UpdateToolbar;
            EditorApplication.playModeStateChanged -= UpdateToolbar;

            Selection.selectionChanged += UpdateToolbar;
            EditorApplication.hierarchyChanged += UpdateToolbar;
            EditorApplication.projectChanged += UpdateToolbar;
            EditorApplication.playModeStateChanged += UpdateToolbar;
            
            UpdateToolbar();
        }

        [MainToolbarElement(Path, defaultDockPosition = MainToolbarDockPosition.Left)]
        private static IEnumerable<MainToolbarElement> CreateInjectToolbar()
        {
            return new[]
            {
                InjectSceneButton,
                InjectHierarchiesButton,
                InjectPrefabButton,
                BatchInjectButton
            };
        }

        private static void UpdateToolbar(PlayModeStateChange _)
        {
            UpdateToolbar();
        }

        private static void UpdateToolbar()
        {
            if (InjectSceneButton == null ||
                InjectHierarchiesButton == null ||
                InjectPrefabButton == null ||
                BatchInjectButton == null)
                return;

            InjectToolbarState state = InjectToolbarState.Get();

            InjectSceneButton.enabled = !Application.isPlaying;
            InjectHierarchiesButton.enabled = !Application.isPlaying;
            InjectPrefabButton.enabled = !Application.isPlaying;
            BatchInjectButton.enabled = !Application.isPlaying;

            InjectSceneButton.displayed = state.InjectSceneButtonEnabled;
            InjectHierarchiesButton.displayed = state.InjectHierarchiesButtonEnabled;
            InjectPrefabButton.displayed = state.InjectPrefabButtonEnabled;
            BatchInjectButton.displayed = state.BatchInjectButtonEnabled;

            MainToolbar.Refresh(Path);
        }
    }
}
#endif