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
        private TestComponent testComponent;
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
            testComponent = consumer.AddComponent<TestComponent>();

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

            protected override void ConfigureBindings()
            {
                BindComponent<InjectableService>()
                    .FromMethod(method);

                BindComponent<IInjectableService, InjectableService>()
                    .FromMethod(method);
            }
        }
    }
}