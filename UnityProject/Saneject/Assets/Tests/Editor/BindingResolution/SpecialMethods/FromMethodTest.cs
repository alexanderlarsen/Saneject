using System;
using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests.Editor.BindingResolution.SpecialMethods
{
    public class FromMethodTest
    {
        private Runtime.TestComponent testComponent;
        private InjectableService serviceInstance;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject serviceHolder = new("ServiceHolder");
            serviceInstance = serviceHolder.AddComponent<InjectableService>();

            GameObject consumer = new("ConsumerObject");
            testComponent = consumer.AddComponent<Runtime.TestComponent>();

            TestScope scope = consumer.AddComponent<TestScope>();
            scope.method = () => serviceInstance;

            DependencyInjector.InjectSceneDependencies();
        }

        [Test]
        public void InjectsConcrete()
        {
            Assert.NotNull(testComponent.Service);
        }

        [Test]
        public void InjectsInterface()
        {
            Assert.NotNull(testComponent.ServiceInterface);
        }

        public class TestScope : Scope
        {
            public Func<InjectableService> method;

            public override void Configure()
            {
                Bind<InjectableService>().FromMethod(method);
                Bind<IInjectableService, InjectableService>().FromMethod(method);
            }
        }
    }
}