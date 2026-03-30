using System;
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

namespace Tests.Saneject.Editor.Binding.Filtering
{
    public class BindingFilterExceptionTests
    {
        private bool originalLogUnusedBindings;
        private bool originalLogInjectionSummary;

        [SetUp]
        public void SetUp()
        {
            originalLogUnusedBindings = UserSettings.LogUnusedBindings;
            originalLogInjectionSummary = UserSettings.LogInjectionSummary;

            UserSettings.LogUnusedBindings = false;
            UserSettings.LogInjectionSummary = false;
        }

        [TearDown]
        public void TearDown()
        {
            UserSettings.LogUnusedBindings = originalLogUnusedBindings;
            UserSettings.LogInjectionSummary = originalLogInjectionSummary;
        }

        [Test]
        public void BindComponent_WHEN_FilterThrows_LogsBindingFilterException_ContinuesRunAndLeavesFieldUnset()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex(@"^Saneject: Binding filter exception \(exception details in next log\)"));
            LogAssert.Expect(LogType.Exception, new Regex("Filter exception"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SuppressedSingleConcreteComponentTarget throwingTarget = scene.Add<SuppressedSingleConcreteComponentTarget>("Root 1");
            SingleConcreteAssetTarget injectedTarget = scene.Add<SingleConcreteAssetTarget>("Root 1");
            scene.Add<ComponentDependency>("Root 1");

            // Find dependency
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .FromSelf()
                .Where(_ => throw new InvalidOperationException("Filter exception"));

            scope.BindAsset<AssetDependency>()
                .FromFolder("Assets/Tests/Saneject/Fixtures/Resources")
                .Where(asset => asset.name == "AssetDependency 1");

            // Inject
            Assert.That(() => InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects), Throws.Nothing);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(throwingTarget.dependency, Is.Null);
            Assert.That(injectedTarget.dependency, Is.EqualTo(dependency));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void BindAsset_WHEN_FilterThrows_LogsBindingFilterException_ContinuesRunAndLeavesFieldUnset()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex(@"^Saneject: Binding filter exception \(exception details in next log\)"));
            LogAssert.Expect(LogType.Exception, new Regex("Filter exception"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SuppressedSingleConcreteAssetTarget throwingTarget = scene.Add<SuppressedSingleConcreteAssetTarget>("Root 1");
            SingleConcreteComponentTarget injectedTarget = scene.Add<SingleConcreteComponentTarget>("Root 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindAsset<AssetDependency>()
                .FromFolder("Assets/Tests/Saneject/Fixtures/Resources")
                .Where(_ => throw new InvalidOperationException("Filter exception"));

            scope.BindComponent<ComponentDependency>()
                .FromSelf();

            // Inject
            Assert.That(() => InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects), Throws.Nothing);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(throwingTarget.dependency, Is.Null);
            Assert.That(injectedTarget.dependency, Is.EqualTo(dependency));
            LogAssert.NoUnexpectedReceived();
        }
    }
}
