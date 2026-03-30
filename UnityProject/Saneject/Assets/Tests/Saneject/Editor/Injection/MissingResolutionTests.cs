using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Runtime.Settings;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Injection
{
    public class MissingResolutionTests
    {
        private bool originalLogUnusedBindings;

        [SetUp]
        public void SetUp()
        {
            originalLogUnusedBindings = UserSettings.LogUnusedBindings;
            UserSettings.LogUnusedBindings = false;
        }

        [TearDown]
        public void TearDown()
        {
            UserSettings.LogUnusedBindings = originalLogUnusedBindings;
        }

        [Test]
        public void Inject_WHEN_NoMatchingBindingExists_LogsMissingBinding_DoesNotThrowAndLeavesFieldUnset()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");

            // Inject
            Assert.That(() => InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects), Throws.Nothing);

            // Assert
            Assert.That(target.dependency, Is.Null);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Inject_WHEN_BindingResolvesNoCandidates_LogsMissingDependency_DoesNotThrowAndLeavesFieldUnset()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromAnywhere();

            // Inject
            Assert.That(() => InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects), Throws.Nothing);

            // Assert
            Assert.That(target.dependency, Is.Null);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Inject_WHEN_OnlyCollectionBindingExistsForSingleField_LogsMissingBinding_DoesNotThrowAndLeavesFieldUnset()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
            scene.Add<ComponentDependency>("Root 1");
            scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponents<ComponentDependency>().FromSelf();

            // Inject
            Assert.That(() => InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects), Throws.Nothing);

            // Assert
            Assert.That(target.dependency, Is.Null);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Inject_WHEN_OnlySingleBindingExistsForCollectionField_LogsMissingBinding_DoesNotThrowAndLeavesCollectionUnset()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentCollectionTarget target = scene.Add<SingleConcreteComponentCollectionTarget>("Root 1");
            scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            Assert.That(() => InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects), Throws.Nothing);

            // Assert
            Assert.That(target.dependencies, Is.Null);
            LogAssert.NoUnexpectedReceived();
        }
    }
}
