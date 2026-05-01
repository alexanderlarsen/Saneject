using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Binding.Qualification
{
    public class ComponentBindingQualifierTests
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
            SingleConcreteComponentTargetWithID matchingTarget = scene.Add<SingleConcreteComponentTargetWithID>("Root 1/Child 1/Child 1");
            SingleConcreteComponentTarget nonMatchingTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .ToID("qualified")
                .FromScopeSelf();

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
            SingleConcreteComponentTarget matchingTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1/Child 1");
            SingleConcreteComponentTargetWithDifferentMemberName nonMatchingTarget = scene.Add<SingleConcreteComponentTargetWithDifferentMemberName>("Root 1/Child 2/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .ToMember("dependency")
                .FromScopeSelf();

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
            SingleConcreteComponentTarget matchingTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1/Child 1");
            SingleConcreteComponentOtherTarget nonMatchingTarget = scene.Add<SingleConcreteComponentOtherTarget>("Root 1/Child 2/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .ToTarget(typeof(SingleConcreteComponentTarget))
                .FromScopeSelf();

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
            SingleConcreteComponentOtherTarget matchingTarget = scene.Add<SingleConcreteComponentOtherTarget>("Root 1/Child 1/Child 1");
            SingleConcreteComponentTarget nonMatchingTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .ToTarget<SingleConcreteComponentOtherTarget>()
                .FromScopeSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(matchingTarget.dependency, Is.EqualTo(dependency));
            Assert.That(nonMatchingTarget.dependency, Is.Null);
        }

        [Test]
        public void ToTarget_Generic_TConcrete_InjectsNestedSerializableTarget()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            NestedRootTarget target = scene.Add<NestedRootTarget>("Root 1");

            // Find dependency
            AssetDependency assetDependency = Resources.Load<AssetDependency>("AssetDependency 1");
            ComponentDependency singleDependency = scene.Add<ComponentDependency>("Root 1");

            ComponentDependency[] dependencies =
            {
                singleDependency,
                scene.Add<ComponentDependency>("Root 1/Child 1")
            };

            // Bind
            scope.BindAsset<AssetDependency>()
                .ToTarget<NestedChildTarget>()
                .FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            scope.BindAsset<AssetDependency>()
                .ToID("nested-method-id")
                .ToTarget<NestedChildTarget>()
                .FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            scope.BindComponents<ComponentDependency>()
                .ToID("nested-method-id")
                .ToTarget<NestedChildTarget>()
                .FromDescendants(includeSelf: true);

            scope.BindComponent<IDependency>()
                .ToID("nested-method-id")
                .ToTarget<NestedChildTarget>()
                .FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(assetDependency, Is.Not.Null);
            Assert.That(target.nested.nestedFieldDependency, Is.EqualTo(assetDependency));
            Assert.That(target.nested.NestedMethodAssetDependency, Is.EqualTo(assetDependency));

            CollectionAssert.AreEquivalent
            (
                dependencies,
                target.nested.NestedMethodComponentDependencies
            );

            Assert.That(target.nested.NestedMethodInterfaceDependency, Is.EqualTo(singleDependency));
        }
    }
}
