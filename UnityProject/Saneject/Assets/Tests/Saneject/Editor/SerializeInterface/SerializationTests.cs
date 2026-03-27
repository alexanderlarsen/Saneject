using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.SerializeInterface
{
    public class SerializationTests
    {
        [Test]
        public void OnBeforeSerialize_GivenInterfaceFields_SyncsToBackingFields()
        {
            // Set up components
            GameObject root = new("Root");

            try
            {
                GameObject singleTargetObject = new("Single Target");
                singleTargetObject.transform.SetParent(root.transform);
                SingleInterfaceTarget singleTarget = singleTargetObject.AddComponent<SingleInterfaceTarget>();

                GameObject multiTargetObject = new("Multi Target");
                multiTargetObject.transform.SetParent(root.transform);
                MultiInterfaceTarget multiTarget = multiTargetObject.AddComponent<MultiInterfaceTarget>();

                GameObject singleDependencyObject = new("Single Dependency");
                singleDependencyObject.transform.SetParent(root.transform);
                ComponentDependency singleDependency = singleDependencyObject.AddComponent<ComponentDependency>();

                GameObject firstDependencyObject = new("First Dependency");
                firstDependencyObject.transform.SetParent(root.transform);
                ComponentDependency firstDependency = firstDependencyObject.AddComponent<ComponentDependency>();

                GameObject secondDependencyObject = new("Second Dependency");
                secondDependencyObject.transform.SetParent(root.transform);
                ComponentDependency secondDependency = secondDependencyObject.AddComponent<ComponentDependency>();

                singleTarget.dependency = singleDependency;

                multiTarget.array = new IDependency[]
                {
                    firstDependency,
                    secondDependency
                };

                multiTarget.list = new List<IDependency>
                {
                    firstDependency,
                    secondDependency
                };

                // Serialize
                singleTarget.OnBeforeSerialize();
                multiTarget.OnBeforeSerialize();

                // Find backing fields
                FieldInfo singleBackingFieldInfo = typeof(SingleInterfaceTarget).GetField("__dependency", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo arrayBackingFieldInfo = typeof(MultiInterfaceTarget).GetField("__array", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo listBackingFieldInfo = typeof(MultiInterfaceTarget).GetField("__list", BindingFlags.NonPublic | BindingFlags.Instance);

                Assert.That(singleBackingFieldInfo, Is.Not.Null);
                Assert.That(arrayBackingFieldInfo, Is.Not.Null);
                Assert.That(listBackingFieldInfo, Is.Not.Null);

                Object singleBackingField = singleBackingFieldInfo.GetValue(singleTarget) as Object;
                Object[] arrayBackingField = arrayBackingFieldInfo.GetValue(multiTarget) as Object[];
                List<Object> listBackingField = listBackingFieldInfo.GetValue(multiTarget) as List<Object>;

                // Assert
                Assert.That(singleDependency, Is.Not.Null);
                Assert.That(firstDependency, Is.Not.Null);
                Assert.That(secondDependency, Is.Not.Null);
                Assert.That(singleBackingField, Is.EqualTo(singleDependency));

                CollectionAssert.AreEqual(new Object[]
                {
                    firstDependency,
                    secondDependency
                }, arrayBackingField);

                CollectionAssert.AreEqual(new Object[]
                {
                    firstDependency,
                    secondDependency
                }, listBackingField);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void OnAfterDeserialize_GivenInterfaceFields_SyncsToInterfaceFields()
        {
            // Set up components
            GameObject root = new("Root");

            try
            {
                GameObject singleTargetObject = new("Single Target");
                singleTargetObject.transform.SetParent(root.transform);
                SingleInterfaceTarget singleTarget = singleTargetObject.AddComponent<SingleInterfaceTarget>();

                GameObject multiTargetObject = new("Multi Target");
                multiTargetObject.transform.SetParent(root.transform);
                MultiInterfaceTarget multiTarget = multiTargetObject.AddComponent<MultiInterfaceTarget>();

                GameObject singleDependencyObject = new("Single Dependency");
                singleDependencyObject.transform.SetParent(root.transform);
                ComponentDependency singleDependency = singleDependencyObject.AddComponent<ComponentDependency>();

                GameObject firstDependencyObject = new("First Dependency");
                firstDependencyObject.transform.SetParent(root.transform);
                ComponentDependency firstDependency = firstDependencyObject.AddComponent<ComponentDependency>();

                GameObject secondDependencyObject = new("Second Dependency");
                secondDependencyObject.transform.SetParent(root.transform);
                ComponentDependency secondDependency = secondDependencyObject.AddComponent<ComponentDependency>();

                FieldInfo singleBackingFieldInfo = typeof(SingleInterfaceTarget).GetField("__dependency", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo arrayBackingFieldInfo = typeof(MultiInterfaceTarget).GetField("__array", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo listBackingFieldInfo = typeof(MultiInterfaceTarget).GetField("__list", BindingFlags.NonPublic | BindingFlags.Instance);

                Assert.That(singleBackingFieldInfo, Is.Not.Null);
                Assert.That(arrayBackingFieldInfo, Is.Not.Null);
                Assert.That(listBackingFieldInfo, Is.Not.Null);

                singleBackingFieldInfo.SetValue(singleTarget, singleDependency);

                arrayBackingFieldInfo.SetValue(multiTarget, new Object[]
                {
                    firstDependency,
                    secondDependency
                });

                listBackingFieldInfo.SetValue(multiTarget, new List<Object>
                {
                    firstDependency,
                    secondDependency
                });

                // Deserialize
                singleTarget.OnAfterDeserialize();
                multiTarget.OnAfterDeserialize();

                // Assert
                Assert.That(singleDependency, Is.Not.Null);
                Assert.That(firstDependency, Is.Not.Null);
                Assert.That(secondDependency, Is.Not.Null);
                Assert.That(singleTarget.dependency, Is.EqualTo(singleDependency));

                CollectionAssert.AreEqual(new IDependency[]
                {
                    firstDependency,
                    secondDependency
                }, multiTarget.array);

                CollectionAssert.AreEqual(new IDependency[]
                {
                    firstDependency,
                    secondDependency
                }, multiTarget.list);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void OnBeforeSerialize_GivenInterfaceProperties_SyncsToBackingFields()
        {
            // Set up components
            GameObject root = new("Root");

            try
            {
                GameObject singleTargetObject = new("Single Target");
                singleTargetObject.transform.SetParent(root.transform);
                SingleInterfacePropertyTarget singleTarget = singleTargetObject.AddComponent<SingleInterfacePropertyTarget>();

                GameObject multiTargetObject = new("Multi Target");
                multiTargetObject.transform.SetParent(root.transform);
                MultiInterfacePropertyTarget multiTarget = multiTargetObject.AddComponent<MultiInterfacePropertyTarget>();

                GameObject singleDependencyObject = new("Single Dependency");
                singleDependencyObject.transform.SetParent(root.transform);
                ComponentDependency singleDependency = singleDependencyObject.AddComponent<ComponentDependency>();

                GameObject firstDependencyObject = new("First Dependency");
                firstDependencyObject.transform.SetParent(root.transform);
                ComponentDependency firstDependency = firstDependencyObject.AddComponent<ComponentDependency>();

                GameObject secondDependencyObject = new("Second Dependency");
                secondDependencyObject.transform.SetParent(root.transform);
                ComponentDependency secondDependency = secondDependencyObject.AddComponent<ComponentDependency>();

                PropertyInfo singlePropertyInfo = typeof(SingleInterfacePropertyTarget).GetProperty("Dependency", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo arrayPropertyInfo = typeof(MultiInterfacePropertyTarget).GetProperty("Array", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo listPropertyInfo = typeof(MultiInterfacePropertyTarget).GetProperty("List", BindingFlags.Public | BindingFlags.Instance);

                Assert.That(singlePropertyInfo, Is.Not.Null);
                Assert.That(arrayPropertyInfo, Is.Not.Null);
                Assert.That(listPropertyInfo, Is.Not.Null);

                MethodInfo singleSetter = singlePropertyInfo.GetSetMethod(nonPublic: true);
                MethodInfo arraySetter = arrayPropertyInfo.GetSetMethod(nonPublic: true);
                MethodInfo listSetter = listPropertyInfo.GetSetMethod(nonPublic: true);

                Assert.That(singleSetter, Is.Not.Null);
                Assert.That(arraySetter, Is.Not.Null);
                Assert.That(listSetter, Is.Not.Null);

                singleSetter.Invoke(singleTarget, new object[] { singleDependency });

                arraySetter.Invoke(multiTarget, new object[]
                {
                    new IDependency[]
                    {
                        firstDependency,
                        secondDependency
                    }
                });

                listSetter.Invoke(multiTarget, new object[]
                {
                    new List<IDependency>
                    {
                        firstDependency,
                        secondDependency
                    }
                });

                // Serialize
                singleTarget.OnBeforeSerialize();
                multiTarget.OnBeforeSerialize();

                // Find backing fields
                FieldInfo singleBackingFieldInfo = typeof(SingleInterfacePropertyTarget).GetField("__Dependency", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo arrayBackingFieldInfo = typeof(MultiInterfacePropertyTarget).GetField("__Array", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo listBackingFieldInfo = typeof(MultiInterfacePropertyTarget).GetField("__List", BindingFlags.NonPublic | BindingFlags.Instance);

                Assert.That(singleBackingFieldInfo, Is.Not.Null);
                Assert.That(arrayBackingFieldInfo, Is.Not.Null);
                Assert.That(listBackingFieldInfo, Is.Not.Null);

                Object singleBackingField = singleBackingFieldInfo.GetValue(singleTarget) as Object;
                Object[] arrayBackingField = arrayBackingFieldInfo.GetValue(multiTarget) as Object[];
                List<Object> listBackingField = listBackingFieldInfo.GetValue(multiTarget) as List<Object>;

                // Assert
                Assert.That(singleDependency, Is.Not.Null);
                Assert.That(firstDependency, Is.Not.Null);
                Assert.That(secondDependency, Is.Not.Null);
                Assert.That(singleBackingField, Is.EqualTo(singleDependency));

                CollectionAssert.AreEqual(new Object[]
                {
                    firstDependency,
                    secondDependency
                }, arrayBackingField);

                CollectionAssert.AreEqual(new Object[]
                {
                    firstDependency,
                    secondDependency
                }, listBackingField);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void OnAfterDeserialize_GivenInterfaceProperties_SyncsToInterfaceProperties()
        {
            // Set up components
            GameObject root = new("Root");

            try
            {
                GameObject singleTargetObject = new("Single Target");
                singleTargetObject.transform.SetParent(root.transform);
                SingleInterfacePropertyTarget singleTarget = singleTargetObject.AddComponent<SingleInterfacePropertyTarget>();

                GameObject multiTargetObject = new("Multi Target");
                multiTargetObject.transform.SetParent(root.transform);
                MultiInterfacePropertyTarget multiTarget = multiTargetObject.AddComponent<MultiInterfacePropertyTarget>();

                GameObject singleDependencyObject = new("Single Dependency");
                singleDependencyObject.transform.SetParent(root.transform);
                ComponentDependency singleDependency = singleDependencyObject.AddComponent<ComponentDependency>();

                GameObject firstDependencyObject = new("First Dependency");
                firstDependencyObject.transform.SetParent(root.transform);
                ComponentDependency firstDependency = firstDependencyObject.AddComponent<ComponentDependency>();

                GameObject secondDependencyObject = new("Second Dependency");
                secondDependencyObject.transform.SetParent(root.transform);
                ComponentDependency secondDependency = secondDependencyObject.AddComponent<ComponentDependency>();

                FieldInfo singleBackingFieldInfo = typeof(SingleInterfacePropertyTarget).GetField("__Dependency", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo arrayBackingFieldInfo = typeof(MultiInterfacePropertyTarget).GetField("__Array", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo listBackingFieldInfo = typeof(MultiInterfacePropertyTarget).GetField("__List", BindingFlags.NonPublic | BindingFlags.Instance);

                Assert.That(singleBackingFieldInfo, Is.Not.Null);
                Assert.That(arrayBackingFieldInfo, Is.Not.Null);
                Assert.That(listBackingFieldInfo, Is.Not.Null);

                singleBackingFieldInfo.SetValue(singleTarget, singleDependency);

                arrayBackingFieldInfo.SetValue(multiTarget, new Object[]
                {
                    firstDependency,
                    secondDependency
                });

                listBackingFieldInfo.SetValue(multiTarget, new List<Object>
                {
                    firstDependency,
                    secondDependency
                });

                // Deserialize
                singleTarget.OnAfterDeserialize();
                multiTarget.OnAfterDeserialize();

                // Assert
                Assert.That(singleDependency, Is.Not.Null);
                Assert.That(firstDependency, Is.Not.Null);
                Assert.That(secondDependency, Is.Not.Null);
                Assert.That(singleTarget.Dependency, Is.EqualTo(singleDependency));

                CollectionAssert.AreEqual(new IDependency[]
                {
                    firstDependency,
                    secondDependency
                }, multiTarget.Array);

                CollectionAssert.AreEqual(new IDependency[]
                {
                    firstDependency,
                    secondDependency
                }, multiTarget.List);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
    }
}