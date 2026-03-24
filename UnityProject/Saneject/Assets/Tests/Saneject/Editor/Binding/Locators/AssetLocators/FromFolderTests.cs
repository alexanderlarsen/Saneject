using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.Locators.AssetLocators
{
    public class FromFolderTests
    {
        [Test]
        public void FromFolder_InjectsAsset()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteAssetTarget target = scene.Add<SingleConcreteAssetTarget>("Root 1");
            AssetDependency dependency = Resources.Load<AssetDependency>("AssetDependency 1");

            scope.BindAsset<AssetDependency>().FromFolder("Assets/Tests/Saneject/Fixtures/Resources");

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}