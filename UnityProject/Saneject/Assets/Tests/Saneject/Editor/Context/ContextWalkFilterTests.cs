using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Context
{
    public class ContextWalkFilterTests
    {
        [Test]
        public void SceneObjects_GivenMixedSceneAndPrefabInstanceHierarchy_InjectsSceneTargetsOnly()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestScope sceneScope = scene.Add<TestScope>("Root 1");
            ComponentDependency sceneDependency = scene.Add<ComponentDependency>("Root 1");
            SingleConcreteComponentTarget sceneRootTarget = scene.Add<SingleConcreteComponentTarget>("Root 1");
            SingleConcreteComponentTarget sceneChildOneTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
            SingleConcreteComponentTarget sceneChildTwoTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1/Child 1"), "Prefab Instance 1");
            prefabInstanceOne.AddTransform("Prefab Root/Scene Child");
            TestScope prefabScopeOne = prefabInstanceOne.Add<TestScope>("Prefab Root");
            ComponentDependency prefabDependencyOne = prefabInstanceOne.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget prefabTargetOne = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            SingleConcreteComponentTarget sceneChildInsidePrefabTarget = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Scene Child");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1/Child 2"), "Prefab Instance 2");
            TestScope prefabScopeTwo = prefabInstanceTwo.Add<TestScope>("Prefab Root");
            ComponentDependency prefabDependencyTwo = prefabInstanceTwo.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget prefabTargetTwo = prefabInstanceTwo.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");

            try
            {
                // Bind
                sceneScope.BindComponent<ComponentDependency>().FromSelf();
                prefabScopeOne.BindComponent<ComponentDependency>().FromSelf();
                prefabScopeTwo.BindComponent<ComponentDependency>().FromSelf();

                // Inject
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

                // Assert
                Assert.That(sceneDependency, Is.Not.Null);
                Assert.That(prefabDependencyOne, Is.Not.Null);
                Assert.That(prefabDependencyTwo, Is.Not.Null);
                Assert.That(sceneRootTarget.dependency, Is.Not.Null);
                Assert.That(sceneChildOneTarget.dependency, Is.Not.Null);
                Assert.That(sceneChildTwoTarget.dependency, Is.Not.Null);
                Assert.That(sceneChildInsidePrefabTarget.dependency, Is.Not.Null);
                Assert.That(prefabTargetOne.dependency, Is.Null);
                Assert.That(prefabTargetTwo.dependency, Is.Null);
            }
            finally
            {
                prefabInstanceOne.Destroy();
                prefabInstanceTwo.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabInstances_GivenMixedSceneAndPrefabInstanceHierarchy_InjectsPrefabInstanceTargetsOnly()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestScope sceneScope = scene.Add<TestScope>("Root 1");
            ComponentDependency sceneDependency = scene.Add<ComponentDependency>("Root 1");
            SingleConcreteComponentTarget sceneRootTarget = scene.Add<SingleConcreteComponentTarget>("Root 1");
            SingleConcreteComponentTarget sceneChildOneTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
            SingleConcreteComponentTarget sceneChildTwoTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1/Child 1"), "Prefab Instance 1");
            prefabInstanceOne.AddTransform("Prefab Root/Scene Child");
            TestScope prefabScopeOne = prefabInstanceOne.Add<TestScope>("Prefab Root");
            ComponentDependency prefabDependencyOne = prefabInstanceOne.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget prefabTargetOne = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            SingleConcreteComponentTarget sceneChildInsidePrefabTarget = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Scene Child");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1/Child 2"), "Prefab Instance 2");
            TestScope prefabScopeTwo = prefabInstanceTwo.Add<TestScope>("Prefab Root");
            ComponentDependency prefabDependencyTwo = prefabInstanceTwo.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget prefabTargetTwo = prefabInstanceTwo.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");

            try
            {
                // Bind
                sceneScope.BindComponent<ComponentDependency>().FromSelf();
                prefabScopeOne.BindComponent<ComponentDependency>().FromSelf();
                prefabScopeTwo.BindComponent<ComponentDependency>().FromSelf();

                // Inject
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.PrefabInstances);

                // Assert
                Assert.That(sceneDependency, Is.Not.Null);
                Assert.That(prefabDependencyOne, Is.Not.Null);
                Assert.That(prefabDependencyTwo, Is.Not.Null);
                Assert.That(sceneRootTarget.dependency, Is.Null);
                Assert.That(sceneChildOneTarget.dependency, Is.Null);
                Assert.That(sceneChildTwoTarget.dependency, Is.Null);
                Assert.That(sceneChildInsidePrefabTarget.dependency, Is.Null);
                Assert.That(prefabTargetOne.dependency, Is.Not.Null);
                Assert.That(prefabTargetTwo.dependency, Is.Not.Null);
            }
            finally
            {
                prefabInstanceOne.Destroy();
                prefabInstanceTwo.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAssetObjects_GivenMixedSceneAndPrefabInstanceHierarchy_InjectsNothing()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestScope sceneScope = scene.Add<TestScope>("Root 1");
            ComponentDependency sceneDependency = scene.Add<ComponentDependency>("Root 1");
            SingleConcreteComponentTarget sceneRootTarget = scene.Add<SingleConcreteComponentTarget>("Root 1");
            SingleConcreteComponentTarget sceneChildOneTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
            SingleConcreteComponentTarget sceneChildTwoTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1/Child 1"), "Prefab Instance 1");
            prefabInstanceOne.AddTransform("Prefab Root/Scene Child");
            TestScope prefabScopeOne = prefabInstanceOne.Add<TestScope>("Prefab Root");
            ComponentDependency prefabDependencyOne = prefabInstanceOne.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget prefabTargetOne = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            SingleConcreteComponentTarget sceneChildInsidePrefabTarget = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Scene Child");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1/Child 2"), "Prefab Instance 2");
            TestScope prefabScopeTwo = prefabInstanceTwo.Add<TestScope>("Prefab Root");
            ComponentDependency prefabDependencyTwo = prefabInstanceTwo.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget prefabTargetTwo = prefabInstanceTwo.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");

            try
            {
                // Bind
                sceneScope.BindComponent<ComponentDependency>().FromSelf();
                prefabScopeOne.BindComponent<ComponentDependency>().FromSelf();
                prefabScopeTwo.BindComponent<ComponentDependency>().FromSelf();

                // Inject
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.PrefabAssetObjects);

                // Assert
                Assert.That(sceneDependency, Is.Not.Null);
                Assert.That(prefabDependencyOne, Is.Not.Null);
                Assert.That(prefabDependencyTwo, Is.Not.Null);
                Assert.That(sceneRootTarget.dependency, Is.Null);
                Assert.That(sceneChildOneTarget.dependency, Is.Null);
                Assert.That(sceneChildTwoTarget.dependency, Is.Null);
                Assert.That(sceneChildInsidePrefabTarget.dependency, Is.Null);
                Assert.That(prefabTargetOne.dependency, Is.Null);
                Assert.That(prefabTargetTwo.dependency, Is.Null);
            }
            finally
            {
                prefabInstanceOne.Destroy();
                prefabInstanceTwo.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void AllContexts_GivenMixedSceneAndPrefabInstanceHierarchy_InjectsSceneAndPrefabInstanceTargets()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestScope sceneScope = scene.Add<TestScope>("Root 1");
            ComponentDependency sceneDependency = scene.Add<ComponentDependency>("Root 1");
            SingleConcreteComponentTarget sceneRootTarget = scene.Add<SingleConcreteComponentTarget>("Root 1");
            SingleConcreteComponentTarget sceneChildOneTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
            SingleConcreteComponentTarget sceneChildTwoTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1/Child 1"), "Prefab Instance 1");
            prefabInstanceOne.AddTransform("Prefab Root/Scene Child");
            TestScope prefabScopeOne = prefabInstanceOne.Add<TestScope>("Prefab Root");
            ComponentDependency prefabDependencyOne = prefabInstanceOne.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget prefabTargetOne = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            SingleConcreteComponentTarget sceneChildInsidePrefabTarget = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Scene Child");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1/Child 2"), "Prefab Instance 2");
            TestScope prefabScopeTwo = prefabInstanceTwo.Add<TestScope>("Prefab Root");
            ComponentDependency prefabDependencyTwo = prefabInstanceTwo.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget prefabTargetTwo = prefabInstanceTwo.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");

            try
            {
                // Bind
                sceneScope.BindComponent<ComponentDependency>().FromSelf();
                prefabScopeOne.BindComponent<ComponentDependency>().FromSelf();
                prefabScopeTwo.BindComponent<ComponentDependency>().FromSelf();

                // Inject
                InjectionRunner.Run(scene.Roots, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(sceneDependency, Is.Not.Null);
                Assert.That(prefabDependencyOne, Is.Not.Null);
                Assert.That(prefabDependencyTwo, Is.Not.Null);
                Assert.That(sceneRootTarget.dependency, Is.Not.Null);
                Assert.That(sceneChildOneTarget.dependency, Is.Not.Null);
                Assert.That(sceneChildTwoTarget.dependency, Is.Not.Null);
                Assert.That(sceneChildInsidePrefabTarget.dependency, Is.Not.Null);
                Assert.That(prefabTargetOne.dependency, Is.Not.Null);
                Assert.That(prefabTargetTwo.dependency, Is.Not.Null);
            }
            finally
            {
                prefabInstanceOne.Destroy();
                prefabInstanceTwo.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void SameContextsAsSelection_GivenMixedSceneAndPrefabInstanceHierarchy_InjectsTargetsInSelectedContexts()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestScope sceneScope = scene.Add<TestScope>("Root 1");
            ComponentDependency sceneDependency = scene.Add<ComponentDependency>("Root 1");
            SingleConcreteComponentTarget sceneRootTarget = scene.Add<SingleConcreteComponentTarget>("Root 1");
            SingleConcreteComponentTarget sceneChildOneTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
            SingleConcreteComponentTarget sceneChildTwoTarget = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1/Child 1"), "Prefab Instance 1");
            prefabInstanceOne.AddTransform("Prefab Root/Scene Child");
            TestScope prefabScopeOne = prefabInstanceOne.Add<TestScope>("Prefab Root");
            ComponentDependency prefabDependencyOne = prefabInstanceOne.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget prefabTargetOne = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            SingleConcreteComponentTarget sceneChildInsidePrefabTarget = prefabInstanceOne.Add<SingleConcreteComponentTarget>("Prefab Root/Scene Child");
            TestPrefabInstance prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1/Child 2"), "Prefab Instance 2");
            TestScope prefabScopeTwo = prefabInstanceTwo.Add<TestScope>("Prefab Root");
            ComponentDependency prefabDependencyTwo = prefabInstanceTwo.Add<ComponentDependency>("Prefab Root");
            SingleConcreteComponentTarget prefabTargetTwo = prefabInstanceTwo.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");

            Transform[] startTransforms =
            {
                scene.GetTransform("Root 1/Child 1"),
                prefabInstanceOne.GetTransform("Prefab Root/Child 1")
            };

            try
            {
                // Bind
                sceneScope.BindComponent<ComponentDependency>().FromSelf();
                prefabScopeOne.BindComponent<ComponentDependency>().FromSelf();
                prefabScopeTwo.BindComponent<ComponentDependency>().FromSelf();

                // Inject
                InjectionRunner.Run(startTransforms, ContextWalkFilter.SameContextsAsSelection);

                // Assert
                Assert.That(sceneDependency, Is.Not.Null);
                Assert.That(prefabDependencyOne, Is.Not.Null);
                Assert.That(prefabDependencyTwo, Is.Not.Null);
                Assert.That(sceneRootTarget.dependency, Is.Not.Null);
                Assert.That(sceneChildOneTarget.dependency, Is.Not.Null);
                Assert.That(sceneChildTwoTarget.dependency, Is.Not.Null);
                Assert.That(sceneChildInsidePrefabTarget.dependency, Is.Not.Null);
                Assert.That(prefabTargetOne.dependency, Is.Not.Null);
                Assert.That(prefabTargetTwo.dependency, Is.Null);
            }
            finally
            {
                prefabInstanceOne.Destroy();
                prefabInstanceTwo.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void SceneObjects_GivenMixedPrefabAssetAndPrefabInstanceHierarchy_InjectsNothing()
        {
            // Set up prefab asset
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Host Root", width: 2, depth: 2);
            hostPrefab.Add<TestScope>("Host Root");
            hostPrefab.Add<ComponentDependency>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 1");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 2");

            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 2);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestPrefabInstance nestedInstanceOne = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 1"), "Nested Prefab 1");
            TestScope nestedScopeOne = nestedInstanceOne.Add<TestScope>("Nested Root");
            ComponentDependency nestedDependencyOne = nestedInstanceOne.Add<ComponentDependency>("Nested Root");
            SingleConcreteComponentTarget nestedTargetOne = nestedInstanceOne.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            TestPrefabInstance nestedInstanceTwo = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 2"), "Nested Prefab 2");
            TestScope nestedScopeTwo = nestedInstanceTwo.Add<TestScope>("Nested Root");
            ComponentDependency nestedDependencyTwo = nestedInstanceTwo.Add<ComponentDependency>("Nested Root");
            SingleConcreteComponentTarget nestedTargetTwo = nestedInstanceTwo.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            TestScope hostScope = hostStage.Get<TestScope>("Host Root");
            ComponentDependency hostDependency = hostStage.Get<ComponentDependency>("Host Root");
            SingleConcreteComponentTarget hostRootTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root");
            SingleConcreteComponentTarget hostChildOneTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 1");
            SingleConcreteComponentTarget hostChildTwoTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 2");

            try
            {
                // Bind
                hostScope.BindComponent<ComponentDependency>().FromSelf();
                nestedScopeOne.BindComponent<ComponentDependency>().FromSelf();
                nestedScopeTwo.BindComponent<ComponentDependency>().FromSelf();

                // Inject
                InjectionRunner.Run(new[] { hostStage.Root.transform }, ContextWalkFilter.SceneObjects);

                // Assert
                Assert.That(hostDependency, Is.Not.Null);
                Assert.That(nestedDependencyOne, Is.Not.Null);
                Assert.That(nestedDependencyTwo, Is.Not.Null);
                Assert.That(hostRootTarget.dependency, Is.Null);
                Assert.That(hostChildOneTarget.dependency, Is.Null);
                Assert.That(hostChildTwoTarget.dependency, Is.Null);
                Assert.That(nestedTargetOne.dependency, Is.Null);
                Assert.That(nestedTargetTwo.dependency, Is.Null);
            }
            finally
            {
                nestedInstanceOne.Destroy();
                nestedInstanceTwo.Destroy();

                TestPrefabAsset.CloseStage();

                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();

                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabInstances_GivenMixedPrefabAssetAndPrefabInstanceHierarchy_InjectsPrefabInstanceTargetsOnly()
        {
            // Set up prefab asset
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Host Root", width: 2, depth: 2);
            hostPrefab.Add<TestScope>("Host Root");
            hostPrefab.Add<ComponentDependency>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 1");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 2");

            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 2);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestPrefabInstance nestedInstanceOne = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 1"), "Nested Prefab 1");
            TestScope nestedScopeOne = nestedInstanceOne.Add<TestScope>("Nested Root");
            ComponentDependency nestedDependencyOne = nestedInstanceOne.Add<ComponentDependency>("Nested Root");
            SingleConcreteComponentTarget nestedTargetOne = nestedInstanceOne.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            TestPrefabInstance nestedInstanceTwo = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 2"), "Nested Prefab 2");
            TestScope nestedScopeTwo = nestedInstanceTwo.Add<TestScope>("Nested Root");
            ComponentDependency nestedDependencyTwo = nestedInstanceTwo.Add<ComponentDependency>("Nested Root");
            SingleConcreteComponentTarget nestedTargetTwo = nestedInstanceTwo.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            TestScope hostScope = hostStage.Get<TestScope>("Host Root");
            ComponentDependency hostDependency = hostStage.Get<ComponentDependency>("Host Root");
            SingleConcreteComponentTarget hostRootTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root");
            SingleConcreteComponentTarget hostChildOneTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 1");
            SingleConcreteComponentTarget hostChildTwoTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 2");

            try
            {
                // Bind
                hostScope.BindComponent<ComponentDependency>().FromSelf();
                nestedScopeOne.BindComponent<ComponentDependency>().FromSelf();
                nestedScopeTwo.BindComponent<ComponentDependency>().FromSelf();

                // Inject
                InjectionRunner.Run(new[] { hostStage.Root.transform }, ContextWalkFilter.PrefabInstances);

                // Assert
                Assert.That(hostDependency, Is.Not.Null);
                Assert.That(nestedDependencyOne, Is.Not.Null);
                Assert.That(nestedDependencyTwo, Is.Not.Null);
                Assert.That(hostRootTarget.dependency, Is.Null);
                Assert.That(hostChildOneTarget.dependency, Is.Null);
                Assert.That(hostChildTwoTarget.dependency, Is.Null);
                Assert.That(nestedTargetOne.dependency, Is.Not.Null);
                Assert.That(nestedTargetTwo.dependency, Is.Not.Null);
            }
            finally
            {
                nestedInstanceOne.Destroy();
                nestedInstanceTwo.Destroy();

                TestPrefabAsset.CloseStage();

                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();

                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }

        [Test]
        public void PrefabAssetObjects_GivenMixedPrefabAssetAndPrefabInstanceHierarchy_InjectsPrefabAssetTargetsOnly()
        {
            // Set up prefab asset
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Host Root", width: 2, depth: 2);
            hostPrefab.Add<TestScope>("Host Root");
            hostPrefab.Add<ComponentDependency>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 1");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 2");

            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 2);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestPrefabInstance nestedInstanceOne = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 1"), "Nested Prefab 1");
            TestScope nestedScopeOne = nestedInstanceOne.Add<TestScope>("Nested Root");
            ComponentDependency nestedDependencyOne = nestedInstanceOne.Add<ComponentDependency>("Nested Root");
            SingleConcreteComponentTarget nestedTargetOne = nestedInstanceOne.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            TestPrefabInstance nestedInstanceTwo = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 2"), "Nested Prefab 2");
            TestScope nestedScopeTwo = nestedInstanceTwo.Add<TestScope>("Nested Root");
            ComponentDependency nestedDependencyTwo = nestedInstanceTwo.Add<ComponentDependency>("Nested Root");
            SingleConcreteComponentTarget nestedTargetTwo = nestedInstanceTwo.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            TestScope hostScope = hostStage.Get<TestScope>("Host Root");
            ComponentDependency hostDependency = hostStage.Get<ComponentDependency>("Host Root");
            SingleConcreteComponentTarget hostRootTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root");
            SingleConcreteComponentTarget hostChildOneTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 1");
            SingleConcreteComponentTarget hostChildTwoTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 2");

            try
            {
                // Bind
                hostScope.BindComponent<ComponentDependency>().FromSelf();
                nestedScopeOne.BindComponent<ComponentDependency>().FromSelf();
                nestedScopeTwo.BindComponent<ComponentDependency>().FromSelf();

                // Inject
                InjectionRunner.Run(new[] { hostStage.Root.transform }, ContextWalkFilter.PrefabAssetObjects);

                // Assert
                Assert.That(hostDependency, Is.Not.Null);
                Assert.That(nestedDependencyOne, Is.Not.Null);
                Assert.That(nestedDependencyTwo, Is.Not.Null);
                Assert.That(hostRootTarget.dependency, Is.Not.Null);
                Assert.That(hostChildOneTarget.dependency, Is.Not.Null);
                Assert.That(hostChildTwoTarget.dependency, Is.Not.Null);
                Assert.That(nestedTargetOne.dependency, Is.Null);
                Assert.That(nestedTargetTwo.dependency, Is.Null);
            }
            finally
            {
                nestedInstanceOne.Destroy();
                nestedInstanceTwo.Destroy();

                TestPrefabAsset.CloseStage();

                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();

                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }

        [Test]
        public void AllContexts_GivenMixedPrefabAssetAndPrefabInstanceHierarchy_InjectsPrefabAssetAndPrefabInstanceTargets()
        {
            // Set up prefab asset
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Host Root", width: 2, depth: 2);
            hostPrefab.Add<TestScope>("Host Root");
            hostPrefab.Add<ComponentDependency>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 1");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 2");

            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 2);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestPrefabInstance nestedInstanceOne = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 1"), "Nested Prefab 1");
            TestScope nestedScopeOne = nestedInstanceOne.Add<TestScope>("Nested Root");
            ComponentDependency nestedDependencyOne = nestedInstanceOne.Add<ComponentDependency>("Nested Root");
            SingleConcreteComponentTarget nestedTargetOne = nestedInstanceOne.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            TestPrefabInstance nestedInstanceTwo = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 2"), "Nested Prefab 2");
            TestScope nestedScopeTwo = nestedInstanceTwo.Add<TestScope>("Nested Root");
            ComponentDependency nestedDependencyTwo = nestedInstanceTwo.Add<ComponentDependency>("Nested Root");
            SingleConcreteComponentTarget nestedTargetTwo = nestedInstanceTwo.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            TestScope hostScope = hostStage.Get<TestScope>("Host Root");
            ComponentDependency hostDependency = hostStage.Get<ComponentDependency>("Host Root");
            SingleConcreteComponentTarget hostRootTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root");
            SingleConcreteComponentTarget hostChildOneTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 1");
            SingleConcreteComponentTarget hostChildTwoTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 2");

            try
            {
                // Bind
                hostScope.BindComponent<ComponentDependency>().FromSelf();
                nestedScopeOne.BindComponent<ComponentDependency>().FromSelf();
                nestedScopeTwo.BindComponent<ComponentDependency>().FromSelf();

                // Inject
                InjectionRunner.Run(new[] { hostStage.Root.transform }, ContextWalkFilter.AllContexts);

                // Assert
                Assert.That(hostDependency, Is.Not.Null);
                Assert.That(nestedDependencyOne, Is.Not.Null);
                Assert.That(nestedDependencyTwo, Is.Not.Null);
                Assert.That(hostRootTarget.dependency, Is.Not.Null);
                Assert.That(hostChildOneTarget.dependency, Is.Not.Null);
                Assert.That(hostChildTwoTarget.dependency, Is.Not.Null);
                Assert.That(nestedTargetOne.dependency, Is.Not.Null);
                Assert.That(nestedTargetTwo.dependency, Is.Not.Null);
            }
            finally
            {
                nestedInstanceOne.Destroy();
                nestedInstanceTwo.Destroy();

                TestPrefabAsset.CloseStage();

                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();

                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }

        [Test]
        public void SameContextsAsSelection_GivenMixedPrefabAssetAndPrefabInstanceHierarchy_InjectsTargetsInSelectedContexts()
        {
            // Set up prefab asset
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Host Root", width: 2, depth: 2);
            hostPrefab.Add<TestScope>("Host Root");
            hostPrefab.Add<ComponentDependency>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 1");
            hostPrefab.Add<SingleConcreteComponentTarget>("Host Root/Child 2");

            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 2);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestPrefabInstance nestedInstanceOne = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 1"), "Nested Prefab 1");
            TestScope nestedScopeOne = nestedInstanceOne.Add<TestScope>("Nested Root");
            ComponentDependency nestedDependencyOne = nestedInstanceOne.Add<ComponentDependency>("Nested Root");
            SingleConcreteComponentTarget nestedTargetOne = nestedInstanceOne.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            TestPrefabInstance nestedInstanceTwo = nestedPrefab.Instantiate(hostStage.GetTransform("Host Root/Child 2"), "Nested Prefab 2");
            TestScope nestedScopeTwo = nestedInstanceTwo.Add<TestScope>("Nested Root");
            ComponentDependency nestedDependencyTwo = nestedInstanceTwo.Add<ComponentDependency>("Nested Root");
            SingleConcreteComponentTarget nestedTargetTwo = nestedInstanceTwo.Add<SingleConcreteComponentTarget>("Nested Root/Child 1");
            TestScope hostScope = hostStage.Get<TestScope>("Host Root");
            ComponentDependency hostDependency = hostStage.Get<ComponentDependency>("Host Root");
            SingleConcreteComponentTarget hostRootTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root");
            SingleConcreteComponentTarget hostChildOneTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 1");
            SingleConcreteComponentTarget hostChildTwoTarget = hostStage.Get<SingleConcreteComponentTarget>("Host Root/Child 2");

            Transform[] startTransforms =
            {
                hostStage.GetTransform("Host Root/Child 1"),
                nestedInstanceOne.GetTransform("Nested Root/Child 1")
            };

            try
            {
                // Bind
                hostScope.BindComponent<ComponentDependency>().FromSelf();
                nestedScopeOne.BindComponent<ComponentDependency>().FromSelf();
                nestedScopeTwo.BindComponent<ComponentDependency>().FromSelf();

                // Inject
                InjectionRunner.Run(startTransforms, ContextWalkFilter.SameContextsAsSelection);

                // Assert
                Assert.That(hostDependency, Is.Not.Null);
                Assert.That(nestedDependencyOne, Is.Not.Null);
                Assert.That(nestedDependencyTwo, Is.Not.Null);
                Assert.That(hostRootTarget.dependency, Is.Not.Null);
                Assert.That(hostChildOneTarget.dependency, Is.Not.Null);
                Assert.That(hostChildTwoTarget.dependency, Is.Not.Null);
                Assert.That(nestedTargetOne.dependency, Is.Not.Null);
                Assert.That(nestedTargetTwo.dependency, Is.Null);
            }
            finally
            {
                nestedInstanceOne.Destroy();
                nestedInstanceTwo.Destroy();

                TestPrefabAsset.CloseStage();

                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();

                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }
    }
}