using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Inspectors.Models;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Inspector
{
    public class ComponentModelTests
    {
        [Test]
        public void ComponentModel_GivenInheritedInspectableFields_CapturesFieldsInInspectorOrder()
        {
            // Set up object
            GameObject root = new("Root");

            try
            {
                InspectorFieldInfoTarget target = root.AddComponent<InspectorFieldInfoTarget>();
                SerializedObject serializedObject = new(target);

                // Capture model
                ComponentModel componentModel = new(target, serializedObject);

                // Assert
                Assert.That(componentModel.Target, Is.EqualTo(target));
                Assert.That(componentModel.SerializedObject, Is.EqualTo(serializedObject));

                CollectionAssert.AreEqual(new[]
                {
                    "Base Public",
                    "Base Serialized",
                    "Base Interface (IDependency)",
                    "Read Only Dependency",
                    "Derived Public",
                    "Injected Dependency"
                }, componentModel.Properties.Select(x => x.DisplayName).ToArray());
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void ComponentModel_GivenFieldsAndProperties_CapturesMembersInDeclarationOrder()
        {
            // Set up object
            GameObject root = new("Root");

            try
            {
                NestedRootTarget target = root.AddComponent<NestedRootTarget>();
                SerializedObject serializedObject = new(target);

                // Capture model
                ComponentModel componentModel = new(target, serializedObject);

                // Assert
                Assert.That(componentModel.Target, Is.EqualTo(target));
                Assert.That(componentModel.SerializedObject, Is.EqualTo(serializedObject));

                CollectionAssert.AreEqual(new[]
                {
                    "Field Dependency",
                    "Property Dependencies",
                    "Nested",
                    "Null Nested"
                }, componentModel.Properties.Select(x => x.DisplayName).ToArray());

                Assert.That(componentModel.Properties[0].SerializedProperty, Is.Not.Null);
                Assert.That(componentModel.Properties[1].SerializedProperty, Is.Not.Null);
                Assert.That(componentModel.Properties[2].SerializedProperty, Is.Not.Null);
                Assert.That(componentModel.Properties[3].SerializedProperty, Is.Not.Null);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void ComponentModel_GivenSerializedInterfaceProperty_CapturesExpectedPropertyMetadata()
        {
            // Set up object
            GameObject root = new("Root");

            try
            {
                SingleInterfacePropertyTarget target = root.AddComponent<SingleInterfacePropertyTarget>();
                SerializedObject serializedObject = new(target);

                // Capture model
                ComponentModel componentModel = new(target, serializedObject);
                PropertyModel propertyModel = componentModel.Properties.Single();

                // Assert
                Assert.That(propertyModel.SerializedProperty, Is.Not.Null);
                Assert.That(propertyModel.SerializedProperty.propertyPath, Is.EqualTo("__Dependency"));
                Assert.That(propertyModel.DisplayName, Is.EqualTo("Dependency (IDependency)"));
                Assert.That(propertyModel.IsReadOnly, Is.True);
                Assert.That(propertyModel.IsSerializedInterface, Is.True);
                Assert.That(propertyModel.ExpectedType, Is.EqualTo(typeof(IDependency)));
                Assert.That(propertyModel.HasInjectAttribute, Is.True);
                Assert.That(propertyModel.IsCollection, Is.False);
                Assert.That(propertyModel.Children, Is.Empty);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
    }
}
