using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Binding.Qualification
{
    public class AssetBindingQualifierTests
    {
        [Test]
        public void ToID_TConcrete_InjectsMatchingID_NotWithoutID()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Target without ID
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteAssetTargetWithID matchingTarget = scene.Add<SingleConcreteAssetTargetWithID>("Root 1/Child 1/Child 1");
            SingleConcreteAssetTarget nonMatchingTarget = scene.Add<SingleConcreteAssetTarget>("Root 1/Child 2/Child 1");

            // Find dependency
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");

            // Bind
            scope.BindAsset<AssetDependency>()
                .ToID("qualified")
                .FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(matchingTarget.dependency, Is.EqualTo(dependency));
            Assert.That(nonMatchingTarget.dependency, Is.Null);
        }

        [Test]
        public void ToMember_TConcrete_InjectsMatchingMember_NotDifferentMemberName()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Target with different member name
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteAssetTarget matchingTarget = scene.Add<SingleConcreteAssetTarget>("Root 1/Child 1/Child 1");
            SingleConcreteAssetTargetWithDifferentMemberName nonMatchingTarget = scene.Add<SingleConcreteAssetTargetWithDifferentMemberName>("Root 1/Child 2/Child 1");

            // Find dependency
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");

            // Bind
            scope.BindAsset<AssetDependency>()
                .ToMember("dependency")
                .FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(matchingTarget.dependency, Is.EqualTo(dependency));
            Assert.That(nonMatchingTarget.otherDependency, Is.Null);
        }

        [Test]
        public void ToTarget_Type_TConcrete_InjectsMatchingTarget_NotDifferentTarget()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Different target type
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteAssetTarget matchingTarget = scene.Add<SingleConcreteAssetTarget>("Root 1/Child 1/Child 1");
            SingleConcreteAssetOtherTarget nonMatchingTarget = scene.Add<SingleConcreteAssetOtherTarget>("Root 1/Child 2/Child 1");

            // Find dependency
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");

            // Bind
            scope.BindAsset<AssetDependency>()
                .ToTarget(typeof(SingleConcreteAssetTarget))
                .FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(matchingTarget.dependency, Is.EqualTo(dependency));
            Assert.That(nonMatchingTarget.dependency, Is.Null);
        }

        [Test]
        public void ToTarget_Generic_TConcrete_InjectsMatchingTarget_NotDifferentTarget()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding")); // Different target type
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteAssetOtherTarget matchingTarget = scene.Add<SingleConcreteAssetOtherTarget>("Root 1/Child 1/Child 1");
            SingleConcreteAssetTarget nonMatchingTarget = scene.Add<SingleConcreteAssetTarget>("Root 1/Child 2/Child 1");

            // Find dependency
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");

            // Bind
            scope.BindAsset<AssetDependency>()
                .ToTarget<SingleConcreteAssetOtherTarget>()
                .FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(matchingTarget.dependency, Is.EqualTo(dependency));
            Assert.That(nonMatchingTarget.dependency, Is.Null);
        }
    }
}