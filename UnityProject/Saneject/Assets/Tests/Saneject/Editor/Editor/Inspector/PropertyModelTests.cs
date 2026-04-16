using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Inspectors;
using Plugins.Saneject.Editor.Inspectors.Models;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Inspector
{
    public class PropertyModelTests
    {
        [Test]
        public void PropertyModel_GivenSerializedInterfaceProperty_CapturesExpectedMetadata()
        {
            // Set up object
            GameObject root = new("Root");

            try
            {
                SingleInterfacePropertyTarget target = root.AddComponent<SingleInterfacePropertyTarget>();
                SerializedObject serializedObject = new(target);
                FieldInfo field = typeof(SingleInterfacePropertyTarget)
                    .GetFieldInfosForInspector()
                    .Single();

                // Capture model
                Assert.That(field, Is.Not.Null);

                PropertyModel propertyModel = new(serializedObject, field);

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

        [Test]
        public void PropertyModel_GivenSerializedInterfaceCollection_CapturesExpectedMetadata()
        {
            // Set up object
            GameObject root = new("Root");

            try
            {
                MultiInterfaceTarget target = root.AddComponent<MultiInterfaceTarget>();
                SerializedObject serializedObject = new(target);
                FieldInfo field = typeof(MultiInterfaceTarget).GetField("array", BindingFlags.Public | BindingFlags.Instance);

                // Capture model
                Assert.That(field, Is.Not.Null);

                PropertyModel propertyModel = new(serializedObject, field);

                // Assert
                Assert.That(propertyModel.SerializedProperty, Is.Not.Null);
                Assert.That(propertyModel.SerializedProperty.propertyPath, Is.EqualTo("__array"));
                Assert.That(propertyModel.DisplayName, Is.EqualTo("Array (IDependency)"));
                Assert.That(propertyModel.IsReadOnly, Is.True);
                Assert.That(propertyModel.IsSerializedInterface, Is.True);
                Assert.That(propertyModel.ExpectedType, Is.EqualTo(typeof(IDependency)));
                Assert.That(propertyModel.HasInjectAttribute, Is.True);
                Assert.That(propertyModel.IsCollection, Is.True);
                Assert.That(propertyModel.Children, Is.Empty);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void PropertyModel_GivenNestedSerializableField_CapturesChildrenInDeclarationOrder()
        {
            // Set up object
            GameObject root = new("Root");

            try
            {
                NestedRootTarget target = root.AddComponent<NestedRootTarget>();
                SerializedObject serializedObject = new(target);
                FieldInfo field = typeof(NestedRootTarget).GetField("nested", BindingFlags.Public | BindingFlags.Instance);

                // Capture model
                Assert.That(field, Is.Not.Null);

                PropertyModel propertyModel = new(serializedObject, field);

                // Assert
                Assert.That(propertyModel.SerializedProperty, Is.Not.Null);
                Assert.That(propertyModel.DisplayName, Is.EqualTo("Nested"));
                Assert.That(propertyModel.IsReadOnly, Is.False);
                Assert.That(propertyModel.IsSerializedInterface, Is.False);
                Assert.That(propertyModel.ExpectedType, Is.EqualTo(typeof(NestedChildTarget)));
                Assert.That(propertyModel.HasInjectAttribute, Is.False);
                Assert.That(propertyModel.IsCollection, Is.False);

                CollectionAssert.AreEqual(new[]
                {
                    "Nested Field Dependency",
                    "Nested Property Dependency",
                    "Deep Nested"
                }, propertyModel.Children.Select(x => x.DisplayName).ToArray());

                Assert.That(propertyModel.Children[0].IsReadOnly, Is.True);
                Assert.That(propertyModel.Children[0].ExpectedType, Is.EqualTo(typeof(AssetDependency)));
                Assert.That(propertyModel.Children[1].IsReadOnly, Is.True);
                Assert.That(propertyModel.Children[1].ExpectedType, Is.EqualTo(typeof(AssetDependency)));
                Assert.That(propertyModel.Children[2].IsReadOnly, Is.False);
                Assert.That(propertyModel.Children[2].ExpectedType, Is.EqualTo(typeof(NestedDeepTarget)));

                CollectionAssert.AreEqual(new[]
                {
                    "Deep Field Dependency"
                }, propertyModel.Children[2].Children.Select(x => x.DisplayName).ToArray());
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
    }
}
