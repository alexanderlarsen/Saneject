using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Injection
{
    public class NestedSerializableInjectionTests
    {
        [Test]
        public void Inject_Field_InjectsToNestedAndDeepNestedFields()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            GraphMetadataTarget target = scene.Add<GraphMetadataTarget>("Root 1");

            // Find dependencies
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");
            scene.Add<ComponentDependency>("Root 1");
            scene.Add<ComponentDependency>("Root 1/Child 1");

            // Bind
            scope.BindAsset<AssetDependency>().FromResources("AssetDependency 1");
            scope.BindAsset<AssetDependency>()
                .ToID("nested-property-id", "deep-field-id", "nested-method-id")
                .FromResources("AssetDependency 1");
            scope.BindComponent<ComponentDependency>()
                .ToID("field-id", "method-id")
                .FromSelf();
            scope.BindComponents<ComponentDependency>()
                .ToID("property-id", "method-id", "nested-method-id")
                .FromDescendants(includeSelf: true);
            scope.BindComponent<IDependency>()
                .ToID("method-id", "nested-method-id")
                .FromSelf();
            scope.BindComponents<IDependency>()
                .ToID("method-id")
                .FromDescendants(includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.nested, Is.Not.Null);
            Assert.That(target.nested.deepNested, Is.Not.Null);
            Assert.That(target.nested.nestedFieldDependency, Is.EqualTo(dependency));
            Assert.That(target.nested.deepNested.deepFieldDependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Inject_Property_InjectsToNestedProperty()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            GraphMetadataTarget target = scene.Add<GraphMetadataTarget>("Root 1");

            // Find dependencies
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");
            scene.Add<ComponentDependency>("Root 1");
            scene.Add<ComponentDependency>("Root 1/Child 1");

            // Bind
            scope.BindAsset<AssetDependency>().FromResources("AssetDependency 1");
            scope.BindAsset<AssetDependency>()
                .ToID("nested-property-id", "deep-field-id", "nested-method-id")
                .FromResources("AssetDependency 1");
            scope.BindComponent<ComponentDependency>()
                .ToID("field-id", "method-id")
                .FromSelf();
            scope.BindComponents<ComponentDependency>()
                .ToID("property-id", "method-id", "nested-method-id")
                .FromDescendants(includeSelf: true);
            scope.BindComponent<IDependency>()
                .ToID("method-id", "nested-method-id")
                .FromSelf();
            scope.BindComponents<IDependency>()
                .ToID("method-id")
                .FromDescendants(includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.nested, Is.Not.Null);
            Assert.That(target.nested.NestedPropertyDependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Inject_Method_InjectsToNestedMethodParameters()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            GraphMetadataTarget target = scene.Add<GraphMetadataTarget>("Root 1");

            // Find dependencies
            AssetDependency assetDependency = Resources.Load<AssetDependency>("AssetDependency 1");
            ComponentDependency singleDependency = scene.Add<ComponentDependency>("Root 1");
            ComponentDependency[] dependencies =
            {
                singleDependency,
                scene.Add<ComponentDependency>("Root 1/Child 1")
            };

            // Bind
            scope.BindAsset<AssetDependency>().FromResources("AssetDependency 1");
            scope.BindAsset<AssetDependency>()
                .ToID("nested-property-id", "deep-field-id", "nested-method-id")
                .FromResources("AssetDependency 1");
            scope.BindComponent<ComponentDependency>()
                .ToID("field-id", "method-id")
                .FromSelf();
            scope.BindComponents<ComponentDependency>()
                .ToID("property-id", "method-id", "nested-method-id")
                .FromDescendants(includeSelf: true);
            scope.BindComponent<IDependency>()
                .ToID("method-id", "nested-method-id")
                .FromSelf();
            scope.BindComponents<IDependency>()
                .ToID("method-id")
                .FromDescendants(includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(assetDependency, Is.Not.Null);
            Assert.That(singleDependency, Is.Not.Null);
            Assert.That(target.nested, Is.Not.Null);
            Assert.That(target.nested.NestedMethodAssetDependency, Is.EqualTo(assetDependency));
            CollectionAssert.AreEquivalent(dependencies, target.nested.NestedMethodComponentDependencies);
            Assert.That(target.nested.NestedMethodInterfaceDependency, Is.EqualTo(singleDependency));
        }
    }
}
