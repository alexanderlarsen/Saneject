using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Plugins.Saneject.Runtime.Settings;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Context
{
    public class ContextIsolationTests
    {
        [Test]
        public void Scene_SameScene_Injects_WHEN_ContextIsolationIsTrue()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.EqualTo(dependency));
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;
            }
        }

        [Test]
        public void Scene_PrefabInstance_DoesNotInject_WHEN_ContextIsolationIsTrue()
        {
            // Expect error logs for missing dependencies
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));
            
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            ComponentDependency dependency = prefabInstance.Add<ComponentDependency>("Prefab Root");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.Null);
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
        public void Scene_PrefabAsset_DoesNotInject_WHEN_ContextIsolationIsTrue()
        {
            // Expect error logs for missing dependencies
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency")); 
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));
            
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            prefab.Add<ComponentDependency>("Prefab Root");
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
            ComponentDependency dependency = prefabAsset.GetComponent<ComponentDependency>();
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.Null);
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabInstance_SamePrefabInstance_Injects_WHEN_ContextIsolationIsTrue()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            TestScope scope = prefabInstance.Add<TestScope>("Prefab Root");
            ComponentDependency dependency = prefabInstance.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget target = prefabInstance.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.EqualTo(dependency));
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
        public void PrefabInstance_OtherPrefabInstance_DoesNotInject_WHEN_ContextIsolationIsTrue()
        {
            // Expect error logs for missing dependencies
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));
            
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance 1");
            TestScope scope = prefabInstanceOne.Add<TestScope>("Prefab Root");
            SingleConcreteComponentTarget target = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance 2");
            ComponentDependency dependency = prefabInstanceTwo.Add<ComponentDependency>("Prefab Root");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.Null);
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                prefabInstanceOne?.Destroy();
                prefabInstanceTwo?.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabInstance_Scene_DoesNotInject_WHEN_ContextIsolationIsTrue()
        {
            // Expect error logs for missing dependencies
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));
            
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            TestScope scope = prefabInstance.Add<TestScope>("Prefab Root");
            SingleConcreteComponentTarget target = prefabInstance.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.Null);
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
        public void PrefabInstance_PrefabAsset_DoesNotInject_WHEN_ContextIsolationIsTrue()
        {
            // Expect error logs for missing dependencies
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));
            
            // Set up prefab asset
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Host Root", width: 1, depth: 2);
            hostPrefab.Add<ComponentDependency>("Host Root");
            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 2);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestPrefabInstance nestedInstance = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root"), "Nested Prefab");
            TestScope scope = nestedInstance.Add<TestScope>("Nested Root");
            SingleConcreteComponentTarget target = nestedInstance.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            ComponentDependency dependency = hostStage.Get<ComponentDependency>("Host Root");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(new[] { hostStage.Root.transform }, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.Null);
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                nestedInstance?.Destroy();

                TestPrefabAsset.CloseStage();

                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();

                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAsset_SamePrefabAsset_Injects_WHEN_ContextIsolationIsTrue()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            prefab.Add<TestScope>("Prefab Root");
            prefab.Add<ComponentDependency>("Prefab Root");
            prefab.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
            TestScope scope = prefabAsset.GetComponent<TestScope>();
            ComponentDependency dependency = prefabAsset.GetComponent<ComponentDependency>();
            SingleConcreteComponentTarget target = prefabAsset.transform.Find("Child 1").GetComponent<SingleConcreteComponentTarget>();
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(new[] { prefabAsset.transform }, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target, Is.Not.Null);
                Assert.That(target.dependency, Is.EqualTo(dependency));
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAsset_Scene_DoesNotInject_WHEN_ContextIsolationIsTrue()
        {
            // Expect error logs for missing dependencies
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));
            
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            prefab.Add<TestScope>("Prefab Root");
            prefab.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
            TestScope scope = prefabAsset.GetComponent<TestScope>();
            SingleConcreteComponentTarget target = prefabAsset.transform.Find("Child 1").GetComponent<SingleConcreteComponentTarget>();
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(new[] { prefabAsset.transform }, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target, Is.Not.Null);
                Assert.That(target.dependency, Is.Null);
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAsset_PrefabInstance_DoesNotInject_WHEN_ContextIsolationIsTrue()
        {
            // Expect error logs for missing dependencies
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));
            
            // Set up prefab asset
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Host Root", width: 1, depth: 2);
            hostPrefab.Add<TestScope>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 1");
            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 2);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestPrefabInstance nestedInstance = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root"), "Nested Prefab");
            TestScope scope = hostStage.Get<TestScope>("Host Root");
            SingleConcreteComponentTarget target = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 1");
            ComponentDependency dependency = nestedInstance.Add<ComponentDependency>("Nested Root");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = true;
                InjectionRunner.Run(new[] { hostStage.Root.transform }, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.Null);
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                nestedInstance?.Destroy();

                TestPrefabAsset.CloseStage();

                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();

                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }

        [Test]
        public void Scene_SameScene_Injects_WHEN_ContextIsolationIsFalse()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.EqualTo(dependency));
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;
            }
        }

        [Test]
        public void Scene_PrefabInstance_Injects_WHEN_ContextIsolationIsFalse()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            ComponentDependency dependency = prefabInstance.Add<ComponentDependency>("Prefab Root");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.EqualTo(dependency));
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
        public void Scene_PrefabAsset_DoesNotInject_WHEN_ContextIsolationIsFalse()
        {
            // Expect error logs for missing dependencies
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));
            
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            prefab.Add<ComponentDependency>("Prefab Root");
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
            ComponentDependency dependency = prefabAsset.GetComponent<ComponentDependency>();
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.Null);
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabInstance_SamePrefabInstance_Injects_WHEN_ContextIsolationIsFalse()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            TestScope scope = prefabInstance.Add<TestScope>("Prefab Root");
            ComponentDependency dependency = prefabInstance.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget target = prefabInstance.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.EqualTo(dependency));
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
        public void PrefabInstance_OtherPrefabInstance_Injects_WHEN_ContextIsolationIsFalse()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance 1");
            TestScope scope = prefabInstanceOne.Add<TestScope>("Prefab Root");
            SingleConcreteComponentTarget target = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance 2");
            ComponentDependency dependency = prefabInstanceTwo.Add<ComponentDependency>("Prefab Root");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.EqualTo(dependency));
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                prefabInstanceOne?.Destroy();
                prefabInstanceTwo?.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabInstance_Scene_Injects_WHEN_ContextIsolationIsFalse()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            TestScope scope = prefabInstance.Add<TestScope>("Prefab Root");
            SingleConcreteComponentTarget target = prefabInstance.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.EqualTo(dependency));
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
        public void PrefabInstance_PrefabAsset_Injects_WHEN_ContextIsolationIsFalse()
        {
            // Set up prefab asset
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Host Root", width: 1, depth: 2);
            hostPrefab.Add<ComponentDependency>("Host Root");
            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 2);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestPrefabInstance nestedInstance = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root"), "Nested Prefab");
            TestScope scope = nestedInstance.Add<TestScope>("Nested Root");
            SingleConcreteComponentTarget target = nestedInstance.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            ComponentDependency dependency = hostStage.Get<ComponentDependency>("Host Root");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(new[] { hostStage.Root.transform }, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.EqualTo(dependency));
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                nestedInstance?.Destroy();

                TestPrefabAsset.CloseStage();

                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();

                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAsset_SamePrefabAsset_Injects_WHEN_ContextIsolationIsFalse()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            prefab.Add<TestScope>("Prefab Root");
            prefab.Add<ComponentDependency>("Prefab Root");
            prefab.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
            TestScope scope = prefabAsset.GetComponent<TestScope>();
            ComponentDependency dependency = prefabAsset.GetComponent<ComponentDependency>();
            SingleConcreteComponentTarget target = prefabAsset.transform.Find("Child 1").GetComponent<SingleConcreteComponentTarget>();
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(new[] { prefabAsset.transform }, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target, Is.Not.Null);
                Assert.That(target.dependency, Is.EqualTo(dependency));
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAsset_Scene_DoesNotInject_WHEN_ContextIsolationIsFalse()
        {
            // Expect error logs for missing dependencies
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Could not locate dependency"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));
            
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            prefab.Add<TestScope>("Prefab Root");
            prefab.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
            TestScope scope = prefabAsset.GetComponent<TestScope>();
            SingleConcreteComponentTarget target = prefabAsset.transform.Find("Child 1").GetComponent<SingleConcreteComponentTarget>();
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(new[] { prefabAsset.transform }, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target, Is.Not.Null);
                Assert.That(target.dependency, Is.Null);
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAsset_PrefabInstance_Injects_WHEN_ContextIsolationIsFalse()
        {
            // Set up prefab asset
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Host Root", width: 1, depth: 2);
            hostPrefab.Add<TestScope>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 1");
            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 2);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestPrefabInstance nestedInstance = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root"), "Nested Prefab");
            TestScope scope = hostStage.Get<TestScope>("Host Root");
            SingleConcreteComponentTarget target = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 1");
            ComponentDependency dependency = nestedInstance.Add<ComponentDependency>("Nested Root");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Bind
                scope.BindComponent<ComponentDependency>().FromInstance(dependency);

                // Inject
                ProjectSettings.UseContextIsolation = false;
                InjectionRunner.Run(new[] { hostStage.Root.transform }, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(target.dependency, Is.EqualTo(dependency));
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                nestedInstance?.Destroy();

                TestPrefabAsset.CloseStage();

                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();

                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }
    }
}
