using System.Collections.Generic;
using NUnit.Framework;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;

namespace Tests.Editor.Bindings.Misc
{
    public class BindingEqualityTests : BaseTest
    {
        private Scope scopeA, scopeB;

        public override void SetUp()
        {
            scopeA = new GameObject("ScopeA").AddComponent<TestScope>();
            scopeB = new GameObject("ScopeB").AddComponent<TestScope>();
        }

        [Test]
        public void EqualBindings_AreEqual()
        {
            Binding a = MakeBinding(scopeA);
            Binding b = MakeBinding(scopeA);

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Bindings_WithDifferentScopes_AreNotEqual()
        {
            Binding a = MakeBinding(scopeA);
            Binding b = MakeBinding(scopeB);

            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void Bindings_WithDifferentIds_AreNotEqual()
        {
            Binding a = MakeBinding(scopeA, id: "x");
            Binding b = MakeBinding(scopeA, id: "y");

            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void Bindings_WithDifferentGlobalFlags_AreNotEqual()
        {
            Binding a = MakeBinding(scopeA, isGlobal: false);
            Binding b = MakeBinding(scopeA, isGlobal: true);

            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void Bindings_WithDifferentCollectionFlags_AreNotEqual()
        {
            Binding a = MakeBinding(scopeA, isCollection: false);
            Binding b = MakeBinding(scopeA, isCollection: true);

            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void Bindings_WithDifferentTargetTypes_AreNotEqual()
        {
            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetFilter(_ => true, typeof(Component));

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetFilter(_ => true, typeof(Transform));

            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void Bindings_WithSameTargetTypes_DifferentOrder_AreEqual()
        {
            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetFilter(_ => true, typeof(Transform));
            a.AddInjectionTargetFilter(_ => true, typeof(MeshRenderer));

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetFilter(_ => true, typeof(MeshRenderer));
            b.AddInjectionTargetFilter(_ => true, typeof(Transform));

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Binding_WithNull_InterfaceType_UsesConcreteType()
        {
            Binding a = new(null, typeof(InjectableComponent), scopeA);
            Binding b = new(null, typeof(InjectableComponent), scopeA);

            a.MarkComponentBinding();
            b.MarkComponentBinding();
            a.SetLocator(_ => null);
            b.SetLocator(_ => null);

            Assert.AreEqual(a, b);
        }

        [Test]
        public void Binding_WithNulls_AreNotEqual()
        {
            Binding a = new(null, typeof(InjectableComponent), scopeA);
            Binding b = new(null, null, scopeA);

            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void HashSet_TreatsEqualBindingsAsSame()
        {
            HashSet<Binding> set = new();

            Binding a = MakeBinding(scopeA, id: "same");
            Binding b = MakeBinding(scopeA, id: "same");

            bool addedA = set.Add(a);
            bool addedB = set.Add(b);

            Assert.IsTrue(addedA);
            Assert.IsFalse(addedB, "Expected second binding with same identity to be rejected by HashSet.");
            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void HashSet_TreatsDifferentBindingsAsUnique()
        {
            HashSet<Binding> set = new();

            set.Add(MakeBinding(scopeA, id: "a"));
            set.Add(MakeBinding(scopeA, id: "b"));
            set.Add(MakeBinding(scopeB, id: "a")); // different scope

            Assert.AreEqual(3, set.Count);
        }

        [Test]
        public void HashSet_RespectsTargetTypeEqualityRegardlessOfOrder()
        {
            HashSet<Binding> set = new();

            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetFilter(_ => true, typeof(Transform));
            a.AddInjectionTargetFilter(_ => true, typeof(MeshRenderer));

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetFilter(_ => true, typeof(MeshRenderer));
            b.AddInjectionTargetFilter(_ => true, typeof(Transform));

            bool addedA = set.Add(a);
            bool addedB = set.Add(b);

            Assert.IsTrue(addedA);
            Assert.IsFalse(addedB, "HashSet should treat reordered target type filters as the same.");
            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void HashSet_TreatsBindingWithExtraTargetFilterAsDifferent()
        {
            HashSet<Binding> set = new();

            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetFilter(_ => true, typeof(Transform));

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetFilter(_ => true, typeof(Transform));
            b.AddInjectionTargetFilter(_ => true, typeof(MeshRenderer));

            set.Add(a);
            set.Add(b);

            Assert.AreEqual(2, set.Count, "Bindings with differing numbers of target filters should be treated as unique.");
        }

        private Binding MakeBinding(
            Scope scope,
            string id = null,
            bool isGlobal = false,
            bool isCollection = false)
        {
            Binding binding = new(typeof(IInjectable), typeof(InjectableComponent), scope);
            binding.MarkComponentBinding();
            binding.SetLocator(_ => null);

            if (id != null)
                binding.SetId(id);

            if (isGlobal)
                binding.MarkGlobal();

            if (isCollection)
                binding.MarkCollectionBinding();

            return binding;
        }
    }
}