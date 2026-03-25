using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.Equality
{
    public class BindingNodeEqualityTests
    {
        [Test]
        public void BindingNode_SameScope_TConcrete_WithoutQualifiers_IsEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();
            scope.BindComponent<ComponentDependency>().FromAnywhere();

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");
            BindingNode firstBinding = scopeNode.BindingNodes[0];
            BindingNode secondBinding = scopeNode.BindingNodes[1];

            // Assert
            Assert.That(firstBinding, Is.EqualTo(secondBinding));
        }

        [Test]
        public void BindingNode_SameScope_TAsset_WithoutQualifiers_IsEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindAsset<AssetDependency>().FromResources("AssetDependency 1");
            scope.BindAsset<AssetDependency>().FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");
            BindingNode firstBinding = scopeNode.BindingNodes[0];
            BindingNode secondBinding = scopeNode.BindingNodes[1];

            // Assert
            Assert.That(firstBinding, Is.EqualTo(secondBinding));
        }

        [Test]
        public void BindingNode_SameScope_TConcrete_SingleAndCollection_IsNotEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();
            scope.BindComponents<ComponentDependency>().FromSelf();

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");
            BindingNode singleBinding = scopeNode.BindingNodes[0];
            BindingNode collectionBinding = scopeNode.BindingNodes[1];

            // Assert
            Assert.That(singleBinding, Is.Not.EqualTo(collectionBinding));
        }

        [Test]
        public void BindingNode_SameScope_TConcreteAndTInterface_IsNotEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();
            scope.BindComponent<IDependency>().FromSelf();

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");
            BindingNode concreteBinding = scopeNode.BindingNodes[0];
            BindingNode interfaceBinding = scopeNode.BindingNodes[1];

            // Assert
            Assert.That(concreteBinding, Is.Not.EqualTo(interfaceBinding));
        }

        [Test]
        public void BindingNode_SameScope_TConcrete_ComponentAndGlobal_IsNotEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();
            scope.BindGlobal<ComponentDependency>().FromSelf();

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");
            BindingNode componentBinding = scopeNode.BindingNodes[0];
            BindingNode globalBinding = scopeNode.BindingNodes[1];

            // Assert
            Assert.That(componentBinding, Is.Not.EqualTo(globalBinding));
        }

        [Test]
        public void BindingNode_DifferentScopes_TConcrete_IsNotEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 1, depth: 1);
            TestScope firstScope = scene.Add<TestScope>("Root 1");
            TestScope secondScope = scene.Add<TestScope>("Root 2");

            // Bind
            firstScope.BindComponent<ComponentDependency>().FromSelf();
            secondScope.BindComponent<ComponentDependency>().FromSelf();

            // Build graph
            ScopeNode firstScopeNode = CreateScopeNode(scene, "Root 1");
            ScopeNode secondScopeNode = CreateScopeNode(scene, "Root 2");
            BindingNode firstBinding = firstScopeNode.BindingNodes[0];
            BindingNode secondBinding = secondScopeNode.BindingNodes[0];

            // Assert
            Assert.That(firstBinding, Is.Not.EqualTo(secondBinding));
        }

        [Test]
        public void BindingNode_SameScope_TConcrete_WithOverlappingQualifiers_IsEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .ToID("qualified", "alternate")
                .ToMember("dependency", "otherDependency")
                .ToTarget<SingleConcreteComponentTarget>()
                .FromSelf();

            scope.BindComponent<ComponentDependency>()
                .ToID("qualified")
                .ToMember("dependency")
                .ToTarget<SingleConcreteComponentTarget>()
                .FromAnywhere();

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");
            BindingNode firstBinding = scopeNode.BindingNodes[0];
            BindingNode secondBinding = scopeNode.BindingNodes[1];

            // Assert
            Assert.That(firstBinding, Is.EqualTo(secondBinding));
        }

        [Test]
        public void BindingNode_SameScope_TConcrete_WithOnlyIDQualifiers_IsNotEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .ToID("qualified")
                .FromSelf();

            scope.BindComponent<ComponentDependency>()
                .ToID("qualified")
                .FromAnywhere();

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");
            BindingNode firstBinding = scopeNode.BindingNodes[0];
            BindingNode secondBinding = scopeNode.BindingNodes[1];

            // Assert
            Assert.That(firstBinding, Is.Not.EqualTo(secondBinding));
        }

        [Test]
        public void BindingNode_SameScope_TConcrete_WithAssignableTargetQualifiers_IsEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .ToID("qualified")
                .ToMember("dependency")
                .ToTarget<SingleConcreteComponentTarget>()
                .FromSelf();

            scope.BindComponent<ComponentDependency>()
                .ToID("qualified")
                .ToMember("dependency")
                .ToTarget<MonoBehaviour>()
                .FromAnywhere();

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");
            BindingNode firstBinding = scopeNode.BindingNodes[0];
            BindingNode secondBinding = scopeNode.BindingNodes[1];

            // Assert
            Assert.That(firstBinding, Is.EqualTo(secondBinding));
        }

        [Test]
        public void BindingNode_SameScope_TConcrete_WithSameIDAndMemberButUnrelatedTargetQualifiers_IsNotEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .ToID("qualified")
                .ToMember("dependency")
                .ToTarget<SingleConcreteComponentTarget>()
                .FromSelf();

            scope.BindComponent<ComponentDependency>()
                .ToID("qualified")
                .ToMember("dependency")
                .ToTarget<SingleConcreteComponentOtherTarget>()
                .FromAnywhere();

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");
            BindingNode firstBinding = scopeNode.BindingNodes[0];
            BindingNode secondBinding = scopeNode.BindingNodes[1];

            // Assert
            Assert.That(firstBinding, Is.Not.EqualTo(secondBinding));
        }

        [Test]
        public void BindingNode_SameScope_TConcrete_WithSameTargetAndIDButDifferentMemberQualifiers_IsNotEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .ToID("qualified")
                .ToMember("dependency")
                .ToTarget<SingleConcreteComponentTarget>()
                .FromSelf();

            scope.BindComponent<ComponentDependency>()
                .ToID("qualified")
                .ToMember("otherDependency")
                .ToTarget<SingleConcreteComponentTarget>()
                .FromAnywhere();

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");
            BindingNode firstBinding = scopeNode.BindingNodes[0];
            BindingNode secondBinding = scopeNode.BindingNodes[1];

            // Assert
            Assert.That(firstBinding, Is.Not.EqualTo(secondBinding));
        }

        [Test]
        public void BindingNode_SameScope_TConcrete_WithSameTargetAndMemberButDifferentIDQualifiers_IsNotEqual()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .ToID("qualified")
                .ToMember("dependency")
                .ToTarget<SingleConcreteComponentTarget>()
                .FromSelf();

            scope.BindComponent<ComponentDependency>()
                .ToID("other")
                .ToMember("dependency")
                .ToTarget<SingleConcreteComponentTarget>()
                .FromAnywhere();

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");
            BindingNode firstBinding = scopeNode.BindingNodes[0];
            BindingNode secondBinding = scopeNode.BindingNodes[1];

            // Assert
            Assert.That(firstBinding, Is.Not.EqualTo(secondBinding));
        }

        private static ScopeNode CreateScopeNode(
            TestScene scene,
            string path)
        {
            TransformNode transformNode = new(scene.GetTransform(path));
            ScopeNode scopeNode = transformNode.DeclaredScopeNode;
            Assert.That(scopeNode, Is.Not.Null);
            return scopeNode;
        }
    }
}