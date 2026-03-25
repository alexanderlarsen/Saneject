using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Data.Graph;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Data.Injection;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Graph
{
    public class InjectionGraphContextTests
    {
        [Test]
        public void ContextIdentity_GivenSceneObjectPrefabInstancePrefabAssetAndAsset_CapturesExpectedContextMetadata()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefab prefab = CreateContextBoundaryPrefab();
            GameObject prefabInstanceRoot = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            Transform prefabInstanceChild = prefab.GetTransform(prefabInstanceRoot.transform, "Prefab Root/Scoped Child");
            AssetDependency assetDependency = Resources.Load<AssetDependency>("AssetDependency 1");

            try
            {
                // Capture context identities
                ContextIdentity sceneIdentity = new(scene.GetTransform("Root 1"));
                ContextIdentity prefabInstanceRootIdentity = new(prefabInstanceRoot.transform);
                ContextIdentity prefabInstanceChildIdentity = new(prefabInstanceChild);
                PrefabStage prefabStage = prefab.OpenStage();
                ContextIdentity prefabAssetIdentity = new(prefabStage.prefabContentsRoot.transform);
                ContextIdentity assetIdentity = new(assetDependency);

                // Assert
                Assert.That(assetDependency, Is.Not.Null);
                Assert.That(sceneIdentity.Type, Is.EqualTo(ContextType.SceneObject));
                Assert.That(sceneIdentity.IsPrefab, Is.False);
                Assert.That(prefabInstanceRootIdentity.Type, Is.EqualTo(ContextType.PrefabInstance));
                Assert.That(prefabInstanceRootIdentity.IsPrefab, Is.True);
                Assert.That(prefabInstanceChildIdentity.Type, Is.EqualTo(ContextType.PrefabInstance));
                Assert.That(prefabInstanceChildIdentity.IsPrefab, Is.True);
                Assert.That(prefabInstanceChildIdentity.Id, Is.EqualTo(prefabInstanceRootIdentity.Id));
                Assert.That(prefabInstanceChildIdentity, Is.EqualTo(prefabInstanceRootIdentity));
                Assert.That(prefabAssetIdentity.Type, Is.EqualTo(ContextType.PrefabAsset));
                Assert.That(prefabAssetIdentity.IsPrefab, Is.True);
                Assert.That(assetIdentity.Type, Is.EqualTo(ContextType.Global));
                Assert.That(assetIdentity.IsPrefab, Is.False);
                Assert.That(sceneIdentity, Is.Not.EqualTo(prefabInstanceRootIdentity));
                Assert.That(prefabAssetIdentity, Is.Not.EqualTo(prefabInstanceRootIdentity));
            }
            finally
            {
                TestPrefab.CloseStage();

                if (prefabInstanceRoot)
                    Object.DestroyImmediate(prefabInstanceRoot);

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void GraphFilter_GivenSceneHierarchyWithPrefabInstances_ReturnsExpectedTransformsForScenePrefabAndSelectionFilters()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestPrefab prefab = CreateContextBoundaryPrefab();
            GameObject prefabInstanceOne = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance 1");
            GameObject prefabInstanceTwo = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance 2");

            Transform[] startTransforms =
            {
                scene.GetTransform("Root 1/Child 1"),
                prefab.GetTransform(prefabInstanceOne.transform, "Prefab Root/Scoped Child")
            };

            try
            {
                // Build graph and apply filters
                InjectionGraph graph = new(startTransforms);
                IReadOnlyCollection<TransformNode> allNodes = ApplyWalkFilter(graph, startTransforms, ContextWalkFilter.AllContexts);
                IReadOnlyCollection<TransformNode> sceneNodes = ApplyWalkFilter(graph, startTransforms, ContextWalkFilter.SceneObjects);
                IReadOnlyCollection<TransformNode> prefabInstanceNodes = ApplyWalkFilter(graph, startTransforms, ContextWalkFilter.PrefabInstances);
                IReadOnlyCollection<TransformNode> sameContextNodes = ApplyWalkFilter(graph, startTransforms, ContextWalkFilter.SameContextsAsSelection);

                // Assert
                Transform[] expectedSceneTransforms =
                {
                    scene.GetTransform("Root 1"),
                    scene.GetTransform("Root 1/Child 1")
                };

                Transform[] expectedPrefabInstanceTransforms = GetAllTransforms(prefabInstanceOne.transform)
                    .Concat(GetAllTransforms(prefabInstanceTwo.transform))
                    .ToArray();

                Transform[] expectedAllTransforms = expectedSceneTransforms
                    .Concat(expectedPrefabInstanceTransforms)
                    .ToArray();

                Transform[] expectedSameContextTransforms = expectedSceneTransforms
                    .Concat(GetAllTransforms(prefabInstanceOne.transform))
                    .ToArray();

                CollectionAssert.AreEquivalent(expectedAllTransforms, allNodes.Select(node => node.Transform).ToArray());
                CollectionAssert.AreEquivalent(expectedSceneTransforms, sceneNodes.Select(node => node.Transform).ToArray());
                CollectionAssert.AreEquivalent(expectedPrefabInstanceTransforms, prefabInstanceNodes.Select(node => node.Transform).ToArray());
                CollectionAssert.AreEquivalent(expectedSameContextTransforms, sameContextNodes.Select(node => node.Transform).ToArray());
            }
            finally
            {
                if (prefabInstanceOne)
                    Object.DestroyImmediate(prefabInstanceOne);

                if (prefabInstanceTwo)
                    Object.DestroyImmediate(prefabInstanceTwo);

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void GraphFilter_GivenPrefabAssetHierarchy_ReturnsOnlyPrefabAssetTransformsForPrefabAssetFilter()
        {
            // Set up prefab asset
            TestPrefab prefab = CreateContextBoundaryPrefab();
            PrefabStage prefabStage = prefab.OpenStage();

            Transform[] startTransforms =
            {
                prefabStage.prefabContentsRoot.transform,
                prefab.GetTransform(prefabStage.prefabContentsRoot.transform, "Prefab Root/Scoped Child")
            };

            try
            {
                // Build graph and apply filters
                InjectionGraph graph = new(startTransforms);
                IReadOnlyCollection<TransformNode> prefabAssetNodes = ApplyWalkFilter(graph, startTransforms, ContextWalkFilter.PrefabAssetObjects);
                IReadOnlyCollection<TransformNode> sceneNodes = ApplyWalkFilter(graph, startTransforms, ContextWalkFilter.SceneObjects);
                IReadOnlyCollection<TransformNode> prefabInstanceNodes = ApplyWalkFilter(graph, startTransforms, ContextWalkFilter.PrefabInstances);
                IReadOnlyCollection<TransformNode> sameContextNodes = ApplyWalkFilter(graph, startTransforms, ContextWalkFilter.SameContextsAsSelection);

                // Assert
                Transform[] expectedTransforms = GetAllTransforms(prefabStage.prefabContentsRoot.transform);

                CollectionAssert.AreEquivalent(expectedTransforms, prefabAssetNodes.Select(node => node.Transform).ToArray());
                CollectionAssert.IsEmpty(sceneNodes.ToArray());
                CollectionAssert.IsEmpty(prefabInstanceNodes.ToArray());
                CollectionAssert.AreEquivalent(expectedTransforms, sameContextNodes.Select(node => node.Transform).ToArray());

                CollectionAssert.AreEqual
                (
                    new[] { ContextType.PrefabAsset },
                    prefabAssetNodes.Select(node => node.ContextIdentity.Type).Distinct().ToArray()
                );
            }
            finally
            {
                TestPrefab.CloseStage();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void ContextIdentity_GivenTwoSceneObjectsInSameScene_AreEqualAndShareSceneContextId()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);

            // Capture context identities
            ContextIdentity rootIdentity = new(scene.GetTransform("Root 1"));
            ContextIdentity childIdentity = new(scene.GetTransform("Root 1/Child 2"));

            // Assert
            Assert.That(rootIdentity.Type, Is.EqualTo(ContextType.SceneObject));
            Assert.That(childIdentity.Type, Is.EqualTo(ContextType.SceneObject));
            Assert.That(rootIdentity.IsPrefab, Is.False);
            Assert.That(childIdentity.IsPrefab, Is.False);
            Assert.That(childIdentity, Is.EqualTo(rootIdentity));
            Assert.That(childIdentity.Id, Is.EqualTo(rootIdentity.Id));
        }

        [Test]
        public void InjectionContext_GivenFilteredGraph_ProjectsActiveTransformsComponentsScopesAndBindings()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestScope rootScope = scene.Add<TestScope>("Root 1");
            GraphMetadataTarget target = scene.Add<GraphMetadataTarget>("Root 1/Child 1");
            TestScope childScope = scene.Add<TestScope>("Root 1/Child 2");

            // Bind
            rootScope.BindComponent<ComponentDependency>().FromSelf();
            childScope.BindAsset<AssetDependency>().FromResources("AssetDependency 1");

            // Build graph, filter and create injection context
            InjectionGraph graph = new(new[] { scene.GetTransform("Root 1") });

            IReadOnlyCollection<TransformNode> activeTransformNodes = ApplyWalkFilter
            (
                graph,
                new[] { scene.GetTransform("Root 1") },
                ContextWalkFilter.SceneObjects
            );

            InjectionContext injectionContext = CreateInjectionContext(activeTransformNodes);

            // Assert
            Assert.That(target, Is.Not.Null);

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    scene.GetTransform("Root 1"),
                    scene.GetTransform("Root 1/Child 1"),
                    scene.GetTransform("Root 1/Child 2")
                },
                injectionContext.ActiveTransformNodes.Select(node => node.Transform).ToArray()
            );

            CollectionAssert.AreEquivalent
            (
                new[] { typeof(GraphMetadataTarget) },
                injectionContext.ActiveComponentNodes.Select(node => node.Component.GetType()).ToArray()
            );

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    typeof(TestScope),
                    typeof(TestScope)
                },
                injectionContext.ActiveScopeNodes.Select(node => node.ScopeType).ToArray()
            );

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    typeof(ComponentBindingNode),
                    typeof(AssetBindingNode)
                },
                injectionContext.ActiveBindingNodes.Select(node => node.GetType()).ToArray()
            );
        }

        private static IReadOnlyCollection<TransformNode> ApplyWalkFilter(
            InjectionGraph graph,
            Transform[] startTransforms,
            ContextWalkFilter contextWalkFilter)
        {
            InjectionProgressTracker progressTracker = new(totalSegments: 1);

            try
            {
                return GraphFilter.ApplyWalkFilter(graph, startTransforms, contextWalkFilter, progressTracker);
            }
            finally
            {
                progressTracker.ClearProgressBar();
            }
        }

        private static Transform[] GetAllTransforms(Transform root)
        {
            List<Transform> transforms = new();
            CollectTransforms(root, transforms);
            return transforms.ToArray();
        }

        private static InjectionContext CreateInjectionContext(IReadOnlyCollection<TransformNode> activeTransformNodes)
        {
            InjectionProgressTracker progressTracker = new(totalSegments: 1);

            try
            {
                return new InjectionContext(activeTransformNodes, progressTracker);
            }
            finally
            {
                progressTracker.ClearProgressBar();
            }
        }

        private static void CollectTransforms(
            Transform current,
            ICollection<Transform> transforms)
        {
            transforms.Add(current);

            foreach (Transform child in current)
                CollectTransforms(child, transforms);
        }

        private static TestPrefab CreateContextBoundaryPrefab()
        {
            TestPrefab prefab = TestPrefab.Create("Prefab Root");
            prefab.Add<GraphMetadataTarget>("Prefab Root");
            prefab.AddTransform("Prefab Root/Unscoped Child");
            prefab.AddTransform("Prefab Root/Scoped Child");
            prefab.Add<TestScope>("Prefab Root/Scoped Child");
            prefab.AddTransform("Prefab Root/Scoped Child/Scoped Grandchild");
            return prefab;
        }
    }
}
