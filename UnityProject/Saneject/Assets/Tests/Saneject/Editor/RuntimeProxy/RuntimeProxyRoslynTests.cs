using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Runtime.Proxy;
using Tests.Saneject.Fixtures.Scripts.RuntimeProxy;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.RuntimeProxy
{
    public class RuntimeProxyRoslynTests
    {
        private static readonly Type[] AllInterfaces =
        {
            typeof(IRuntimeProxyTarget),
            typeof(IAdvancedRuntimeProxyTarget),
            typeof(IBaseRuntimeProxyTarget)
        };

        [Test]
        public void Proxy_Implements_AllPublicNonGenericInterfaces_From_Target()
        {
            // Find implemented interfaces
            Type[] interfaces = typeof(GeneratedRuntimeProxy).GetInterfaces();

            // Assert
            CollectionAssert.Contains(interfaces, typeof(IRuntimeProxyTarget));
            CollectionAssert.Contains(interfaces, typeof(IAdvancedRuntimeProxyTarget));
            CollectionAssert.Contains(interfaces, typeof(IBaseRuntimeProxyTarget));
        }

        [Test]
        public void Proxy_Implements_AllMembers_From_AllInterfaces()
        {
            foreach (Type iface in AllInterfaces)
            {
                foreach (MemberInfo member in iface.GetMembers())
                {
                    bool exists = typeof(GeneratedRuntimeProxy)
                        .GetMember(member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                        .Any(m => m.MemberType == member.MemberType);

                    Assert.That(exists, Is.True, $"Missing member from {iface.Name}: {member.MemberType} {member.Name}");
                }
            }
        }

        [Test]
        public void Proxy_Handles_MemberNameCollisionsAcrossInterfaces()
        {
            // Find colliding members
            MethodInfo echo = typeof(GeneratedRuntimeProxy).GetMethod("Echo", new[] { typeof(string) });

            List<PropertyInfo> readWriteProperties = typeof(GeneratedRuntimeProxy)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.Name == "ReadWriteProperty")
                .ToList();

            // Assert
            Assert.That(echo, Is.Not.Null);
            Assert.That(readWriteProperties, Has.Count.EqualTo(1));
        }

        [Test]
        public void Proxy_Implements_ExpectedPropertyAccess()
        {
            // Find properties
            PropertyInfo baseProp = typeof(GeneratedRuntimeProxy).GetProperty("BaseProp");
            PropertyInfo readOnlyProperty = typeof(GeneratedRuntimeProxy).GetProperty("ReadOnlyProperty");
            PropertyInfo readOnlyProp = typeof(GeneratedRuntimeProxy).GetProperty("ReadOnlyProp");
            PropertyInfo readWriteProperty = typeof(GeneratedRuntimeProxy).GetProperty("ReadWriteProperty");
            PropertyInfo writeOnlyProperty = typeof(GeneratedRuntimeProxy).GetProperty("WriteOnlyProperty");
            PropertyInfo writeOnlyProp = typeof(GeneratedRuntimeProxy).GetProperty("WriteOnlyProp");

            // Assert
            Assert.That(baseProp, Is.Not.Null);
            Assert.That(baseProp.CanRead, Is.True);
            Assert.That(baseProp.CanWrite, Is.False);
            Assert.That(readOnlyProperty, Is.Not.Null);
            Assert.That(readOnlyProperty.CanRead, Is.True);
            Assert.That(readOnlyProperty.CanWrite, Is.False);
            Assert.That(readOnlyProp, Is.Not.Null);
            Assert.That(readOnlyProp.CanRead, Is.True);
            Assert.That(readOnlyProp.CanWrite, Is.False);
            Assert.That(readWriteProperty, Is.Not.Null);
            Assert.That(readWriteProperty.CanRead, Is.True);
            Assert.That(readWriteProperty.CanWrite, Is.True);
            Assert.That(writeOnlyProperty, Is.Not.Null);
            Assert.That(writeOnlyProperty.CanRead, Is.False);
            Assert.That(writeOnlyProperty.CanWrite, Is.True);
            Assert.That(writeOnlyProp, Is.Not.Null);
            Assert.That(writeOnlyProp.CanRead, Is.False);
            Assert.That(writeOnlyProp.CanWrite, Is.True);
        }

        [Test]
        public void Proxy_Implements_ExpectedMethods()
        {
            // Find methods
            MethodInfo baseMethod = typeof(GeneratedRuntimeProxy).GetMethod("BaseMethod", Type.EmptyTypes);
            MethodInfo doSomething = typeof(GeneratedRuntimeProxy).GetMethod("DoSomething", Type.EmptyTypes);
            MethodInfo noArgMethod = typeof(GeneratedRuntimeProxy).GetMethod("NoArgMethod", Type.EmptyTypes);
            MethodInfo echo = typeof(GeneratedRuntimeProxy).GetMethod("Echo", new[] { typeof(string) });

            MethodInfo add = typeof(GeneratedRuntimeProxy).GetMethod("Add", new[]
            {
                typeof(int),
                typeof(int)
            });

            MethodInfo sum = typeof(GeneratedRuntimeProxy).GetMethod("Sum", new[]
            {
                typeof(int),
                typeof(int)
            });

            MethodInfo getList = typeof(GeneratedRuntimeProxy).GetMethod("GetList", Type.EmptyTypes);

            // Assert
            Assert.That(baseMethod, Is.Not.Null);
            Assert.That(doSomething, Is.Not.Null);
            Assert.That(noArgMethod, Is.Not.Null);
            Assert.That(echo, Is.Not.Null);
            Assert.That(add, Is.Not.Null);
            Assert.That(sum, Is.Not.Null);
            Assert.That(getList, Is.Not.Null);
        }

        [Test]
        public void Proxy_Implements_ExpectedEvents()
        {
            // Find events
            EventInfo onTriggered = typeof(GeneratedRuntimeProxy).GetEvent("OnTriggered");
            EventInfo somethingHappened = typeof(GeneratedRuntimeProxy).GetEvent("SomethingHappened");

            // Assert
            Assert.That(onTriggered, Is.Not.Null);
            Assert.That(somethingHappened, Is.Not.Null);
        }

        [Test]
        public void Proxy_DoesNotImplement_FilteredInterfaces()
        {
            // Find implemented interfaces
            Type[] interfaces = typeof(GeneratedRuntimeProxy).GetInterfaces();

            // internal interface
            Type hiddenInterface = typeof(RuntimeProxyTargetComponent)
                .GetInterfaces()
                .First(type => type.Name == "IHiddenRuntimeProxyTarget");

            // Assert
            CollectionAssert.DoesNotContain(interfaces, typeof(IRuntimeProxyGenericTarget<int>));
            CollectionAssert.DoesNotContain(interfaces, typeof(ISerializationCallbackReceiver));
            CollectionAssert.DoesNotContain(interfaces, typeof(IRuntimeProxySwapTarget));
            CollectionAssert.DoesNotContain(interfaces, hiddenInterface);
        }

        [Test]
        public void Proxy_AccessedDirectly_ThrowsInvalidOperationException()
        {
            // Set up proxy
            GeneratedRuntimeProxy proxy = ScriptableObject.CreateInstance<GeneratedRuntimeProxy>();
            IRuntimeProxyTarget runtimeProxyTarget = proxy;

            Action handler = () =>
            {
            };

            try
            {
                // Access members
                InvalidOperationException getterException = Assert.Throws<InvalidOperationException>(() => _ = runtimeProxyTarget.ReadOnlyProperty);
                InvalidOperationException setterException = Assert.Throws<InvalidOperationException>(() => runtimeProxyTarget.ReadWriteProperty = "Value");
                InvalidOperationException methodException = Assert.Throws<InvalidOperationException>(() => runtimeProxyTarget.DoSomething());
                InvalidOperationException addEventException = Assert.Throws<InvalidOperationException>(() => runtimeProxyTarget.OnTriggered += handler);
                InvalidOperationException removeEventException = Assert.Throws<InvalidOperationException>(() => runtimeProxyTarget.OnTriggered -= handler);

                // Assert
                Assert.That(getterException, Is.Not.Null);
                Assert.That(setterException, Is.Not.Null);
                Assert.That(methodException, Is.Not.Null);
                Assert.That(addEventException, Is.Not.Null);
                Assert.That(removeEventException, Is.Not.Null);
                Assert.That(getterException.Message, Does.Contain("RuntimeProxy instances are serialized placeholders and should never be accessed directly."));
                Assert.That(setterException.Message, Does.Contain("RuntimeProxy instances are serialized placeholders and should never be accessed directly."));
                Assert.That(methodException.Message, Does.Contain("RuntimeProxy instances are serialized placeholders and should never be accessed directly."));
                Assert.That(addEventException.Message, Does.Contain("RuntimeProxy instances are serialized placeholders and should never be accessed directly."));
                Assert.That(removeEventException.Message, Does.Contain("RuntimeProxy instances are serialized placeholders and should never be accessed directly."));
            }
            finally
            {
                Object.DestroyImmediate(proxy);
            }
        }
    }
}