using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Filters
{
    public class ComponentFilterBuilderBaseMethodsTests : BaseBindingTest
    {
        private GameObject root, childA, childB;

        // -------- Where (concrete) --------
        [Test]
        public void InjectsConcrete_UsingWhere()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            // candidates
            childA.AddComponent<InjectableComponent>();
            InjectableComponent target = childB.AddComponent<InjectableComponent>();

            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .Where(c => c != null && c.gameObject.name == "ChildB");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        // -------- Where (interface) --------
        [Test]
        public void InjectsInterface_UsingWhere()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            childA.AddComponent<InjectableComponent>();
            InjectableComponent target = childB.AddComponent<InjectableComponent>();
            IInjectable targetIface = target; // capture as interface so we can compare by reference

            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromRootDescendants()
                .Where(x => ReferenceEquals(x, targetIface)); // filter purely via the interface

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.interfaceComponent);
        }

        // -------- WhereComponent (concrete) --------
        [Test]
        public void InjectsConcrete_UsingWhereComponent()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            childA.AddComponent<InjectableComponent>();
            InjectableComponent target = childB.AddComponent<InjectableComponent>();

            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereComponent(c => c.gameObject.name == "ChildB");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        // -------- WhereComponent (interface) --------
        [Test]
        public void InjectsInterface_UsingWhereComponent()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            childA.AddComponent<InjectableComponent>();
            InjectableComponent target = childB.AddComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereComponent(c => c.gameObject.name == "ChildB"); // access Component API even though T is interface

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.interfaceComponent);
        }

        // -------- WhereGameObject (concrete) --------
        [Test]
        public void InjectsConcrete_UsingWhereGameObject()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            InjectableComponent target = childA.AddComponent<InjectableComponent>();
            childB.AddComponent<InjectableComponent>();

            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereGameObject(go => go.name == "ChildA");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        // -------- WhereGameObject (interface) --------
        [Test]
        public void InjectsInterface_UsingWhereGameObject()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            InjectableComponent target = childA.AddComponent<InjectableComponent>();
            childB.AddComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereGameObject(go => go.name == "ChildA");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.interfaceComponent);
        }

        // -------- WhereTransform (concrete) --------
        [Test]
        public void InjectsConcrete_UsingWhereTransform()
        {
            IgnoreErrorMessages();

            // Make childB a child of childA so we can filter by parent
            childB.transform.SetParent(childA.transform);

            TestScope scope = childA.AddComponent<TestScope>();
            ComponentRequester requester = childA.AddComponent<ComponentRequester>();

            InjectableComponent target = childB.AddComponent<InjectableComponent>();

            BindComponent<InjectableComponent>(scope)
                .FromDescendants()
                .WhereTransform(t => t.parent != null && t.parent.name == "ChildA");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        // -------- WhereTransform (interface) --------
        [Test]
        public void InjectsInterface_UsingWhereTransform()
        {
            IgnoreErrorMessages();

            childB.transform.SetParent(childA.transform);

            TestScope scope = childA.AddComponent<TestScope>();
            ComponentRequester requester = childA.AddComponent<ComponentRequester>();

            InjectableComponent target = childB.AddComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromDescendants()
                .WhereTransform(t => t.parent != null && t.parent.name == "ChildA");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
            childA = new GameObject("ChildA");
            childB = new GameObject("ChildB");

            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);
        }
    }
}