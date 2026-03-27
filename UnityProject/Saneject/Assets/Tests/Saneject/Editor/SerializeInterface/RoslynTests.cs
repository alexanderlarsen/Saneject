using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Runtime.Proxy;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.SerializeInterface
{
    public class RoslynTests
    {
        [Test]
        public void Class_GivenSerializeInterfaceFields_ImplementsISerializationCallbackReceiver()
        {
            // Find implemented interfaces
            Type[] singleInterfaces = typeof(SingleInterfaceTarget).GetInterfaces();
            Type[] multiInterfaces = typeof(MultiInterfaceTarget).GetInterfaces();

            // Assert
            CollectionAssert.Contains(singleInterfaces, typeof(ISerializationCallbackReceiver));
            CollectionAssert.Contains(multiInterfaces, typeof(ISerializationCallbackReceiver));
        }

        [Test]
        public void Class_GivenSerializeInterfaceFields_ImplementsIRuntimeProxySwapTarget()
        {
            // Find implemented interfaces
            Type[] singleInterfaces = typeof(SingleInterfaceTarget).GetInterfaces();
            Type[] multiInterfaces = typeof(MultiInterfaceTarget).GetInterfaces();

            // Assert
            CollectionAssert.Contains(singleInterfaces, typeof(IRuntimeProxySwapTarget));
            CollectionAssert.Contains(multiInterfaces, typeof(IRuntimeProxySwapTarget));
        }

        [Test]
        public void Class_GivenSerializeInterfaceFields_GeneratesExpectedMembers()
        {
            // Find generated members
            FieldInfo singleBackingField = typeof(SingleInterfaceTarget).GetField("__dependency", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo arrayBackingField = typeof(MultiInterfaceTarget).GetField("__array", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo listBackingField = typeof(MultiInterfaceTarget).GetField("__list", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo singleOnBeforeSerialize = typeof(SingleInterfaceTarget).GetMethod("OnBeforeSerialize", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo singleOnAfterDeserialize = typeof(SingleInterfaceTarget).GetMethod("OnAfterDeserialize", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo singleSwapProxiesWithRealInstances = typeof(SingleInterfaceTarget).GetMethod("SwapProxiesWithRealInstances", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo multiOnBeforeSerialize = typeof(MultiInterfaceTarget).GetMethod("OnBeforeSerialize", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo multiOnAfterDeserialize = typeof(MultiInterfaceTarget).GetMethod("OnAfterDeserialize", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo multiSwapProxiesWithRealInstances = typeof(MultiInterfaceTarget).GetMethod("SwapProxiesWithRealInstances", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            Assert.That(singleBackingField, Is.Not.Null);
            Assert.That(singleBackingField.FieldType, Is.EqualTo(typeof(Object)));
            Assert.That(arrayBackingField, Is.Not.Null);
            Assert.That(arrayBackingField.FieldType, Is.EqualTo(typeof(Object[])));
            Assert.That(listBackingField, Is.Not.Null);
            Assert.That(listBackingField.FieldType, Is.EqualTo(typeof(List<Object>)));
            Assert.That(singleOnBeforeSerialize, Is.Not.Null);
            Assert.That(singleOnAfterDeserialize, Is.Not.Null);
            Assert.That(singleSwapProxiesWithRealInstances, Is.Not.Null);
            Assert.That(multiOnBeforeSerialize, Is.Not.Null);
            Assert.That(multiOnAfterDeserialize, Is.Not.Null);
            Assert.That(multiSwapProxiesWithRealInstances, Is.Not.Null);
        }

        [Test]
        public void Class_GivenSerializeInterfaceProperties_ImplementsISerializationCallbackReceiver()
        {
            // Find implemented interfaces
            Type[] singleInterfaces = typeof(SingleInterfacePropertyTarget).GetInterfaces();
            Type[] multiInterfaces = typeof(MultiInterfacePropertyTarget).GetInterfaces();

            // Assert
            CollectionAssert.Contains(singleInterfaces, typeof(ISerializationCallbackReceiver));
            CollectionAssert.Contains(multiInterfaces, typeof(ISerializationCallbackReceiver));
        }

        [Test]
        public void Class_GivenSerializeInterfaceProperties_ImplementsIRuntimeProxySwapTarget()
        {
            // Find implemented interfaces
            Type[] singleInterfaces = typeof(SingleInterfacePropertyTarget).GetInterfaces();
            Type[] multiInterfaces = typeof(MultiInterfacePropertyTarget).GetInterfaces();

            // Assert
            CollectionAssert.Contains(singleInterfaces, typeof(IRuntimeProxySwapTarget));
            CollectionAssert.Contains(multiInterfaces, typeof(IRuntimeProxySwapTarget));
        }

        [Test]
        public void Class_GivenSerializeInterfaceProperties_GeneratesExpectedMembers()
        {
            // Find generated members
            FieldInfo singleBackingField = typeof(SingleInterfacePropertyTarget).GetField("__Dependency", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo arrayBackingField = typeof(MultiInterfacePropertyTarget).GetField("__Array", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo listBackingField = typeof(MultiInterfacePropertyTarget).GetField("__List", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo singleOnBeforeSerialize = typeof(SingleInterfacePropertyTarget).GetMethod("OnBeforeSerialize", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo singleOnAfterDeserialize = typeof(SingleInterfacePropertyTarget).GetMethod("OnAfterDeserialize", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo singleSwapProxiesWithRealInstances = typeof(SingleInterfacePropertyTarget).GetMethod("SwapProxiesWithRealInstances", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo multiOnBeforeSerialize = typeof(MultiInterfacePropertyTarget).GetMethod("OnBeforeSerialize", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo multiOnAfterDeserialize = typeof(MultiInterfacePropertyTarget).GetMethod("OnAfterDeserialize", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo multiSwapProxiesWithRealInstances = typeof(MultiInterfacePropertyTarget).GetMethod("SwapProxiesWithRealInstances", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            Assert.That(singleBackingField, Is.Not.Null);
            Assert.That(singleBackingField.FieldType, Is.EqualTo(typeof(Object)));
            Assert.That(arrayBackingField, Is.Not.Null);
            Assert.That(arrayBackingField.FieldType, Is.EqualTo(typeof(Object[])));
            Assert.That(listBackingField, Is.Not.Null);
            Assert.That(listBackingField.FieldType, Is.EqualTo(typeof(List<Object>)));
            Assert.That(singleOnBeforeSerialize, Is.Not.Null);
            Assert.That(singleOnAfterDeserialize, Is.Not.Null);
            Assert.That(singleSwapProxiesWithRealInstances, Is.Not.Null);
            Assert.That(multiOnBeforeSerialize, Is.Not.Null);
            Assert.That(multiOnAfterDeserialize, Is.Not.Null);
            Assert.That(multiSwapProxiesWithRealInstances, Is.Not.Null);
        }
    }
}
