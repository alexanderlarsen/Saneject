using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.Locators.AssetLocators
{
    public class SpecialTests
    {
        [Test]
        public void FromInstance_InjectsAsset()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteAssetTarget target = scene.Add<ConcreteAssetTarget>("Root 1");
            AssetDependency expected = Resources.Load<AssetDependency>("AssetDependency 1");

            scope.BindAsset<AssetDependency>().FromInstance(expected);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.NotNull(expected);
            Assert.NotNull(target.dependency);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromMethod_InjectsAsset()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteAssetTarget target = scene.Add<ConcreteAssetTarget>("Root 1");
            AssetDependency expected = Resources.Load<AssetDependency>("AssetDependency 1");

            scope.BindAsset<AssetDependency>().FromMethod(() => expected);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.NotNull(expected);
            Assert.NotNull(target.dependency);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromMethodEnumerable_InjectsAsset()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteAssetTarget target = scene.Add<ConcreteAssetTarget>("Root 1");
            AssetDependency expected = Resources.Load<AssetDependency>("AssetDependency 1");

            scope.BindAsset<AssetDependency>().FromMethod(() => new[]
            {
                expected
            });

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.NotNull(expected);
            Assert.NotNull(target.dependency);
            Assert.AreEqual(expected, target.dependency);
        }
    }
}