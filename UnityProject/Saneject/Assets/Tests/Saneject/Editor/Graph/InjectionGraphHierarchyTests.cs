using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Data.Graph;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Runtime.Settings;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Graph
{
    public class InjectionGraphHierarchyTests
    {
        [Test]
        public void InjectionGraph_GivenOverlappingStartTransforms_CreatesDistinctRootTransformNodes()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 2, depth: 3);

            // Build graph
            InjectionGraph graph = CreateGraph
            (
                scene.GetTransform("Root 1"),
                scene.GetTransform("Root 1/Child 1"),
                scene.GetTransform("Root 2/Child 2")
            );

            // Assert
            Assert.That(graph.RootTransformNodes.Count, Is.EqualTo(2));

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    scene.GetTransform("Root 1"),
                    scene.GetTransform("Root 2")
                },
                graph.RootTransformNodes.Select(node => node.Transform).ToArray()
            );
        }

        [Test]
        public void TransformNode_GivenSceneHierarchy_MirrorsUnityHierarchyAndOnlyCapturesInjectableComponents()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1/Child 1");
            GraphMetadataTarget target = scene.Add<GraphMetadataTarget>("Root 1/Child 1");

            // Build graph
            InjectionGraph graph = CreateGraph(scene.GetTransform("Root 1"));
            TransformNode rootNode = GetTransformNode(graph, scene.GetTransform("Root 1"));
            TransformNode childNode = GetTransformNode(graph, scene.GetTransform("Root 1/Child 1"));
            TransformNode grandchildNode = GetTransformNode(graph, scene.GetTransform("Root 1/Child 1/Child 2"));

            // Assert
            Assert.That(scope, Is.Not.Null);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target, Is.Not.Null);
            Assert.That(rootNode.ParentTransformNode, Is.Null);
            Assert.That(childNode.ParentTransformNode, Is.EqualTo(rootNode));
            Assert.That(grandchildNode.ParentTransformNode, Is.EqualTo(childNode));

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    scene.GetTransform("Root 1/Child 1"),
                    scene.GetTransform("Root 1/Child 2")
                },
                rootNode.ChildTransformNodes.Select(node => node.Transform).ToArray()
            );

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    scene.GetTransform("Root 1/Child 1/Child 1"),
                    scene.GetTransform("Root 1/Child 1/Child 2")
                },
                childNode.ChildTransformNodes.Select(node => node.Transform).ToArray()
            );

            Assert.That(rootNode.DeclaredScopeNode, Is.Not.Null);
            Assert.That(rootNode.DeclaredScopeNode.Scope, Is.EqualTo(scope));
            Assert.That(rootNode.DeclaredScopeNode.ScopeType, Is.EqualTo(typeof(TestScope)));
            Assert.That(rootNode.NearestScopeNode, Is.EqualTo(rootNode.DeclaredScopeNode));

            Assert.That(childNode.DeclaredScopeNode, Is.Null);
            Assert.That(childNode.NearestScopeNode, Is.EqualTo(rootNode.DeclaredScopeNode));
            Assert.That(grandchildNode.NearestScopeNode, Is.EqualTo(rootNode.DeclaredScopeNode));

            CollectionAssert.IsEmpty(rootNode.ComponentNodes.ToArray());

            CollectionAssert.AreEquivalent
            (
                new[] { typeof(GraphMetadataTarget) },
                childNode.ComponentNodes.Select(node => node.Component.GetType()).ToArray()
            );
        }

        [Test]
        public void TransformNode_GivenNestedSceneScopes_CapturesDeclaredScopeParentChain()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            TestScope rootScope = scene.Add<TestScope>("Root 1");
            TestScope childScope = scene.Add<TestScope>("Root 1/Child 1");

            // Build graph
            InjectionGraph graph = CreateGraph(scene.GetTransform("Root 1"));
            TransformNode rootNode = GetTransformNode(graph, scene.GetTransform("Root 1"));
            TransformNode childNode = GetTransformNode(graph, scene.GetTransform("Root 1/Child 1"));
            TransformNode grandchildNode = GetTransformNode(graph, scene.GetTransform("Root 1/Child 1/Child 1"));

            // Assert
            Assert.That(rootScope, Is.Not.Null);
            Assert.That(childScope, Is.Not.Null);
            Assert.That(rootNode.DeclaredScopeNode, Is.Not.Null);
            Assert.That(rootNode.DeclaredScopeNode.ParentScopeNode, Is.Null);
            Assert.That(childNode.DeclaredScopeNode, Is.Not.Null);
            Assert.That(childNode.DeclaredScopeNode.ParentScopeNode, Is.EqualTo(rootNode.DeclaredScopeNode));
            Assert.That(childNode.NearestScopeNode, Is.EqualTo(childNode.DeclaredScopeNode));
            Assert.That(grandchildNode.DeclaredScopeNode, Is.Null);
            Assert.That(grandchildNode.NearestScopeNode, Is.EqualTo(childNode.DeclaredScopeNode));
        }

        [Test]
        public void TransformNode_GivenSceneHierarchy_CapturesSceneContextIdentityForAllSceneNodes()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);

            // Build graph
            InjectionGraph graph = CreateGraph(scene.GetTransform("Root 1/Child 1"));
            TransformNode rootNode = GetTransformNode(graph, scene.GetTransform("Root 1"));
            TransformNode firstChildNode = GetTransformNode(graph, scene.GetTransform("Root 1/Child 1"));
            TransformNode secondChildNode = GetTransformNode(graph, scene.GetTransform("Root 1/Child 2"));

            // Assert
            Assert.That(rootNode.ContextIdentity.Type, Is.EqualTo(ContextType.SceneObject));
            Assert.That(firstChildNode.ContextIdentity.Type, Is.EqualTo(ContextType.SceneObject));
            Assert.That(secondChildNode.ContextIdentity.Type, Is.EqualTo(ContextType.SceneObject));
            Assert.That(rootNode.ContextIdentity.IsPrefab, Is.False);
            Assert.That(firstChildNode.ContextIdentity, Is.EqualTo(rootNode.ContextIdentity));
            Assert.That(secondChildNode.ContextIdentity, Is.EqualTo(rootNode.ContextIdentity));
            Assert.That(firstChildNode.ContextIdentity.Id, Is.EqualTo(rootNode.ContextIdentity.Id));
            Assert.That(secondChildNode.ContextIdentity.Id, Is.EqualTo(rootNode.ContextIdentity.Id));
        }

        [Test]
        public void InjectionGraph_GivenSceneHierarchy_TraversalExtensionsEnumerateTransformsComponentsScopesAndBindings()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestScope rootScope = scene.Add<TestScope>("Root 1");
            GraphMetadataTarget target = scene.Add<GraphMetadataTarget>("Root 1/Child 1");
            TestScope childScope = scene.Add<TestScope>("Root 1/Child 2");

            // Bind
            rootScope.BindComponent<ComponentDependency>().FromSelf();
            childScope.BindAsset<AssetDependency>().FromResources("AssetDependency 1");

            // Build graph
            InjectionGraph graph = CreateGraph(scene.GetTransform("Root 1"));

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
                graph.EnumerateAllTransformNodes().Select(node => node.Transform).ToArray()
            );

            CollectionAssert.AreEquivalent
            (
                new[] { typeof(GraphMetadataTarget) },
                graph.EnumerateAllTransformNodes()
                    .EnumerateAllComponentNodes()
                    .Select(node => node.Component.GetType())
                    .ToArray()
            );

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    typeof(TestScope),
                    typeof(TestScope)
                },
                graph.EnumerateAllTransformNodes()
                    .EnumerateAllScopeNodes()
                    .Select(node => node.ScopeType)
                    .ToArray()
            );

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    typeof(ComponentBindingNode),
                    typeof(AssetBindingNode)
                },
                graph.EnumerateAllTransformNodes()
                    .EnumerateAllScopeNodes()
                    .EnumerateAllBindingNodes()
                    .Select(node => node.GetType())
                    .ToArray()
            );
        }

        [Test]
        public void TransformNode_WHEN_ContextIsolationIsFalse_SelectsExpectedScopesAcrossPrefabInstanceBoundary()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope sceneScope = scene.Add<TestScope>("Root 1");
            TestPrefab prefab = CreateScopeBoundaryPrefab();
            GameObject prefabInstanceRoot = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Build graph
                ProjectSettings.UseContextIsolation = false;
                InjectionGraph graph = CreateGraph(scene.GetTransform("Root 1"));
                ScopeNode sceneScopeNode = GetTransformNode(graph, scene.GetTransform("Root 1")).DeclaredScopeNode;
                Transform unscopedChild = prefab.GetTransform(prefabInstanceRoot.transform, "Prefab Root/Unscoped Child");
                Transform scopedChild = prefab.GetTransform(prefabInstanceRoot.transform, "Prefab Root/Scoped Child");
                Transform scopedGrandchild = prefab.GetTransform(prefabInstanceRoot.transform, "Prefab Root/Scoped Child/Scoped Grandchild");
                TransformNode unscopedChildNode = GetTransformNode(graph, unscopedChild);
                TransformNode scopedChildNode = GetTransformNode(graph, scopedChild);
                TransformNode scopedGrandchildNode = GetTransformNode(graph, scopedGrandchild);

                // Assert
                Assert.That(sceneScope, Is.Not.Null);
                Assert.That(sceneScopeNode, Is.Not.Null);
                Assert.That(unscopedChildNode.NearestScopeNode, Is.EqualTo(sceneScopeNode));
                Assert.That(scopedChildNode.DeclaredScopeNode, Is.Not.Null);
                Assert.That(scopedChildNode.NearestScopeNode, Is.EqualTo(scopedChildNode.DeclaredScopeNode));
                Assert.That(scopedChildNode.DeclaredScopeNode.ParentScopeNode, Is.EqualTo(sceneScopeNode));
                Assert.That(scopedGrandchildNode.NearestScopeNode, Is.EqualTo(scopedChildNode.DeclaredScopeNode));
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                if (prefabInstanceRoot)
                    Object.DestroyImmediate(prefabInstanceRoot);

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void TransformNode_WHEN_ContextIsolationIsTrue_SelectsExpectedScopesAcrossPrefabInstanceBoundary()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope sceneScope = scene.Add<TestScope>("Root 1");
            TestPrefab prefab = CreateScopeBoundaryPrefab();
            GameObject prefabInstanceRoot = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            bool originalUseContextIsolation = ProjectSettings.UseContextIsolation;

            try
            {
                // Build graph
                ProjectSettings.UseContextIsolation = true;
                InjectionGraph graph = CreateGraph(scene.GetTransform("Root 1"));
                ScopeNode sceneScopeNode = GetTransformNode(graph, scene.GetTransform("Root 1")).DeclaredScopeNode;
                Transform unscopedChild = prefab.GetTransform(prefabInstanceRoot.transform, "Prefab Root/Unscoped Child");
                Transform scopedChild = prefab.GetTransform(prefabInstanceRoot.transform, "Prefab Root/Scoped Child");
                Transform scopedGrandchild = prefab.GetTransform(prefabInstanceRoot.transform, "Prefab Root/Scoped Child/Scoped Grandchild");
                TransformNode unscopedChildNode = GetTransformNode(graph, unscopedChild);
                TransformNode scopedChildNode = GetTransformNode(graph, scopedChild);
                TransformNode scopedGrandchildNode = GetTransformNode(graph, scopedGrandchild);

                // Assert
                Assert.That(sceneScope, Is.Not.Null);
                Assert.That(sceneScopeNode, Is.Not.Null);
                Assert.That(unscopedChildNode.NearestScopeNode, Is.Null);
                Assert.That(scopedChildNode.DeclaredScopeNode, Is.Not.Null);
                Assert.That(scopedChildNode.NearestScopeNode, Is.EqualTo(scopedChildNode.DeclaredScopeNode));
                Assert.That(scopedChildNode.DeclaredScopeNode.ParentScopeNode, Is.Null);
                Assert.That(scopedGrandchildNode.NearestScopeNode, Is.EqualTo(scopedChildNode.DeclaredScopeNode));
            }
            finally
            {
                ProjectSettings.UseContextIsolation = originalUseContextIsolation;

                if (prefabInstanceRoot)
                    Object.DestroyImmediate(prefabInstanceRoot);

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        private static InjectionGraph CreateGraph(params Transform[] startTransforms)
        {
            return new InjectionGraph(startTransforms);
        }

        private static TransformNode GetTransformNode(
            InjectionGraph graph,
            Transform transform)
        {
            TransformNode transformNode = graph
                .EnumerateAllTransformNodes()
                .SingleOrDefault(node => node.Transform == transform);

            Assert.That(transformNode, Is.Not.Null);
            return transformNode;
        }

        private static TestPrefab CreateScopeBoundaryPrefab()
        {
            TestPrefab prefab = TestPrefab.Create("Prefab Root");
            prefab.AddTransform("Prefab Root/Unscoped Child");
            prefab.AddTransform("Prefab Root/Scoped Child");
            prefab.Add<TestScope>("Prefab Root/Scoped Child");
            prefab.AddTransform("Prefab Root/Scoped Child/Scoped Grandchild");
            return prefab;
        }
    }
}
