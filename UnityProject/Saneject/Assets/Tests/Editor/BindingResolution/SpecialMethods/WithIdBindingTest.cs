using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.SpecialMethods
{
    public class WithIdBindingTest
    {
        private GameObject consumer;
        private Runtime.TestIdComponent target;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject a = new("ServiceA");
            a.AddComponent<InjectableService>();

            GameObject b = new("ServiceB");
            b.AddComponent<InjectableService>();

            GameObject ia = new("ServiceInterfaceA");
            ia.AddComponent<InjectableService>();

            GameObject ib = new("ServiceInterfaceB");
            ib.AddComponent<InjectableService>();

            consumer = new GameObject("Consumer");
            target = consumer.AddComponent<Runtime.TestIdComponent>();

            TestScope testScope = consumer.AddComponent<TestScope>();
            testScope.serviceA = a;
            testScope.serviceB = b;
            testScope.interfaceA = ia;
            testScope.interfaceB = ib;

            DependencyInjector.InjectSceneDependencies();
        }

        [Test]
        public void InjectsAllById()
        {
            Assert.NotNull(target.InjectableServiceA);
            Assert.NotNull(target.InjectableServiceB);
            Assert.NotNull(target.InjectableServiceInterfaceA);
            Assert.NotNull(target.InjectableServiceInterfaceB);

            Assert.AreNotSame(target.InjectableServiceA, target.InjectableServiceB);
            Assert.AreNotSame(target.InjectableServiceInterfaceA, target.InjectableServiceInterfaceB);
            Assert.AreNotSame(target.InjectableServiceA, target.InjectableServiceInterfaceA);
        }

        public class TestScope : Scope
        {
            public GameObject serviceA;
            public GameObject serviceB;
            public GameObject interfaceA;
            public GameObject interfaceB;

            public override void Configure()
            {
                Bind<InjectableService>().WithId("A").From(serviceA.transform);
                Bind<InjectableService>().WithId("B").From(serviceB.transform);
                Bind<IInjectableService, InjectableService>().WithId("InterfaceA").From(interfaceA.transform);
                Bind<IInjectableService, InjectableService>().WithId("InterfaceB").From(interfaceB.transform);
            }
        }
    }
}