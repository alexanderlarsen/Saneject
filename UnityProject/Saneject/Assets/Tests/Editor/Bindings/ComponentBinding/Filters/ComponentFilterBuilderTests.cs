using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Filters
{
    public class ComponentFilterBuilderTests : BaseBindingTest
    {
        private GameObject root, childA, childB, childC;

        [Test]
        public void Injects_UsingWhere_CustomPredicate()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            InjectableComponent target = root.AddComponent<InjectableComponent>();

            BindComponent<InjectableComponent>(scope)
                .FromScopeSelf()
                .Where(c => c != null && c.gameObject.name == "Root");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void Injects_UsingWhereGameObject_ByName()
        {
            IgnoreErrorMessages();

            TestScope scope = childA.AddComponent<TestScope>();
            ComponentRequester requester = childA.AddComponent<ComponentRequester>();

            InjectableComponent target = childA.AddComponent<InjectableComponent>();

            BindComponent<InjectableComponent>(scope)
                .FromScopeSelf()
                .WhereGameObject(go => go.name == "ChildA");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void Injects_UsingWhereTransform_ByParentName()
        {
            IgnoreErrorMessages();

            TestScope scope = childA.AddComponent<TestScope>();
            ComponentRequester requester = childA.AddComponent<ComponentRequester>();

            // Add InjectableComponent on childB
            InjectableComponent target = childB.AddComponent<InjectableComponent>();
            childB.transform.SetParent(childA.transform);

            BindComponent<InjectableComponent>(scope)
                .FromDescendants()
                .WhereTransform(t => t.parent != null && t.parent.name == "ChildA");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void Injects_UsingWhereParent_ByExactTransform()
        {
            IgnoreErrorMessages();

            TestScope scope = childA.AddComponent<TestScope>();
            ComponentRequester requester = childA.AddComponent<ComponentRequester>();

            InjectableComponent target = childB.AddComponent<InjectableComponent>();
            childB.transform.SetParent(childA.transform);

            BindComponent<InjectableComponent>(scope)
                .FromDescendants()
                .WhereParent(p => p == childA.transform);

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void Injects_UsingWhereAnyAncestor_ByRootName()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            childA.transform.SetParent(root.transform);
            // Nested under childA
            GameObject deep = new("Deep");
            deep.transform.SetParent(childA.transform);
            InjectableComponent target = deep.AddComponent<InjectableComponent>();

            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereAnyAncestor(a => a.name == "Root");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void Injects_UsingWhereRoot_ByName()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            childA.transform.SetParent(root.transform);
            ;

            GameObject deep = new("Deep");
            deep.transform.SetParent(childA.transform);
            InjectableComponent target = deep.AddComponent<InjectableComponent>();

            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereRoot(r => r.name == "Root");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void Injects_UsingWhereAnyChild_ByName()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            InjectableComponent target = root.AddComponent<InjectableComponent>();

            // root has children
            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);

            BindComponent<InjectableComponent>(scope)
                .FromScopeSelf()
                .WhereAnyChild(c => c.name == "ChildA");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void Injects_UsingWhereChildAt_ByIndex()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            InjectableComponent target = root.AddComponent<InjectableComponent>();

            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);
            childC.transform.SetParent(root.transform);

            BindComponent<InjectableComponent>(scope)
                .FromScopeSelf()
                .WhereChildAt(1, t => t.name == "ChildB");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void Injects_UsingWhereFirstChild()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            InjectableComponent target = root.AddComponent<InjectableComponent>();

            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);

            BindComponent<InjectableComponent>(scope)
                .FromScopeSelf()
                .WhereFirstChild(t => t.name == "ChildA");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void Injects_UsingWhereLastChild()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            InjectableComponent target = root.AddComponent<InjectableComponent>();

            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);
            childC.transform.SetParent(root.transform);

            BindComponent<InjectableComponent>(scope)
                .FromScopeSelf()
                .WhereLastChild(t => t.name == "ChildC");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void Injects_UsingWhereAnyDescendant()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            InjectableComponent target = root.AddComponent<InjectableComponent>();

            GameObject deep = new("Deep");
            deep.transform.SetParent(childB.transform);
            childB.transform.SetParent(root.transform);

            BindComponent<InjectableComponent>(scope)
                .FromScopeSelf()
                .WhereAnyDescendant(t => t.name == "Deep");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void Injects_UsingWhereAnySibling()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            // Add InjectableComponent to childB
            InjectableComponent target = childB.AddComponent<InjectableComponent>();
            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);
            childC.transform.SetParent(root.transform);

            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereAnySibling(s => s.name == "ChildA");

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
            childA = new GameObject("ChildA");
            childB = new GameObject("ChildB");
            childC = new GameObject("ChildC");
        }
    }
}