using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Plugins.Saneject.Runtime.Settings;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Binding.Validation
{
    public class UnusedBindingsTests
    {
        private bool originalLogUnusedBindings;
        private bool originalLogInjectionSummary;

        [SetUp]
        public void SetUp()
        {
            originalLogUnusedBindings = UserSettings.LogUnusedBindings;
            originalLogInjectionSummary = UserSettings.LogInjectionSummary;

            UserSettings.LogUnusedBindings = true;
            UserSettings.LogInjectionSummary = false;
        }

        [TearDown]
        public void TearDown()
        {
            UserSettings.LogUnusedBindings = originalLogUnusedBindings;
            UserSettings.LogInjectionSummary = originalLogInjectionSummary;
        }

        [Test]
        public void Binding_WHEN_Unused_LogsUnusedBindingWarning()
        {
            // Expect logs
            LogAssert.Expect(LogType.Warning, new Regex("^Saneject: Unused binding"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Binding_WHEN_Used_DoesNotLogUnusedBindingWarning()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Binding_WHEN_Invalid_DoesNotLogUnusedBindingWarning()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromRuntimeProxy();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Binding_WHEN_LogUnusedBindingsIsDisabled_DoesNotLogUnusedBindingWarning()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");
            UserSettings.LogUnusedBindings = false;

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            LogAssert.NoUnexpectedReceived();
        }
    }
}
