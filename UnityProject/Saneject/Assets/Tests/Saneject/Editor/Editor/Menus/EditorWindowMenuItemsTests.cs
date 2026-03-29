using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.EditorWindows;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector;
using Plugins.Saneject.Editor.Menus.SanejectMenuItems;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Menus
{
    public class EditorWindowMenuItemsTests
    {
        [SetUp]
        public void SetUp()
        {
            foreach (BatchInjectorEditorWindow window in Resources.FindObjectsOfTypeAll<BatchInjectorEditorWindow>())
                window.Close();

            foreach (SettingsEditorWindow window in Resources.FindObjectsOfTypeAll<SettingsEditorWindow>())
                window.Close();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (BatchInjectorEditorWindow window in Resources.FindObjectsOfTypeAll<BatchInjectorEditorWindow>())
                window.Close();

            foreach (SettingsEditorWindow window in Resources.FindObjectsOfTypeAll<SettingsEditorWindow>())
                window.Close();
        }

        [Test]
        public void OpenBatchInjectorWindow_OpensBatchInjectorWindow()
        {
            // Find menu method
            MethodInfo openBatchInjectorWindowMethod = typeof(EditorWindowMenuItems).GetMethod
            (
                "OpenBatchInjectorWindow",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            Assert.That(openBatchInjectorWindowMethod, Is.Not.Null);

            // Open window
            openBatchInjectorWindowMethod.Invoke(null, null);
            BatchInjectorEditorWindow editorWindow = Resources.FindObjectsOfTypeAll<BatchInjectorEditorWindow>().FirstOrDefault();

            // Assert
            Assert.That(editorWindow, Is.Not.Null);
            Assert.That(editorWindow.titleContent.text, Is.EqualTo("Saneject Batch Injector"));
            Assert.That(editorWindow.minSize, Is.EqualTo(new Vector2(420, 400)));
        }

        [Test]
        public void ShowSettings_OpensSettingsWindow()
        {
            // Find menu method
            MethodInfo showSettingsMethod = typeof(EditorWindowMenuItems).GetMethod
            (
                "ShowSettings",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            Assert.That(showSettingsMethod, Is.Not.Null);

            // Open window
            showSettingsMethod.Invoke(null, null);
            SettingsEditorWindow editorWindow = Resources.FindObjectsOfTypeAll<SettingsEditorWindow>().FirstOrDefault();

            // Assert
            Assert.That(editorWindow, Is.Not.Null);
            Assert.That(editorWindow.titleContent.text, Is.EqualTo("Saneject Settings"));
        }
    }
}
