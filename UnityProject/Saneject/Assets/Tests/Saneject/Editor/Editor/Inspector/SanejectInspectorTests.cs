using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Inspectors;
using Plugins.Saneject.Editor.Inspectors.Models;
using Plugins.Saneject.Runtime.Settings;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Inspector
{
    public class SanejectInspectorTests
    {
        private bool originalShowInjectedFieldsProperties;

        [SetUp]
        public void SetUp()
        {
            originalShowInjectedFieldsProperties = UserSettings.ShowInjectedFieldsProperties;
        }

        [TearDown]
        public void TearDown()
        {
            UserSettings.ShowInjectedFieldsProperties = originalShowInjectedFieldsProperties;
        }

        [Test]
        public void OnInspectorGUI_GivenScriptableObjectModel_DoesNotModifyHiddenInjectedProperty()
        {
            // Set up objects
            InspectorScriptableTarget target = ScriptableObject.CreateInstance<InspectorScriptableTarget>();
            AssetDependency dependency = ScriptableObject.CreateInstance<AssetDependency>();
            target.dependency = dependency;
            SerializedObject serializedObject = new(target);
            ComponentModel componentModel = new(target, serializedObject);
            UserSettings.ShowInjectedFieldsProperties = false;

            try
            {
                // Draw inspector
                Assert.That(() => SanejectInspector.OnInspectorGUI(componentModel), Throws.Nothing);

                // Assert
                serializedObject.Update();
                SerializedProperty property = serializedObject.FindProperty("dependency");

                Assert.That(property, Is.Not.Null);
                Assert.That(property.objectReferenceValue, Is.EqualTo(dependency));
            }
            finally
            {
                Object.DestroyImmediate(dependency);
                Object.DestroyImmediate(target);
            }
        }

        [Test]
        public void DrawMonoBehaviourScriptField_GivenNonMonoBehaviour_DoesNotThrow()
        {
            // Set up object
            AssetDependency target = ScriptableObject.CreateInstance<AssetDependency>();

            try
            {
                // Draw script field
                Assert.That(() => SanejectInspector.DrawMonoBehaviourScriptField(target), Throws.Nothing);
            }
            finally
            {
                Object.DestroyImmediate(target);
            }
        }

        [Test]
        public void DrawProperty_GivenHiddenInjectedProperty_DoesNotModifySerializedValue()
        {
            // Set up objects
            GameObject root = new("Root");

            try
            {
                SingleConcreteComponentTarget target = root.AddComponent<SingleConcreteComponentTarget>();
                ComponentDependency dependency = root.AddComponent<ComponentDependency>();
                SerializedObject serializedObject = new(target);
                FieldInfo field = typeof(SingleConcreteComponentTarget).GetField("dependency", BindingFlags.Public | BindingFlags.Instance);
                Assert.That(field, Is.Not.Null);
                PropertyModel propertyModel = new(serializedObject, field);
                target.dependency = dependency;
                UserSettings.ShowInjectedFieldsProperties = false;

                // Draw property
                Assert.That(() => SanejectInspector.DrawProperty(propertyModel), Throws.Nothing);

                // Assert
                serializedObject.Update();
                Assert.That(propertyModel.SerializedProperty, Is.Not.Null);
                Assert.That(propertyModel.SerializedProperty.objectReferenceValue, Is.EqualTo(dependency));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void ValidateProperty_GivenSerializedInterfaceGameObjectWithMatchingComponent_ConvertsToComponent()
        {
            // Set up objects
            GameObject root = new("Root");

            try
            {
                SingleInterfaceTarget target = root.AddComponent<SingleInterfaceTarget>();
                GameObject dependencyObject = new("Dependency");
                dependencyObject.transform.SetParent(root.transform);
                ComponentDependency dependency = dependencyObject.AddComponent<ComponentDependency>();
                SerializedObject serializedObject = new(target);
                FieldInfo field = typeof(SingleInterfaceTarget).GetField("dependency", BindingFlags.Public | BindingFlags.Instance);
                Assert.That(field, Is.Not.Null);
                PropertyModel propertyModel = new(serializedObject, field);

                // Assign game object
                propertyModel.SerializedProperty.objectReferenceValue = dependencyObject;

                // Validate
                SanejectInspector.ValidateProperty(propertyModel);

                // Assert
                Assert.That(propertyModel.SerializedProperty, Is.Not.Null);
                Assert.That(propertyModel.SerializedProperty.objectReferenceValue, Is.EqualTo(dependency));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void ValidateProperty_GivenSerializedInterfaceGameObjectWithoutMatchingComponent_ClearsValue()
        {
            // Expect error log for invalid interface assignment
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: 'GameObject' does not implement IDependency"));

            // Set up objects
            GameObject root = new("Root");

            try
            {
                SingleInterfaceTarget target = root.AddComponent<SingleInterfaceTarget>();
                GameObject invalidObject = new("Invalid");
                invalidObject.transform.SetParent(root.transform);
                SerializedObject serializedObject = new(target);
                FieldInfo field = typeof(SingleInterfaceTarget).GetField("dependency", BindingFlags.Public | BindingFlags.Instance);
                Assert.That(field, Is.Not.Null);
                PropertyModel propertyModel = new(serializedObject, field);

                // Assign game object
                propertyModel.SerializedProperty.objectReferenceValue = invalidObject;

                // Validate
                SanejectInspector.ValidateProperty(propertyModel);

                // Assert
                Assert.That(propertyModel.SerializedProperty, Is.Not.Null);
                Assert.That(propertyModel.SerializedProperty.objectReferenceValue, Is.Null);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void ValidateProperty_GivenSerializedInterfaceCollection_ValidatesEachElement()
        {
            // Expect error log for invalid interface assignment
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: 'GameObject' does not implement IDependency"));

            // Set up objects
            GameObject root = new("Root");

            try
            {
                MultiInterfaceTarget target = root.AddComponent<MultiInterfaceTarget>();
                GameObject validObject = new("Valid");
                validObject.transform.SetParent(root.transform);
                ComponentDependency validDependency = validObject.AddComponent<ComponentDependency>();
                GameObject invalidObject = new("Invalid");
                invalidObject.transform.SetParent(root.transform);
                SerializedObject serializedObject = new(target);
                FieldInfo field = typeof(MultiInterfaceTarget).GetField("array", BindingFlags.Public | BindingFlags.Instance);
                Assert.That(field, Is.Not.Null);
                PropertyModel propertyModel = new(serializedObject, field);

                // Assign collection values
                propertyModel.SerializedProperty.arraySize = 2;
                propertyModel.SerializedProperty.GetArrayElementAtIndex(0).objectReferenceValue = validObject;
                propertyModel.SerializedProperty.GetArrayElementAtIndex(1).objectReferenceValue = invalidObject;

                // Validate
                SanejectInspector.ValidateProperty(propertyModel);

                // Assert
                Assert.That(propertyModel.SerializedProperty, Is.Not.Null);
                Assert.That(propertyModel.SerializedProperty.GetArrayElementAtIndex(0).objectReferenceValue, Is.EqualTo(validDependency));
                Assert.That(propertyModel.SerializedProperty.GetArrayElementAtIndex(1).objectReferenceValue, Is.Null);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void GetFieldInfosForInspector_GivenInheritedInspectableFields_ReturnsDrawableFieldsInOrder()
        {
            // Find field infos
            FieldInfo[] fieldInfos = typeof(InspectorFieldInfoTarget)
                .GetFieldInfosForInspector()
                .ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "basePublic",
                "baseSerialized",
                "baseInterface",
                "readOnlyDependency",
                "derivedPublic",
                "injectedDependency"
            }, fieldInfos.Select(x => x.Name).ToArray());
        }

        [Test]
        public void HasReadOnlyAttribute_GivenReadOnlyField_ReturnsTrue()
        {
            // Find field
            FieldInfo field = typeof(InspectorFieldInfoTarget).GetField("readOnlyDependency", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            Assert.That(field, Is.Not.Null);
            Assert.That(field.HasReadOnlyAttribute(), Is.True);
        }

        [Test]
        public void HasInjectAttribute_GivenInjectField_ReturnsTrue()
        {
            // Find field
            FieldInfo field = typeof(InspectorFieldInfoTarget).GetField("injectedDependency", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            Assert.That(field, Is.Not.Null);
            Assert.That(field.HasInjectAttribute(), Is.True);
        }

        [Test]
        public void IsSerializeInterface_GivenSerializeInterfaceField_ReturnsTrue()
        {
            // Find field
            FieldInfo field = typeof(InspectorFieldInfoBaseTarget).GetField
            (
                "baseInterface",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            // Assert
            Assert.That(field, Is.Not.Null);
            Assert.That(field.IsSerializeInterface(), Is.True);
        }
    }
}
