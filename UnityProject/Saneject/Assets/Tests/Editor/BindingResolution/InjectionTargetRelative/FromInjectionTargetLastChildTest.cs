using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.InjectionTargetRelative
{
    public class FromTargetLastChildTest
    {
        private TestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            GameObject targetObject = new("Target");
            targetObject.transform.SetParent(root.transform);

            GameObject firstChild = new("FirstChild");
            firstChild.transform.SetParent(targetObject.transform);

            GameObject lastChild = new("LastChild");
            lastChild.transform.SetParent(targetObject.transform);

            lastChild.AddComponent<InjectableService>();
            testComponent = targetObject.AddComponent<TestComponent>();
            root.AddComponent<TestScope>();

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
            protected override void ConfigureBindings()
            {
                BindComponent<InjectableService>()
                    .FromTargetLastChild();

                BindComponent<IInjectableService, InjectableService>()
                    .FromTargetLastChild();
            }
        }
    }
}