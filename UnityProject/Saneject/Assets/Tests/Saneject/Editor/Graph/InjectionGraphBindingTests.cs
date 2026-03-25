using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Runtime.Bindings.Asset;
using Plugins.Saneject.Runtime.Bindings.Component;
using Plugins.Saneject.Runtime.Proxy;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Graph
{
    public class InjectionGraphBindingTests
    {
        [Test]
        public void ScopeNode_GivenComponentBindings_CapturesComponentBindingMetadata()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            ComponentDependency instanceDependency = scene.Add<ComponentDependency>("Root 1/Child 1");
            Transform customTargetTransform = scene.GetTransform("Root 1/Child 2");
            GameObject proxyPrefab = new("Runtime Proxy Prefab");

            // Bind
            scope.BindComponent<ComponentDependency>();

            scope.BindComponent<ComponentDependency>()
                .FromInstance(instanceDependency);

            scope.BindComponents<IDependency, ComponentDependency>()
                .ToID("qualified", "alternate")
                .ToMember("dependency", "otherDependency")
                .ToTarget<SingleConcreteComponentTarget>()
                .ToTarget(typeof(SingleInterfaceTarget))
                .FromChildWithIndexOf(customTargetTransform, 1)
                .Where(component => component != null)
                .WhereComponent(component => component.GetType() == typeof(ComponentDependency))
                .WhereTransform(transform => transform.name == "Child 1")
                .WhereGameObject(gameObject => gameObject.name == "Child 1")
                .WhereParent(transform => transform != null)
                .WhereAnyAncestor(transform => transform.name == "Root 1", includeSelf: true, maxDepth: 3)
                .WhereRoot(transform => transform.name == "Root 1")
                .WhereAnyChild(transform => transform.name == "Child 1")
                .WhereChildAt(0, transform => transform.name == "Child 1")
                .WhereFirstChild(transform => transform.name == "Child 1")
                .WhereLastChild(transform => transform.name == "Child 2")
                .WhereAnyDescendant(transform => transform.name == "Child 2", includeSelf: true)
                .WhereAnySibling(transform => transform.name == "Child 1");

            scope.BindComponent<ComponentDependency>()
                .FromAnywhere(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);

            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy()
                .FromComponentOnPrefab(proxyPrefab, dontDestroyOnLoad: false)
                .AsSingleton();

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");

            ComponentBindingNode unconfiguredBinding = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single(binding =>
                    !binding.LocatorStrategySpecified &&
                    binding.RuntimeProxyConfig == null &&
                    binding.ResolveFromInstances.Count == 0 &&
                    binding.InterfaceType == null &&
                    binding.ConcreteType == typeof(ComponentDependency) &&
                    !binding.IsCollectionBinding);

            ComponentBindingNode instanceBinding = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single(binding =>
                    binding.SearchOrigin == SearchOrigin.Instance &&
                    binding.ResolveFromInstances.Count == 1 &&
                    binding.RuntimeProxyConfig == null);

            ComponentBindingNode filteredBinding = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single(binding =>
                    binding.SearchOrigin == SearchOrigin.CustomTargetTransform &&
                    binding.SearchDirection == SearchDirection.ChildAtIndex);

            ComponentBindingNode anywhereBinding = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single(binding =>
                    binding.SearchOrigin == SearchOrigin.Scene &&
                    binding.SearchDirection == SearchDirection.Anywhere);

            ComponentBindingNode runtimeProxyBinding = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single(binding => binding.RuntimeProxyConfig != null);

            // Assert
            Assert.That(instanceDependency, Is.Not.Null);
            Assert.That(proxyPrefab, Is.Not.Null);

            Assert.That(unconfiguredBinding.LocatorStrategySpecified, Is.False);
            Assert.That(unconfiguredBinding.SearchOrigin, Is.EqualTo(SearchOrigin.Scope));
            Assert.That(unconfiguredBinding.SearchDirection, Is.EqualTo(SearchDirection.None));
            CollectionAssert.IsEmpty(unconfiguredBinding.DependencyFilters.ToArray());
            CollectionAssert.IsEmpty(unconfiguredBinding.ResolveFromInstances.ToArray());

            Assert.That(instanceBinding.LocatorStrategySpecified, Is.True);
            Assert.That(instanceBinding.SearchOrigin, Is.EqualTo(SearchOrigin.Instance));
            Assert.That(instanceBinding.SearchDirection, Is.EqualTo(SearchDirection.None));
            CollectionAssert.AreEqual(new Object[] { instanceDependency }, instanceBinding.ResolveFromInstances.ToArray());

            Assert.That(filteredBinding.InterfaceType, Is.EqualTo(typeof(IDependency)));
            Assert.That(filteredBinding.ConcreteType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(filteredBinding.IsCollectionBinding, Is.True);
            Assert.That(filteredBinding.LocatorStrategySpecified, Is.True);
            Assert.That(filteredBinding.SearchOrigin, Is.EqualTo(SearchOrigin.CustomTargetTransform));
            Assert.That(filteredBinding.SearchDirection, Is.EqualTo(SearchDirection.ChildAtIndex));
            Assert.That(filteredBinding.CustomTargetTransform, Is.EqualTo(customTargetTransform));
            Assert.That(filteredBinding.ChildIndexForSearch, Is.EqualTo(1));

            CollectionAssert.AreEqual(new[]
            {
                "qualified",
                "alternate"
            }, filteredBinding.IdQualifiers.ToArray());

            CollectionAssert.AreEqual(new[]
            {
                "dependency",
                "otherDependency"
            }, filteredBinding.MemberNameQualifiers.ToArray());

            CollectionAssert.AreEqual
            (
                new[]
                {
                    typeof(SingleConcreteComponentTarget),
                    typeof(SingleInterfaceTarget)
                },
                filteredBinding.TargetTypeQualifiers.ToArray()
            );

            CollectionAssert.AreEqual
            (
                new[]
                {
                    ComponentFilterType.Where,
                    ComponentFilterType.WhereComponent,
                    ComponentFilterType.WhereTransform,
                    ComponentFilterType.WhereGameObject,
                    ComponentFilterType.WhereParent,
                    ComponentFilterType.WhereAnyAncestor,
                    ComponentFilterType.WhereRoot,
                    ComponentFilterType.WhereAnyChild,
                    ComponentFilterType.WhereChildAt,
                    ComponentFilterType.WhereFirstChild,
                    ComponentFilterType.WhereLastChild,
                    ComponentFilterType.WhereAnyDescendant,
                    ComponentFilterType.WhereAnySibling
                },
                filteredBinding.DependencyFilters
                    .Cast<ComponentFilter>()
                    .Select(filter => filter.FilterType)
                    .ToArray()
            );

            Assert.That(anywhereBinding.LocatorStrategySpecified, Is.True);
            Assert.That(anywhereBinding.SearchOrigin, Is.EqualTo(SearchOrigin.Scene));
            Assert.That(anywhereBinding.SearchDirection, Is.EqualTo(SearchDirection.Anywhere));
            Assert.That(anywhereBinding.FindObjectsInactive, Is.EqualTo(FindObjectsInactive.Exclude));
            Assert.That(anywhereBinding.FindObjectsSortMode, Is.EqualTo(FindObjectsSortMode.InstanceID));

            Assert.That(runtimeProxyBinding.InterfaceType, Is.EqualTo(typeof(IDependency)));
            Assert.That(runtimeProxyBinding.ConcreteType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(runtimeProxyBinding.LocatorStrategySpecified, Is.True);
            Assert.That(runtimeProxyBinding.RuntimeProxyConfig, Is.Not.Null);
            Assert.That(runtimeProxyBinding.RuntimeProxyConfig.ResolveMethod, Is.EqualTo(RuntimeProxyResolveMethod.FromComponentOnPrefab));
            Assert.That(runtimeProxyBinding.RuntimeProxyConfig.InstanceMode, Is.EqualTo(RuntimeProxyInstanceMode.Singleton));
            Assert.That(runtimeProxyBinding.RuntimeProxyConfig.Prefab, Is.EqualTo(proxyPrefab));
            Assert.That(runtimeProxyBinding.RuntimeProxyConfig.DontDestroyOnLoad, Is.True);

            Object.DestroyImmediate(proxyPrefab);
        }

        [Test]
        public void ScopeNode_GivenAssetBindings_CapturesAssetBindingMetadata()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Find dependencies
            AssetDependency firstAsset = Resources.Load<AssetDependency>("AssetDependency 1");
            AssetDependency secondAsset = Resources.Load<AssetDependency>("AssetDependency 2");

            // Bind
            scope.BindAssets<IDependency, AssetDependency>()
                .ToID("asset-qualified")
                .ToMember("dependency", "otherDependency")
                .ToTarget<SingleConcreteAssetTarget>()
                .ToTarget(typeof(SingleConcreteAssetOtherTarget))
                .FromFolder("Assets/Tests/Saneject/Fixtures/Resources")
                .Where(asset => asset.name != "AssetDependency 2");

            scope.BindAsset<AssetDependency>()
                .FromMethod(() => firstAsset);

            scope.BindAsset<AssetDependency>()
                .FromResources("AssetDependency 2");

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");

            AssetBindingNode folderBinding = scopeNode.BindingNodes
                .OfType<AssetBindingNode>()
                .Single(binding => binding.AssetLoadType == AssetLoadType.Folder);

            AssetBindingNode instanceBinding = scopeNode.BindingNodes
                .OfType<AssetBindingNode>()
                .Single(binding => binding.AssetLoadType == AssetLoadType.Instance);

            AssetBindingNode resourcesBinding = scopeNode.BindingNodes
                .OfType<AssetBindingNode>()
                .Single(binding =>
                    binding.AssetLoadType == AssetLoadType.Resources &&
                    binding.Path == "AssetDependency 2");

            // Assert
            Assert.That(firstAsset, Is.Not.Null);
            Assert.That(secondAsset, Is.Not.Null);

            Assert.That(folderBinding.InterfaceType, Is.EqualTo(typeof(IDependency)));
            Assert.That(folderBinding.ConcreteType, Is.EqualTo(typeof(AssetDependency)));
            Assert.That(folderBinding.IsCollectionBinding, Is.True);
            Assert.That(folderBinding.LocatorStrategySpecified, Is.True);
            Assert.That(folderBinding.Path, Is.EqualTo("Assets/Tests/Saneject/Fixtures/Resources"));
            Assert.That(folderBinding.AssetLoadType, Is.EqualTo(AssetLoadType.Folder));
            CollectionAssert.AreEqual(new[] { "asset-qualified" }, folderBinding.IdQualifiers.ToArray());

            CollectionAssert.AreEqual(new[]
            {
                "dependency",
                "otherDependency"
            }, folderBinding.MemberNameQualifiers.ToArray());

            CollectionAssert.AreEqual
            (
                new[]
                {
                    typeof(SingleConcreteAssetTarget),
                    typeof(SingleConcreteAssetOtherTarget)
                },
                folderBinding.TargetTypeQualifiers.ToArray()
            );

            CollectionAssert.AreEqual
            (
                new[] { AssetFilterType.Where },
                folderBinding.DependencyFilters
                    .Cast<AssetFilter>()
                    .Select(filter => filter.FilterType)
                    .ToArray()
            );

            Assert.That(instanceBinding.InterfaceType, Is.Null);
            Assert.That(instanceBinding.ConcreteType, Is.EqualTo(typeof(AssetDependency)));
            Assert.That(instanceBinding.IsCollectionBinding, Is.False);
            Assert.That(instanceBinding.LocatorStrategySpecified, Is.True);
            Assert.That(instanceBinding.Path, Is.Null);
            CollectionAssert.AreEqual(new Object[] { firstAsset }, instanceBinding.ResolveFromInstances.ToArray());

            Assert.That(resourcesBinding.InterfaceType, Is.Null);
            Assert.That(resourcesBinding.ConcreteType, Is.EqualTo(typeof(AssetDependency)));
            Assert.That(resourcesBinding.IsCollectionBinding, Is.False);
            Assert.That(resourcesBinding.LocatorStrategySpecified, Is.True);
            Assert.That(resourcesBinding.Path, Is.EqualTo("AssetDependency 2"));
            Assert.That(resourcesBinding.AssetLoadType, Is.EqualTo(AssetLoadType.Resources));
            CollectionAssert.IsEmpty(resourcesBinding.ResolveFromInstances.ToArray());
        }

        [Test]
        public void ScopeNode_GivenGlobalBinding_CreatesGlobalComponentBindingNode()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindGlobal<ComponentDependency>()
                .FromTargetAncestors(includeSelf: true);

            // Build graph and fetch binding node
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");

            GlobalComponentBindingNode bindingNode = scopeNode.BindingNodes
                .OfType<GlobalComponentBindingNode>()
                .Single();

            // Assert
            Assert.That(scope, Is.Not.Null);
            Assert.That(bindingNode.InterfaceType, Is.Null);
            Assert.That(bindingNode.ConcreteType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(bindingNode.IsCollectionBinding, Is.False);
            Assert.That(bindingNode.LocatorStrategySpecified, Is.True);
            Assert.That(bindingNode.SearchOrigin, Is.EqualTo(SearchOrigin.InjectionTarget));
            Assert.That(bindingNode.SearchDirection, Is.EqualTo(SearchDirection.Ancestors));
            Assert.That(bindingNode.IncludeSelfInSearch, Is.True);
            Assert.That(bindingNode.RuntimeProxyConfig, Is.Null);
            CollectionAssert.IsEmpty(bindingNode.DependencyFilters.ToArray());
            CollectionAssert.IsEmpty(bindingNode.ResolveFromInstances.ToArray());
            CollectionAssert.IsEmpty(bindingNode.IdQualifiers.ToArray());
            CollectionAssert.IsEmpty(bindingNode.MemberNameQualifiers.ToArray());
            CollectionAssert.IsEmpty(bindingNode.TargetTypeQualifiers.ToArray());
        }

        [Test]
        public void ScopeNode_GivenRuntimeProxyBindingWithoutFurtherConfiguration_CapturesDefaultRuntimeProxyMetadata()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy();

            // Build graph and fetch binding node
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");

            ComponentBindingNode bindingNode = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single();

            // Assert
            Assert.That(bindingNode.InterfaceType, Is.EqualTo(typeof(IDependency)));
            Assert.That(bindingNode.ConcreteType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(bindingNode.LocatorStrategySpecified, Is.True);
            Assert.That(bindingNode.RuntimeProxyConfig, Is.Not.Null);
            Assert.That(bindingNode.RuntimeProxyConfig.ResolveMethod, Is.EqualTo(RuntimeProxyResolveMethod.FromGlobalScope));
            Assert.That(bindingNode.RuntimeProxyConfig.InstanceMode, Is.EqualTo(RuntimeProxyInstanceMode.Singleton));
            Assert.That(bindingNode.RuntimeProxyConfig.Prefab, Is.Null);
            Assert.That(bindingNode.RuntimeProxyConfig.DontDestroyOnLoad, Is.False);
            CollectionAssert.IsEmpty(bindingNode.ResolveFromInstances.ToArray());
        }

        [Test]
        public void ScopeNode_GivenComponentCollectionBindingFromMethodEnumerable_CapturesResolvedInstanceMetadata()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            ComponentDependency firstDependency = scene.Add<ComponentDependency>("Root 1");
            ComponentDependency secondDependency = scene.Add<ComponentDependency>("Root 1/Child 1");

            // Bind
            scope.BindComponents<ComponentDependency>()
                .FromMethod(() => new[]
                {
                    firstDependency,
                    secondDependency
                });

            // Build graph and fetch binding node
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");

            ComponentBindingNode bindingNode = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single();

            // Assert
            Assert.That(firstDependency, Is.Not.Null);
            Assert.That(secondDependency, Is.Not.Null);
            Assert.That(bindingNode.InterfaceType, Is.Null);
            Assert.That(bindingNode.ConcreteType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(bindingNode.IsCollectionBinding, Is.True);
            Assert.That(bindingNode.SearchOrigin, Is.EqualTo(SearchOrigin.Instance));
            Assert.That(bindingNode.LocatorStrategySpecified, Is.True);

            CollectionAssert.AreEquivalent
            (
                new Object[]
                {
                    firstDependency,
                    secondDependency
                },
                bindingNode.ResolveFromInstances.ToArray()
            );
        }

        [Test]
        public void ScopeNode_GivenAssetBindingsWithAssetLoadAndResourcesAll_CapturesLoadTypeMetadata()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindAssets<AssetDependency>()
                .FromResourcesAll(string.Empty);

            scope.BindAsset<AssetDependency>()
                .FromAssetLoad("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset");

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1");

            AssetBindingNode resourcesAllBinding = scopeNode.BindingNodes
                .OfType<AssetBindingNode>()
                .Single(binding => binding.AssetLoadType == AssetLoadType.ResourcesAll);

            AssetBindingNode assetLoadBinding = scopeNode.BindingNodes
                .OfType<AssetBindingNode>()
                .Single(binding => binding.AssetLoadType == AssetLoadType.AssetLoad);

            // Assert
            Assert.That(resourcesAllBinding.ConcreteType, Is.EqualTo(typeof(AssetDependency)));
            Assert.That(resourcesAllBinding.IsCollectionBinding, Is.True);
            Assert.That(resourcesAllBinding.LocatorStrategySpecified, Is.True);
            Assert.That(resourcesAllBinding.Path, Is.EqualTo(string.Empty));
            Assert.That(resourcesAllBinding.AssetLoadType, Is.EqualTo(AssetLoadType.ResourcesAll));

            Assert.That(assetLoadBinding.ConcreteType, Is.EqualTo(typeof(AssetDependency)));
            Assert.That(assetLoadBinding.IsCollectionBinding, Is.False);
            Assert.That(assetLoadBinding.LocatorStrategySpecified, Is.True);
            Assert.That(assetLoadBinding.Path, Is.EqualTo("Assets/Tests/Saneject/Fixtures/Resources/AssetDependency 1.asset"));
            Assert.That(assetLoadBinding.AssetLoadType, Is.EqualTo(AssetLoadType.AssetLoad));
        }

        [Test]
        public void ScopeNode_GivenLocatorVariants_CapturesSearchOriginDirectionAndFlags()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1/Child 1");
            Transform customTargetTransform = scene.GetTransform("Root 1/Child 2");

            // Bind
            scope.BindComponent<ComponentDependency>()
                .FromScopeAncestors(includeSelf: true);

            scope.BindComponent<ComponentDependency>()
                .FromRootDescendants(includeSelf: true);

            scope.BindComponent<ComponentDependency>()
                .FromTargetLastChild();

            scope.BindComponent<ComponentDependency>()
                .FromParentOf(customTargetTransform);

            scope.BindComponent<ComponentDependency>()
                .FromScopeChildWithIndex(0);

            // Build graph and fetch binding nodes
            ScopeNode scopeNode = CreateScopeNode(scene, "Root 1/Child 1");

            ComponentBindingNode scopeAncestorsBinding = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single(binding =>
                    binding.SearchOrigin == SearchOrigin.Scope &&
                    binding.SearchDirection == SearchDirection.Ancestors);

            ComponentBindingNode rootDescendantsBinding = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single(binding =>
                    binding.SearchOrigin == SearchOrigin.Root &&
                    binding.SearchDirection == SearchDirection.Descendants);

            ComponentBindingNode targetLastChildBinding = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single(binding =>
                    binding.SearchOrigin == SearchOrigin.InjectionTarget &&
                    binding.SearchDirection == SearchDirection.LastChild);

            ComponentBindingNode customParentBinding = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single(binding =>
                    binding.SearchOrigin == SearchOrigin.CustomTargetTransform &&
                    binding.SearchDirection == SearchDirection.Parent);

            ComponentBindingNode scopeChildIndexBinding = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single(binding =>
                    binding.SearchOrigin == SearchOrigin.Scope &&
                    binding.SearchDirection == SearchDirection.ChildAtIndex);

            // Assert
            Assert.That(scopeAncestorsBinding.IncludeSelfInSearch, Is.True);
            Assert.That(rootDescendantsBinding.IncludeSelfInSearch, Is.True);
            Assert.That(targetLastChildBinding.IncludeSelfInSearch, Is.False);
            Assert.That(customParentBinding.CustomTargetTransform, Is.EqualTo(customTargetTransform));
            Assert.That(scopeChildIndexBinding.ChildIndexForSearch, Is.EqualTo(0));
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