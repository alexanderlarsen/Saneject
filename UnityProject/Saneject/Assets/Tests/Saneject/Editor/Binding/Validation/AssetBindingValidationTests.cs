using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Binding.Validation
{
    public class AssetBindingValidationTests
    {
        [Test]
        public void BindAsset_TConcreteTConcrete_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindAsset<AssetDependency, AssetDependency>().FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindAsset_TConcrete_WithoutLocatorStrategy_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindAsset<AssetDependency>();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindAsset_TComponent_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            scope.BindAsset<ComponentDependency>().FromInstance(dependency);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindMultipleAssets_TConcreteTConcrete_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 5);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindAssets<AssetDependency, AssetDependency>().FromFolder("Assets/Tests/Saneject/Fixtures/Resources");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindAsset_TConcrete_DuplicateWithinSameScope_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindAsset<AssetDependency>().FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");
            scope.BindAsset<AssetDependency>().FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

    }
}
