using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Runtime.Settings;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Context
{
    public class DependencyAssetContextTests
    {
        [Test]
        public void Scene_PrefabAsset_Injects_WHEN_ContextIsolationIsTrue_And_UsedAsDependencyAsset()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            PrefabAssetDependencyTarget target = scene.Add<PrefabAssetDependencyTarget>("Root 1");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Find dependency
                GameObject dependency = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);

                // Bind
                scope.BindAsset<GameObject>().FromAssetLoad(prefab.AssetPath);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.Dependency, Is.EqualTo(dependency));
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void Scene_PrefabAsset_Injects_WHEN_ContextIsolationIsFalse_And_UsedAsDependencyAsset()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            PrefabAssetDependencyTarget target = scene.Add<PrefabAssetDependencyTarget>("Root 1");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Find dependency
                GameObject dependency = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);

                // Bind
                scope.BindAsset<GameObject>().FromAssetLoad(prefab.AssetPath);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.Dependency, Is.EqualTo(dependency));
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void Scene_PrefabInstance_DoesNotInject_WHEN_ContextIsolationIsTrue_And_UsedAsDependencyAsset()
        {
            // Expect error logs for missing dependencies
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            PrefabAssetDependencyTarget target = scene.Add<PrefabAssetDependencyTarget>("Root 1");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Find dependency
                GameObject dependency = prefabInstance.GetTransform("Prefab Root").gameObject;

                // Bind
                scope.BindAsset<GameObject>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.Dependency, Is.Null);
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;
                prefabInstance?.Destroy();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void Scene_PrefabInstance_DoesNotInject_WHEN_ContextIsolationIsFalse_And_UsedAsDependencyAsset()
        {
            // Expect error logs for missing dependencies
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            PrefabAssetDependencyTarget target = scene.Add<PrefabAssetDependencyTarget>("Root 1");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Find dependency
                GameObject dependency = prefabInstance.GetTransform("Prefab Root").gameObject;

                // Bind
                scope.BindAsset<GameObject>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.Dependency, Is.Null);
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;
                prefabInstance?.Destroy();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }
    }
}