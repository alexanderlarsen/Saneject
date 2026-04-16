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
    public class SuppressedMissingErrorsTests
    {
        private bool originalLogInjectionSummary;

        [SetUp]
        public void SetUp()
        {
            originalLogInjectionSummary = UserSettings.LogInjectionSummary;
            UserSettings.LogInjectionSummary = true;
        }

        [TearDown]
        public void TearDown()
        {
            UserSettings.LogInjectionSummary = originalLogInjectionSummary;
        }

        [Test]
        public void Inject_WHEN_MissingBindingIsSuppressed_DoesNotLogMissingBinding_AndSummaryReportsSuppressedError()
        {
            // Expect logs
            LogAssert.Expect(LogType.Log, new Regex("^Saneject: Injection complete(?s:.*)1 missing binding/dependency error was suppressed due to \\[Inject\\(suppressMissingErrors: true\\)\\]"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.Add<TestScope>("Root 1");
            SuppressedSingleConcreteComponentTarget target = scene.Add<SuppressedSingleConcreteComponentTarget>("Root 1");

            // Inject
            Assert.That(() => InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects), Throws.Nothing);

            // Assert
            Assert.That(target.dependency, Is.Null);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Inject_WHEN_MissingDependencyIsSuppressed_DoesNotLogMissingDependency_AndSummaryReportsSuppressedError()
        {
            // Expect logs
            LogAssert.Expect(LogType.Log, new Regex("^Saneject: Injection complete(?s:.*)1 missing binding/dependency error was suppressed due to \\[Inject\\(suppressMissingErrors: true\\)\\]"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SuppressedSingleConcreteComponentTarget target = scene.Add<SuppressedSingleConcreteComponentTarget>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            Assert.That(() => InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects), Throws.Nothing);

            // Assert
            Assert.That(target.dependency, Is.Null);
            LogAssert.NoUnexpectedReceived();
        }
    }
}
