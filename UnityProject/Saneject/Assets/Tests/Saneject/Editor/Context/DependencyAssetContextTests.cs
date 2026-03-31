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
        public void PrefabAssetRoot_LoadedFromAssetDatabase_SetsGlobalContextIdentity()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);

            try
            {
                // Capture context identity
                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
                ContextIdentity identity = new(prefabAsset);

                // Assert
                Assert.That(prefabAsset, Is.Not.Null);
                Assert.That(identity.Type, Is.EqualTo(ContextType.Global));
                Assert.That(identity.Id, Is.EqualTo(0));
                Assert.That(identity.ContainerType, Is.EqualTo(ContextType.Global));
                Assert.That(identity.ContainerId, Is.EqualTo(0));
                Assert.That(identity.IsPrefab, Is.False);
            }
            finally
            {
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void Scene_PrefabAsset_Injects_WHEN_ContextIsolationIsTrue_And_UsedAsDependencyAsset()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

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
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

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
    }
}
