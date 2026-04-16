using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Utilities;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Utilities
{
    public class SignatureUtilityTests
    {
        [Test]
        public void GetBindingSignature_GivenQualifiedComponentBinding_ReturnsReadableSignature()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            Transform customTargetTransform = scene.GetTransform("Root 1/Child 1");

            // Bind
            scope.BindComponents<IDependency, ComponentDependency>()
                .ToID("qualified", "alternate")
                .ToMember("dependency", "otherDependency")
                .ToTarget<SingleConcreteComponentTarget>()
                .ToTarget(typeof(SingleInterfaceTarget))
                .FromChildWithIndexOf(customTargetTransform, 1)
                .WhereComponent(component => component.GetType() == typeof(ComponentDependency))
                .WhereRoot(transform => transform.name == "Root 1");

            // Find signature
            TransformNode transformNode = new(scene.GetTransform("Root 1"));
            ScopeNode scopeNode = transformNode.DeclaredScopeNode;
            Assert.That(scopeNode, Is.Not.Null);
            ComponentBindingNode bindingNode = scopeNode.BindingNodes.OfType<ComponentBindingNode>().Single();
            string signature = SignatureUtility.GetBindingSignature(bindingNode);

            // Assert
            Assert.That(bindingNode, Is.Not.Null);
            Assert.That(signature, Is.EqualTo("[Binding: BindComponents<IDependency, ComponentDependency>().ToId(\"qualified\", \"alternate\").ToTarget<SingleConcreteComponentTarget, SingleInterfaceTarget>().ToMember(\"dependency\", \"otherDependency\").FromChildWithIndexOf(Child 1, 1).WhereComponent(...).WhereRoot(...) | Scope: TestScope]"));
        }

        [Test]
        public void GetBindingSignature_GivenAssetBinding_ReturnsReadableSignature()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindAssets<IDependency, AssetDependency>()
                .ToID("asset-qualified")
                .ToMember("dependency")
                .ToTarget<SingleConcreteAssetTarget>()
                .FromFolder("Assets/Tests/Saneject/Fixtures/Resources")
                .Where(asset => asset.name != "AssetDependency 2");

            // Find signature
            TransformNode transformNode = new(scene.GetTransform("Root 1"));
            ScopeNode scopeNode = transformNode.DeclaredScopeNode;
            Assert.That(scopeNode, Is.Not.Null);
            AssetBindingNode bindingNode = scopeNode.BindingNodes.OfType<AssetBindingNode>().Single();
            string signature = SignatureUtility.GetBindingSignature(bindingNode);

            // Assert
            Assert.That(bindingNode, Is.Not.Null);
            Assert.That(signature, Is.EqualTo("[Binding: BindAssets<IDependency, AssetDependency>().ToId(\"asset-qualified\").ToTarget<SingleConcreteAssetTarget>().ToMember(\"dependency\").FromFolder(\"Assets/Tests/Saneject/Fixtures/Resources\").Where(...) | Scope: TestScope]"));
        }

        [Test]
        public void GetBindingSignature_GivenGlobalAndRuntimeProxyBindings_ReturnsReadableSignatures()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindGlobal<ComponentDependency>()
                .FromTargetAncestors(includeSelf: true);

            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy();

            // Find signatures
            TransformNode transformNode = new(scene.GetTransform("Root 1"));
            ScopeNode scopeNode = transformNode.DeclaredScopeNode;
            Assert.That(scopeNode, Is.Not.Null);
            GlobalComponentBindingNode globalBindingNode = scopeNode.BindingNodes.OfType<GlobalComponentBindingNode>().Single();
            ComponentBindingNode runtimeProxyBindingNode = scopeNode.BindingNodes
                .OfType<ComponentBindingNode>()
                .Single(node => node.RuntimeProxyConfig != null);
            string globalSignature = SignatureUtility.GetBindingSignature(globalBindingNode);
            string runtimeProxySignature = SignatureUtility.GetBindingSignature(runtimeProxyBindingNode);

            // Assert
            Assert.That(globalBindingNode, Is.Not.Null);
            Assert.That(runtimeProxyBindingNode, Is.Not.Null);
            Assert.That(globalSignature, Is.EqualTo("[Binding: BindGlobal<ComponentDependency>().FromTargetAncestors() | Scope: TestScope]"));
            Assert.That(runtimeProxySignature, Is.EqualTo("[Binding: BindComponent<IDependency, ComponentDependency>().FromRuntimeProxy().FromGlobalScope() | Scope: TestScope]"));
        }

        [Test]
        public void GetHypotheticalBindingSignature_GivenRequestedTypes_ReturnsReadableSignatures()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Find scope node and signatures
            TransformNode transformNode = new(scene.GetTransform("Root 1"));
            ScopeNode scopeNode = transformNode.DeclaredScopeNode;
            Assert.That(scopeNode, Is.Not.Null);
            string componentSignature = SignatureUtility.GetHypotheticalBindingSignature(typeof(ComponentDependency), false, scopeNode);
            string assetSignature = SignatureUtility.GetHypotheticalBindingSignature(typeof(AssetDependency), false, scopeNode);
            string mixedSignature = SignatureUtility.GetHypotheticalBindingSignature(typeof(IDependency), true, null);

            // Assert
            Assert.That(componentSignature, Is.EqualTo("[Binding: BindComponent<ComponentDependency>() | Nearest scope: TestScope]"));
            Assert.That(assetSignature, Is.EqualTo("[Binding: BindAsset<AssetDependency>() | Nearest scope: TestScope]"));
            Assert.That(mixedSignature, Is.EqualTo("[Binding: BindComponents<IDependency>() or BindAssets<IDependency>() | Nearest scope: null]"));
        }

        [Test]
        public void GetFieldSignature_GivenFieldAndPropertyNodes_ReturnsReadableSignatures()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.Add<NestedRootTarget>("Root 1");

            // Find field nodes and signatures
            TransformNode transformNode = new(scene.GetTransform("Root 1"));
            ComponentNode componentNode = transformNode.ComponentNodes.Single(node => node.Component is NestedRootTarget);
            Assert.That(componentNode, Is.Not.Null);
            FieldNode fieldNode = componentNode.FieldNodes.Single(node => node.QualifyingName == "fieldDependency");
            FieldNode propertyNode = componentNode.FieldNodes.Single(node => node.QualifyingName == "PropertyDependencies");
            string fieldSignature = SignatureUtility.GetFieldSignature(fieldNode);
            string propertySignature = SignatureUtility.GetFieldSignature(propertyNode);

            // Assert
            Assert.That(fieldNode, Is.Not.Null);
            Assert.That(propertyNode, Is.Not.Null);
            Assert.That(fieldSignature, Is.EqualTo("[Field: Root 1/NestedRootTarget/fieldDependency | Inject ID: field-id]"));
            Assert.That(propertySignature, Is.EqualTo("[Property: Root 1/NestedRootTarget/PropertyDependencies | Inject ID: property-id]"));
        }

        [Test]
        public void GetMethodSignature_GivenMethodAndParameterNode_ReturnsReadableSignatures()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.Add<NestedRootTarget>("Root 1");

            // Find method node and signatures
            TransformNode transformNode = new(scene.GetTransform("Root 1"));
            ComponentNode componentNode = transformNode.ComponentNodes.Single(node => node.Component is NestedRootTarget);
            Assert.That(componentNode, Is.Not.Null);
            MethodNode methodNode = componentNode.MethodNodes.Single(node => node.MethodName == "InjectTopLevel");
            MethodParameterNode parameterNode = methodNode.ParameterNodes.First();
            string methodSignature = SignatureUtility.GetMethodSignature(methodNode);
            string parameterSignature = SignatureUtility.GetMethodParameterSignature(parameterNode);

            // Assert
            Assert.That(methodNode, Is.Not.Null);
            Assert.That(parameterNode, Is.Not.Null);
            Assert.That(methodSignature, Is.EqualTo("[Method: Root 1/NestedRootTarget/InjectTopLevel | Inject ID: method-id]"));
            Assert.That(parameterSignature, Is.EqualTo("[Method: Root 1/NestedRootTarget/InjectTopLevel | Inject ID: method-id | Parameter: singleDependency]"));
        }
    }
}
