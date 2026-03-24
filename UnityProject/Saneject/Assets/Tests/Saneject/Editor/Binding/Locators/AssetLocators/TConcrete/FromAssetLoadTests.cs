using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.Locators.AssetLocators.TConcrete
{
    public class FromAssetLoadTests
    {
        [Test]
        public void FromAssetLoad_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteAssetTarget target = scene.Add<SingleConcreteAssetTarget>("Root 1");
            
            // Find dependency 
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");
            
            // Bind
            scope.BindAsset<AssetDependency>().FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");
            
            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            
            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}