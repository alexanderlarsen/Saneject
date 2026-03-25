using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Graph;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Graph
{
    public class InjectionGraphMemberTests
    {
        [Test]
        public void FieldNode_GivenTopLevelFieldAndPropertyTargets_CapturesFieldMetadata()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.Add<SingleConcreteComponentTarget>("Root 1");
            scene.Add<SingleInterfaceTarget>("Root 1");
            scene.Add<SingleConcreteComponentPropertyTarget>("Root 1");
            scene.Add<MultiConcreteComponentPropertyTarget>("Root 1");
            scene.Add<MultiInterfacePropertyTarget>("Root 1");

            // Build graph and fetch component nodes
            TransformNode rootNode = new(scene.GetTransform("Root 1"));
            ComponentNode concreteFieldComponentNode = GetComponentNode<SingleConcreteComponentTarget>(rootNode);
            ComponentNode interfaceFieldComponentNode = GetComponentNode<SingleInterfaceTarget>(rootNode);
            ComponentNode concretePropertyComponentNode = GetComponentNode<SingleConcreteComponentPropertyTarget>(rootNode);
            ComponentNode multiConcretePropertyComponentNode = GetComponentNode<MultiConcreteComponentPropertyTarget>(rootNode);
            ComponentNode multiInterfacePropertyComponentNode = GetComponentNode<MultiInterfacePropertyTarget>(rootNode);

            // Assert
            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    typeof(SingleConcreteComponentTarget),
                    typeof(SingleInterfaceTarget),
                    typeof(SingleConcreteComponentPropertyTarget),
                    typeof(MultiConcreteComponentPropertyTarget),
                    typeof(MultiInterfacePropertyTarget)
                },
                rootNode.ComponentNodes.Select(node => node.Component.GetType()).ToArray()
            );

            FieldNode concreteFieldNode = concreteFieldComponentNode.FieldNodes.Single();
            Assert.That(concreteFieldNode.Owner, Is.EqualTo(concreteFieldComponentNode.Component));
            Assert.That(concreteFieldNode.DeclaringType, Is.EqualTo(typeof(SingleConcreteComponentTarget)));
            Assert.That(concreteFieldNode.QualifyingName, Is.EqualTo("dependency"));
            Assert.That(concreteFieldNode.InjectId, Is.Null);
            Assert.That(concreteFieldNode.SuppressMissingErrors, Is.False);
            Assert.That(concreteFieldNode.FieldType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(concreteFieldNode.RequestedType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(concreteFieldNode.TypeShape, Is.EqualTo(TypeShape.Single));
            Assert.That(concreteFieldNode.IsCollection, Is.False);
            Assert.That(concreteFieldNode.IsInterface, Is.False);
            Assert.That(concreteFieldNode.IsPropertyBackingField, Is.False);
            Assert.That(concreteFieldNode.DisplayPath, Is.EqualTo("Root 1/SingleConcreteComponentTarget/dependency"));
            Assert.That(concreteFieldNode.ShortPath, Is.EqualTo("SingleConcreteComponentTarget.dependency"));

            FieldNode interfaceFieldNode = interfaceFieldComponentNode.FieldNodes.Single();
            Assert.That(interfaceFieldNode.DeclaringType, Is.EqualTo(typeof(SingleInterfaceTarget)));
            Assert.That(interfaceFieldNode.QualifyingName, Is.EqualTo("dependency"));
            Assert.That(interfaceFieldNode.FieldType, Is.EqualTo(typeof(IDependency)));
            Assert.That(interfaceFieldNode.RequestedType, Is.EqualTo(typeof(IDependency)));
            Assert.That(interfaceFieldNode.TypeShape, Is.EqualTo(TypeShape.Single));
            Assert.That(interfaceFieldNode.IsCollection, Is.False);
            Assert.That(interfaceFieldNode.IsInterface, Is.True);
            Assert.That(interfaceFieldNode.IsPropertyBackingField, Is.False);
            Assert.That(interfaceFieldNode.DisplayPath, Is.EqualTo("Root 1/SingleInterfaceTarget/dependency"));
            Assert.That(interfaceFieldNode.ShortPath, Is.EqualTo("SingleInterfaceTarget.dependency"));

            FieldNode concretePropertyNode = concretePropertyComponentNode.FieldNodes.Single();
            Assert.That(concretePropertyNode.DeclaringType, Is.EqualTo(typeof(SingleConcreteComponentPropertyTarget)));
            Assert.That(concretePropertyNode.QualifyingName, Is.EqualTo("Dependency"));
            Assert.That(concretePropertyNode.FieldType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(concretePropertyNode.RequestedType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(concretePropertyNode.TypeShape, Is.EqualTo(TypeShape.Single));
            Assert.That(concretePropertyNode.IsCollection, Is.False);
            Assert.That(concretePropertyNode.IsInterface, Is.False);
            Assert.That(concretePropertyNode.IsPropertyBackingField, Is.True);
            Assert.That(concretePropertyNode.DisplayPath, Is.EqualTo("Root 1/SingleConcreteComponentPropertyTarget/Dependency"));
            Assert.That(concretePropertyNode.ShortPath, Is.EqualTo("SingleConcreteComponentPropertyTarget.Dependency"));

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    "Array",
                    "List"
                },
                multiConcretePropertyComponentNode.FieldNodes.Select(node => node.QualifyingName).ToArray()
            );

            FieldNode concreteArrayPropertyNode = GetFieldNode(multiConcretePropertyComponentNode, "MultiConcreteComponentPropertyTarget.Array");
            Assert.That(concreteArrayPropertyNode.RequestedType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(concreteArrayPropertyNode.TypeShape, Is.EqualTo(TypeShape.Array));
            Assert.That(concreteArrayPropertyNode.IsCollection, Is.True);
            Assert.That(concreteArrayPropertyNode.IsInterface, Is.False);
            Assert.That(concreteArrayPropertyNode.IsPropertyBackingField, Is.True);
            Assert.That(concreteArrayPropertyNode.DisplayPath, Is.EqualTo("Root 1/MultiConcreteComponentPropertyTarget/Array"));

            FieldNode concreteListPropertyNode = GetFieldNode(multiConcretePropertyComponentNode, "MultiConcreteComponentPropertyTarget.List");
            Assert.That(concreteListPropertyNode.RequestedType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(concreteListPropertyNode.TypeShape, Is.EqualTo(TypeShape.List));
            Assert.That(concreteListPropertyNode.IsCollection, Is.True);
            Assert.That(concreteListPropertyNode.IsInterface, Is.False);
            Assert.That(concreteListPropertyNode.IsPropertyBackingField, Is.True);
            Assert.That(concreteListPropertyNode.DisplayPath, Is.EqualTo("Root 1/MultiConcreteComponentPropertyTarget/List"));

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    "Array",
                    "List"
                },
                multiInterfacePropertyComponentNode.FieldNodes.Select(node => node.QualifyingName).ToArray()
            );

            FieldNode interfaceArrayPropertyNode = GetFieldNode(multiInterfacePropertyComponentNode, "MultiInterfacePropertyTarget.Array");
            Assert.That(interfaceArrayPropertyNode.RequestedType, Is.EqualTo(typeof(IDependency)));
            Assert.That(interfaceArrayPropertyNode.TypeShape, Is.EqualTo(TypeShape.Array));
            Assert.That(interfaceArrayPropertyNode.IsCollection, Is.True);
            Assert.That(interfaceArrayPropertyNode.IsInterface, Is.True);
            Assert.That(interfaceArrayPropertyNode.IsPropertyBackingField, Is.True);
            Assert.That(interfaceArrayPropertyNode.DisplayPath, Is.EqualTo("Root 1/MultiInterfacePropertyTarget/Array"));

            FieldNode interfaceListPropertyNode = GetFieldNode(multiInterfacePropertyComponentNode, "MultiInterfacePropertyTarget.List");
            Assert.That(interfaceListPropertyNode.RequestedType, Is.EqualTo(typeof(IDependency)));
            Assert.That(interfaceListPropertyNode.TypeShape, Is.EqualTo(TypeShape.List));
            Assert.That(interfaceListPropertyNode.IsCollection, Is.True);
            Assert.That(interfaceListPropertyNode.IsInterface, Is.True);
            Assert.That(interfaceListPropertyNode.IsPropertyBackingField, Is.True);
            Assert.That(interfaceListPropertyNode.DisplayPath, Is.EqualTo("Root 1/MultiInterfacePropertyTarget/List"));
        }

        [Test]
        public void MethodNode_GivenMethodTargets_CapturesMethodAndParameterMetadata()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.Add<SingleConcreteComponentMethodTarget>("Root 1");
            scene.Add<MixedMethodTarget>("Root 1");

            // Build graph and fetch component nodes
            TransformNode rootNode = new(scene.GetTransform("Root 1"));
            ComponentNode singleMethodComponentNode = GetComponentNode<SingleConcreteComponentMethodTarget>(rootNode);
            ComponentNode mixedMethodComponentNode = GetComponentNode<MixedMethodTarget>(rootNode);

            // Assert
            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    typeof(SingleConcreteComponentMethodTarget),
                    typeof(MixedMethodTarget)
                },
                rootNode.ComponentNodes.Select(node => node.Component.GetType()).ToArray()
            );

            MethodNode singleMethodNode = singleMethodComponentNode.MethodNodes.Single();
            Assert.That(singleMethodNode.Owner, Is.EqualTo(singleMethodComponentNode.Component));
            Assert.That(singleMethodNode.DeclaringType, Is.EqualTo(typeof(SingleConcreteComponentMethodTarget)));
            Assert.That(singleMethodNode.QualifyingName, Is.EqualTo("Inject"));
            Assert.That(singleMethodNode.InjectId, Is.Null);
            Assert.That(singleMethodNode.SuppressMissingErrors, Is.False);
            Assert.That(singleMethodNode.MethodName, Is.EqualTo("Inject"));
            Assert.That(singleMethodNode.DisplayPath, Is.EqualTo("Root 1/SingleConcreteComponentMethodTarget/Inject"));
            Assert.That(singleMethodNode.ShortPath, Is.EqualTo("SingleConcreteComponentMethodTarget.Inject"));

            MethodParameterNode singleParameterNode = singleMethodNode.ParameterNodes.Single();
            Assert.That(singleParameterNode.ParameterName, Is.EqualTo("dependency"));
            Assert.That(singleParameterNode.ParameterType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(singleParameterNode.RequestedType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(singleParameterNode.TypeShape, Is.EqualTo(TypeShape.Single));
            Assert.That(singleParameterNode.IsCollection, Is.False);
            Assert.That(singleParameterNode.IsInterface, Is.False);

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    "InjectSingles",
                    "InjectCollections",
                    "InjectMixed"
                },
                mixedMethodComponentNode.MethodNodes.Select(node => node.MethodName).ToArray()
            );

            MethodNode singlesMethodNode = GetMethodNode(mixedMethodComponentNode, "MixedMethodTarget.InjectSingles");

            CollectionAssert.AreEqual
            (
                new[]
                {
                    "singleConcreteDependency",
                    "singleInterfaceDependency"
                },
                singlesMethodNode.ParameterNodes.Select(node => node.ParameterName).ToArray()
            );

            MethodParameterNode singleConcreteParameterNode = singlesMethodNode.ParameterNodes.First();
            Assert.That(singleConcreteParameterNode.ParameterType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(singleConcreteParameterNode.RequestedType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(singleConcreteParameterNode.TypeShape, Is.EqualTo(TypeShape.Single));
            Assert.That(singleConcreteParameterNode.IsCollection, Is.False);
            Assert.That(singleConcreteParameterNode.IsInterface, Is.False);

            MethodParameterNode singleInterfaceParameterNode = singlesMethodNode.ParameterNodes.Last();
            Assert.That(singleInterfaceParameterNode.ParameterType, Is.EqualTo(typeof(IDependency)));
            Assert.That(singleInterfaceParameterNode.RequestedType, Is.EqualTo(typeof(IDependency)));
            Assert.That(singleInterfaceParameterNode.TypeShape, Is.EqualTo(TypeShape.Single));
            Assert.That(singleInterfaceParameterNode.IsCollection, Is.False);
            Assert.That(singleInterfaceParameterNode.IsInterface, Is.True);

            MethodNode collectionsMethodNode = GetMethodNode(mixedMethodComponentNode, "MixedMethodTarget.InjectCollections");

            CollectionAssert.AreEqual
            (
                new[]
                {
                    "concreteArray",
                    "interfaceList"
                },
                collectionsMethodNode.ParameterNodes.Select(node => node.ParameterName).ToArray()
            );

            MethodParameterNode concreteArrayParameterNode = collectionsMethodNode.ParameterNodes.First();
            Assert.That(concreteArrayParameterNode.ParameterType, Is.EqualTo(typeof(ComponentDependency[])));
            Assert.That(concreteArrayParameterNode.RequestedType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(concreteArrayParameterNode.TypeShape, Is.EqualTo(TypeShape.Array));
            Assert.That(concreteArrayParameterNode.IsCollection, Is.True);
            Assert.That(concreteArrayParameterNode.IsInterface, Is.False);

            MethodParameterNode interfaceListParameterNode = collectionsMethodNode.ParameterNodes.Last();
            Assert.That(interfaceListParameterNode.ParameterType, Is.EqualTo(typeof(List<IDependency>)));
            Assert.That(interfaceListParameterNode.RequestedType, Is.EqualTo(typeof(IDependency)));
            Assert.That(interfaceListParameterNode.TypeShape, Is.EqualTo(TypeShape.List));
            Assert.That(interfaceListParameterNode.IsCollection, Is.True);
            Assert.That(interfaceListParameterNode.IsInterface, Is.True);

            MethodNode mixedMethodNode = GetMethodNode(mixedMethodComponentNode, "MixedMethodTarget.InjectMixed");

            CollectionAssert.AreEqual
            (
                new[]
                {
                    "mixedConcreteDependency",
                    "mixedConcreteList",
                    "mixedInterfaceDependency"
                },
                mixedMethodNode.ParameterNodes.Select(node => node.ParameterName).ToArray()
            );
        }

        [Test]
        public void ComponentNode_GivenNestedSerializableMembers_CapturesNestedOwnerPathAndInjectMetadata()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            GraphMetadataTarget target = scene.Add<GraphMetadataTarget>("Root 1");

            // Build graph and fetch component node
            TransformNode rootNode = new(scene.GetTransform("Root 1"));
            ComponentNode componentNode = GetComponentNode<GraphMetadataTarget>(rootNode);
            FieldNode topLevelFieldNode = GetFieldNode(componentNode, "GraphMetadataTarget.fieldDependency");
            FieldNode topLevelPropertyNode = GetFieldNode(componentNode, "GraphMetadataTarget.PropertyDependencies");
            FieldNode nestedFieldNode = GetFieldNode(componentNode, "GraphMetadataNested.nestedFieldDependency");
            FieldNode nestedPropertyNode = GetFieldNode(componentNode, "GraphMetadataNested.NestedPropertyDependency");
            FieldNode deepFieldNode = GetFieldNode(componentNode, "GraphMetadataNestedChild.deepFieldDependency");
            MethodNode topLevelMethodNode = GetMethodNode(componentNode, "GraphMetadataTarget.InjectTopLevel");
            MethodNode nestedMethodNode = GetMethodNode(componentNode, "GraphMetadataNested.InjectNested");

            // Assert
            Assert.That(target, Is.Not.Null);
            Assert.That(target.nested, Is.Not.Null);
            Assert.That(target.nested.deepNested, Is.Not.Null);
            Assert.That(componentNode.FieldNodes.Count, Is.EqualTo(5));
            Assert.That(componentNode.MethodNodes.Count, Is.EqualTo(2));

            Assert.That(topLevelFieldNode.Owner, Is.EqualTo(target));
            Assert.That(topLevelFieldNode.DeclaringType, Is.EqualTo(typeof(GraphMetadataTarget)));
            Assert.That(topLevelFieldNode.InjectId, Is.EqualTo("field-id"));
            Assert.That(topLevelFieldNode.SuppressMissingErrors, Is.True);
            Assert.That(topLevelFieldNode.DisplayPath, Is.EqualTo("Root 1/GraphMetadataTarget/fieldDependency"));
            Assert.That(topLevelFieldNode.ShortPath, Is.EqualTo("GraphMetadataTarget.fieldDependency"));

            Assert.That(topLevelPropertyNode.Owner, Is.EqualTo(target));
            Assert.That(topLevelPropertyNode.DeclaringType, Is.EqualTo(typeof(GraphMetadataTarget)));
            Assert.That(topLevelPropertyNode.InjectId, Is.EqualTo("property-id"));
            Assert.That(topLevelPropertyNode.SuppressMissingErrors, Is.True);
            Assert.That(topLevelPropertyNode.TypeShape, Is.EqualTo(TypeShape.List));
            Assert.That(topLevelPropertyNode.IsCollection, Is.True);
            Assert.That(topLevelPropertyNode.IsPropertyBackingField, Is.True);
            Assert.That(topLevelPropertyNode.DisplayPath, Is.EqualTo("Root 1/GraphMetadataTarget/PropertyDependencies"));
            Assert.That(topLevelPropertyNode.ShortPath, Is.EqualTo("GraphMetadataTarget.PropertyDependencies"));

            Assert.That(nestedFieldNode.Owner, Is.EqualTo(target.nested));
            Assert.That(nestedFieldNode.DeclaringType, Is.EqualTo(typeof(NestedChildTarget)));
            Assert.That(nestedFieldNode.InjectId, Is.Null);
            Assert.That(nestedFieldNode.SuppressMissingErrors, Is.False);
            Assert.That(nestedFieldNode.DisplayPath, Is.EqualTo("Root 1/GraphMetadataTarget/nested.nestedFieldDependency"));
            Assert.That(nestedFieldNode.ShortPath, Is.EqualTo("GraphMetadataNested.nestedFieldDependency"));

            Assert.That(nestedPropertyNode.Owner, Is.EqualTo(target.nested));
            Assert.That(nestedPropertyNode.DeclaringType, Is.EqualTo(typeof(NestedChildTarget)));
            Assert.That(nestedPropertyNode.InjectId, Is.EqualTo("nested-property-id"));
            Assert.That(nestedPropertyNode.SuppressMissingErrors, Is.True);
            Assert.That(nestedPropertyNode.IsPropertyBackingField, Is.True);
            Assert.That(nestedPropertyNode.DisplayPath, Is.EqualTo("Root 1/GraphMetadataTarget/nested.NestedPropertyDependency"));
            Assert.That(nestedPropertyNode.ShortPath, Is.EqualTo("GraphMetadataNested.NestedPropertyDependency"));

            Assert.That(deepFieldNode.Owner, Is.EqualTo(target.nested.deepNested));
            Assert.That(deepFieldNode.DeclaringType, Is.EqualTo(typeof(GraphMetadataNestedChild)));
            Assert.That(deepFieldNode.InjectId, Is.EqualTo("deep-field-id"));
            Assert.That(deepFieldNode.SuppressMissingErrors, Is.True);
            Assert.That(deepFieldNode.DisplayPath, Is.EqualTo("Root 1/GraphMetadataTarget/nested.deepNested.deepFieldDependency"));
            Assert.That(deepFieldNode.ShortPath, Is.EqualTo("GraphMetadataNestedChild.deepFieldDependency"));

            CollectionAssert.IsEmpty(componentNode.FieldNodes.Where(node => node.DisplayPath.Contains("nullNested")).ToArray());

            Assert.That(topLevelMethodNode.Owner, Is.EqualTo(target));
            Assert.That(topLevelMethodNode.DeclaringType, Is.EqualTo(typeof(GraphMetadataTarget)));
            Assert.That(topLevelMethodNode.InjectId, Is.EqualTo("method-id"));
            Assert.That(topLevelMethodNode.SuppressMissingErrors, Is.True);
            Assert.That(topLevelMethodNode.DisplayPath, Is.EqualTo("Root 1/GraphMetadataTarget/InjectTopLevel"));
            Assert.That(topLevelMethodNode.ShortPath, Is.EqualTo("GraphMetadataTarget.InjectTopLevel"));

            CollectionAssert.AreEqual
            (
                new[]
                {
                    "singleDependency",
                    "interfaceDependency",
                    "arrayDependencies",
                    "listDependencies"
                },
                topLevelMethodNode.ParameterNodes.Select(node => node.ParameterName).ToArray()
            );

            Assert.That(nestedMethodNode.Owner, Is.EqualTo(target.nested));
            Assert.That(nestedMethodNode.DeclaringType, Is.EqualTo(typeof(NestedChildTarget)));
            Assert.That(nestedMethodNode.InjectId, Is.EqualTo("nested-method-id"));
            Assert.That(nestedMethodNode.SuppressMissingErrors, Is.False);
            Assert.That(nestedMethodNode.DisplayPath, Is.EqualTo("Root 1/GraphMetadataTarget/nested.InjectNested"));
            Assert.That(nestedMethodNode.ShortPath, Is.EqualTo("GraphMetadataNested.InjectNested"));

            CollectionAssert.AreEqual
            (
                new[]
                {
                    "nestedAssetDependency",
                    "nestedComponentDependencies",
                    "nestedInterfaceDependency"
                },
                nestedMethodNode.ParameterNodes.Select(node => node.ParameterName).ToArray()
            );
        }

        [Test]
        public void FieldNode_GivenFieldCollectionTargets_CapturesArrayAndListMetadataForFields()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.Add<MultiConcreteComponentTarget>("Root 1");
            scene.Add<MultiInterfaceTarget>("Root 1");

            // Build graph and fetch component nodes
            TransformNode rootNode = new(scene.GetTransform("Root 1"));
            ComponentNode concreteComponentNode = GetComponentNode<MultiConcreteComponentTarget>(rootNode);
            ComponentNode interfaceComponentNode = GetComponentNode<MultiInterfaceTarget>(rootNode);

            // Assert
            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    "array",
                    "list"
                },
                concreteComponentNode.FieldNodes.Select(node => node.QualifyingName).ToArray()
            );

            FieldNode concreteArrayFieldNode = GetFieldNode(concreteComponentNode, "MultiConcreteComponentTarget.array");
            Assert.That(concreteArrayFieldNode.FieldType, Is.EqualTo(typeof(ComponentDependency[])));
            Assert.That(concreteArrayFieldNode.RequestedType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(concreteArrayFieldNode.TypeShape, Is.EqualTo(TypeShape.Array));
            Assert.That(concreteArrayFieldNode.IsCollection, Is.True);
            Assert.That(concreteArrayFieldNode.IsInterface, Is.False);
            Assert.That(concreteArrayFieldNode.IsPropertyBackingField, Is.False);
            Assert.That(concreteArrayFieldNode.DisplayPath, Is.EqualTo("Root 1/MultiConcreteComponentTarget/array"));

            FieldNode concreteListFieldNode = GetFieldNode(concreteComponentNode, "MultiConcreteComponentTarget.list");
            Assert.That(concreteListFieldNode.FieldType, Is.EqualTo(typeof(List<ComponentDependency>)));
            Assert.That(concreteListFieldNode.RequestedType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(concreteListFieldNode.TypeShape, Is.EqualTo(TypeShape.List));
            Assert.That(concreteListFieldNode.IsCollection, Is.True);
            Assert.That(concreteListFieldNode.IsInterface, Is.False);
            Assert.That(concreteListFieldNode.IsPropertyBackingField, Is.False);
            Assert.That(concreteListFieldNode.DisplayPath, Is.EqualTo("Root 1/MultiConcreteComponentTarget/list"));

            CollectionAssert.AreEquivalent
            (
                new[]
                {
                    "array",
                    "list"
                },
                interfaceComponentNode.FieldNodes.Select(node => node.QualifyingName).ToArray()
            );

            FieldNode interfaceArrayFieldNode = GetFieldNode(interfaceComponentNode, "MultiInterfaceTarget.array");
            Assert.That(interfaceArrayFieldNode.FieldType, Is.EqualTo(typeof(IDependency[])));
            Assert.That(interfaceArrayFieldNode.RequestedType, Is.EqualTo(typeof(IDependency)));
            Assert.That(interfaceArrayFieldNode.TypeShape, Is.EqualTo(TypeShape.Array));
            Assert.That(interfaceArrayFieldNode.IsCollection, Is.True);
            Assert.That(interfaceArrayFieldNode.IsInterface, Is.True);
            Assert.That(interfaceArrayFieldNode.IsPropertyBackingField, Is.False);
            Assert.That(interfaceArrayFieldNode.DisplayPath, Is.EqualTo("Root 1/MultiInterfaceTarget/array"));

            FieldNode interfaceListFieldNode = GetFieldNode(interfaceComponentNode, "MultiInterfaceTarget.list");
            Assert.That(interfaceListFieldNode.FieldType, Is.EqualTo(typeof(List<IDependency>)));
            Assert.That(interfaceListFieldNode.RequestedType, Is.EqualTo(typeof(IDependency)));
            Assert.That(interfaceListFieldNode.TypeShape, Is.EqualTo(TypeShape.List));
            Assert.That(interfaceListFieldNode.IsCollection, Is.True);
            Assert.That(interfaceListFieldNode.IsInterface, Is.True);
            Assert.That(interfaceListFieldNode.IsPropertyBackingField, Is.False);
            Assert.That(interfaceListFieldNode.DisplayPath, Is.EqualTo("Root 1/MultiInterfaceTarget/list"));
        }

        [Test]
        public void FieldNode_GivenQualifiedAndRenamedTargets_CapturesInjectIdAndMemberNames()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.Add<SingleConcreteComponentTargetWithID>("Root 1");
            scene.Add<SingleConcreteComponentTargetWithDifferentMemberName>("Root 1");
            scene.Add<SingleConcreteAssetTargetWithID>("Root 1");
            scene.Add<SingleConcreteAssetTargetWithDifferentMemberName>("Root 1");

            // Build graph and fetch component nodes
            TransformNode rootNode = new(scene.GetTransform("Root 1"));
            ComponentNode componentIdNode = GetComponentNode<SingleConcreteComponentTargetWithID>(rootNode);
            ComponentNode componentOtherMemberNode = GetComponentNode<SingleConcreteComponentTargetWithDifferentMemberName>(rootNode);
            ComponentNode assetIdNode = GetComponentNode<SingleConcreteAssetTargetWithID>(rootNode);
            ComponentNode assetOtherMemberNode = GetComponentNode<SingleConcreteAssetTargetWithDifferentMemberName>(rootNode);

            // Assert
            FieldNode componentIdFieldNode = componentIdNode.FieldNodes.Single();
            Assert.That(componentIdFieldNode.InjectId, Is.EqualTo("qualified"));
            Assert.That(componentIdFieldNode.QualifyingName, Is.EqualTo("dependency"));
            Assert.That(componentIdFieldNode.ShortPath, Is.EqualTo("SingleConcreteComponentTargetWithID.dependency"));
            Assert.That(componentIdFieldNode.DisplayPath, Is.EqualTo("Root 1/SingleConcreteComponentTargetWithID/dependency"));

            FieldNode componentOtherMemberFieldNode = componentOtherMemberNode.FieldNodes.Single();
            Assert.That(componentOtherMemberFieldNode.InjectId, Is.Null);
            Assert.That(componentOtherMemberFieldNode.QualifyingName, Is.EqualTo("otherDependency"));
            Assert.That(componentOtherMemberFieldNode.ShortPath, Is.EqualTo("SingleConcreteComponentTargetWithDifferentMemberName.otherDependency"));
            Assert.That(componentOtherMemberFieldNode.DisplayPath, Is.EqualTo("Root 1/SingleConcreteComponentTargetWithDifferentMemberName/otherDependency"));

            FieldNode assetIdFieldNode = assetIdNode.FieldNodes.Single();
            Assert.That(assetIdFieldNode.InjectId, Is.EqualTo("qualified"));
            Assert.That(assetIdFieldNode.QualifyingName, Is.EqualTo("dependency"));
            Assert.That(assetIdFieldNode.RequestedType, Is.EqualTo(typeof(AssetDependency)));
            Assert.That(assetIdFieldNode.TypeShape, Is.EqualTo(TypeShape.Single));
            Assert.That(assetIdFieldNode.ShortPath, Is.EqualTo("SingleConcreteAssetTargetWithID.dependency"));

            FieldNode assetOtherMemberFieldNode = assetOtherMemberNode.FieldNodes.Single();
            Assert.That(assetOtherMemberFieldNode.InjectId, Is.Null);
            Assert.That(assetOtherMemberFieldNode.QualifyingName, Is.EqualTo("otherDependency"));
            Assert.That(assetOtherMemberFieldNode.RequestedType, Is.EqualTo(typeof(AssetDependency)));
            Assert.That(assetOtherMemberFieldNode.ShortPath, Is.EqualTo("SingleConcreteAssetTargetWithDifferentMemberName.otherDependency"));
        }

        [Test]
        public void MethodNode_GivenNestedSerializableMethod_CapturesNestedParameterMetadata()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.Add<GraphMetadataTarget>("Root 1");

            // Build graph and fetch method node
            TransformNode rootNode = new(scene.GetTransform("Root 1"));
            ComponentNode componentNode = GetComponentNode<GraphMetadataTarget>(rootNode);
            MethodNode nestedMethodNode = GetMethodNode(componentNode, "GraphMetadataNested.InjectNested");

            // Assert
            MethodParameterNode nestedAssetParameterNode = nestedMethodNode.ParameterNodes.ElementAt(0);
            Assert.That(nestedAssetParameterNode.ParameterType, Is.EqualTo(typeof(AssetDependency)));
            Assert.That(nestedAssetParameterNode.RequestedType, Is.EqualTo(typeof(AssetDependency)));
            Assert.That(nestedAssetParameterNode.TypeShape, Is.EqualTo(TypeShape.Single));
            Assert.That(nestedAssetParameterNode.IsCollection, Is.False);
            Assert.That(nestedAssetParameterNode.IsInterface, Is.False);

            MethodParameterNode nestedComponentListParameterNode = nestedMethodNode.ParameterNodes.ElementAt(1);
            Assert.That(nestedComponentListParameterNode.ParameterType, Is.EqualTo(typeof(List<ComponentDependency>)));
            Assert.That(nestedComponentListParameterNode.RequestedType, Is.EqualTo(typeof(ComponentDependency)));
            Assert.That(nestedComponentListParameterNode.TypeShape, Is.EqualTo(TypeShape.List));
            Assert.That(nestedComponentListParameterNode.IsCollection, Is.True);
            Assert.That(nestedComponentListParameterNode.IsInterface, Is.False);

            MethodParameterNode nestedInterfaceParameterNode = nestedMethodNode.ParameterNodes.ElementAt(2);
            Assert.That(nestedInterfaceParameterNode.ParameterType, Is.EqualTo(typeof(IDependency)));
            Assert.That(nestedInterfaceParameterNode.RequestedType, Is.EqualTo(typeof(IDependency)));
            Assert.That(nestedInterfaceParameterNode.TypeShape, Is.EqualTo(TypeShape.Single));
            Assert.That(nestedInterfaceParameterNode.IsCollection, Is.False);
            Assert.That(nestedInterfaceParameterNode.IsInterface, Is.True);
        }

        [Test]
        public void TransformNode_GivenOnlyNonInjectableComponents_ExcludesNonInjectableComponentNodes()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Build graph
            TransformNode rootNode = new(scene.GetTransform("Root 1"));

            // Assert
            Assert.That(dependency, Is.Not.Null);
            CollectionAssert.IsEmpty(rootNode.ComponentNodes.ToArray());
        }

        private static ComponentNode GetComponentNode<T>(TransformNode transformNode) where T : Component
        {
            ComponentNode componentNode = transformNode.ComponentNodes
                .SingleOrDefault(node => node.Component is T);

            Assert.That(componentNode, Is.Not.Null);
            return componentNode;
        }

        private static FieldNode GetFieldNode(
            ComponentNode componentNode,
            string shortPath)
        {
            FieldNode fieldNode = componentNode.FieldNodes
                .SingleOrDefault(node => node.ShortPath == shortPath);

            Assert.That(fieldNode, Is.Not.Null);
            return fieldNode;
        }

        private static MethodNode GetMethodNode(
            ComponentNode componentNode,
            string shortPath)
        {
            MethodNode methodNode = componentNode.MethodNodes
                .SingleOrDefault(node => node.ShortPath == shortPath);

            Assert.That(methodNode, Is.Not.Null);
            return methodNode;
        }
    }
}