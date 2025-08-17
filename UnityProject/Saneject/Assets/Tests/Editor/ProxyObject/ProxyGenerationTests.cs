using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Tests.Runtime.Proxy;

namespace Tests.Editor.ProxyObject
{
    public class ProxyGenerationTests
    {
        private static readonly Type[] AllInterfaces =
        {
            typeof(IProxyTarget),
            typeof(IAdvancedProxyTarget)
        };

        private Type proxyType;

        [SetUp]
        public void Setup()
        {
            proxyType = typeof(Runtime.Proxy.ProxyObject);
        }

        [Test]
        public void Proxy_Implements_AllMembers_From_AllInterfaces()
        {
            foreach (Type iface in AllInterfaces)
            {
                foreach (MemberInfo member in iface.GetMembers())
                {
                    bool exists = proxyType.GetMember(member.Name,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                        .Any(m => m.MemberType == member.MemberType);

                    Assert.IsTrue(exists,
                        $"Missing member from {iface.Name}: {member.MemberType} {member.Name}");
                }
            }
        }

        [Test]
        public void Proxy_Handles_MemberNameCollisionsAcrossInterfaces()
        {
            MethodInfo echo = proxyType.GetMethod("Echo", new[] { typeof(string) });
            Assert.NotNull(echo, "Proxy must implement Echo(string)");

            List<PropertyInfo> props = proxyType.GetProperties().Where(p => p.Name == "ReadWriteProperty").ToList();
            Assert.AreEqual(1, props.Count, "Proxy should contain only one ReadWriteProperty");
        }

        [Test]
        public void Proxy_Implements_ExpectedProperties()
        {
            AssertProperty("ReadOnlyProp", canRead: true, canWrite: false);
            AssertProperty("ReadWriteProp", canRead: true, canWrite: true);
            AssertProperty("WriteOnlyProp", canRead: false, canWrite: true);
            AssertProperty("BaseProp", canRead: true, canWrite: false);
        }

        [Test]
        public void Proxy_Implements_ExpectedMethods()
        {
            AssertMethod("NoArgMethod", 0);
            AssertMethod("Echo", 1);
            AssertMethod("Sum", 2);
            AssertMethod("GetList", 0);
            AssertMethod("BaseMethod", 0);
        }

        [Test]
        public void Proxy_Implements_Event()
        {
            EventInfo evt = proxyType.GetEvent("SomethingHappened");
            Assert.NotNull(evt, "Missing event: SomethingHappened");
        }

        private void AssertProperty(
            string name,
            bool canRead,
            bool canWrite)
        {
            PropertyInfo prop = proxyType.GetProperty(name);
            Assert.NotNull(prop, $"Missing property: {name}");
            Assert.AreEqual(canRead, prop.CanRead, $"Property {name} CanRead mismatch");
            Assert.AreEqual(canWrite, prop.CanWrite, $"Property {name} CanWrite mismatch");
        }

        private void AssertMethod(
            string name,
            int paramCount)
        {
            MethodInfo method = proxyType.GetMethods()
                .FirstOrDefault(m => m.Name == name && m.GetParameters().Length == paramCount);

            Assert.NotNull(method, $"Missing method: {name} with {paramCount} parameter(s)");
        }
    }
}