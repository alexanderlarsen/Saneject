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

        // ---------- Core signature (no filters) ----------

        [Test]
        public void Bindings_WithSameCoreAndNoFilters_AreEqual()
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

        // ---------- Target-type filter equality (overlap semantics) ----------

        [Test]
        public void TargetFilters_DisjointTypes_AreNotEqual()
        {
            // Rigidbody vs Transform → no assignability either way → disjoint
            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetQualifier(_ => true, typeof(Rigidbody));

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetQualifier(_ => true, typeof(Transform));

            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void TargetFilters_SameTypes_DifferentOrder_AreEqual()
        {
            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetQualifier(_ => true, typeof(Transform));
            a.AddInjectionTargetQualifier(_ => true, typeof(MeshRenderer));

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetQualifier(_ => true, typeof(MeshRenderer));
            b.AddInjectionTargetQualifier(_ => true, typeof(Transform));

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void TargetFilters_SupersetOverlaps_AreEqual()
        {
            // {Transform} overlaps with {Transform, MeshRenderer} → equal
            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetQualifier(_ => true, typeof(Transform));

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetQualifier(_ => true, typeof(Transform));
            b.AddInjectionTargetQualifier(_ => true, typeof(MeshRenderer));

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void TargetFilters_BaseAndDerivedTypes_Overlap_AreEqual()
        {
            // Component (base) vs Transform (derived) → assignability overlap → equal
            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetQualifier(_ => true, typeof(Component));

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetQualifier(_ => true, typeof(Transform));

            Assert.AreEqual(a, b);
        }

        [Test]
        public void TargetFilters_EmptyVsNonEmpty_AreNotEqual()
        {
            Binding a = MakeBinding(scopeA); // no target filters

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetQualifier(_ => true, typeof(Transform));

            Assert.AreNotEqual(a, b);
        }

        // ---------- Member-name filter equality (overlap semantics) ----------

        [Test]
        public void MemberNameFilters_Disjoint_AreNotEqual()
        {
            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetMemberQualifier(name => name == "monoA", "monoA");

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetMemberQualifier(name => name == "monoB", "monoB");

            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void MemberNameFilters_SameNames_DifferentOrder_AreEqual()
        {
            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetMemberQualifier(name => name == "monoA", "monoA");
            a.AddInjectionTargetMemberQualifier(name => name == "monoB", "monoB");

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetMemberQualifier(name => name == "monoB", "monoB");
            b.AddInjectionTargetMemberQualifier(name => name == "monoA", "monoA");

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void MemberNameFilters_SupersetOverlaps_AreEqual()
        {
            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetMemberQualifier(n => n == "monoA", "monoA");

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetMemberQualifier(n => n == "monoA", "monoA");
            b.AddInjectionTargetMemberQualifier(n => n == "monoB", "monoB");

            Assert.AreEqual(a, b);
        }

        [Test]
        public void MemberNameFilters_EmptyVsNonEmpty_AreNotEqual()
        {
            Binding a = MakeBinding(scopeA); // no member-name filters

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetMemberQualifier(n => n == "monoA", "monoA");

            Assert.AreNotEqual(a, b);
        }

        // ---------- Combined: both target-type and member-name filters ----------

        [Test]
        public void CombinedFilters_BothOverlap_AreEqual()
        {
            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetQualifier(_ => true, typeof(Transform));
            a.AddInjectionTargetMemberQualifier(n => n == "monoA", "monoA");

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetQualifier(_ => true, typeof(Transform));
            b.AddInjectionTargetQualifier(_ => true, typeof(MeshRenderer)); // superset
            b.AddInjectionTargetMemberQualifier(n => n == "monoA", "monoA");
            b.AddInjectionTargetMemberQualifier(n => n == "monoB", "monoB"); // superset

            Assert.AreEqual(a, b, "Both target-types and member-names overlap → equal.");
        }

        [Test]
        public void CombinedFilters_OneOverlaps_OtherDisjoint_AreNotEqual()
        {
            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetQualifier(_ => true, typeof(Transform));
            a.AddInjectionTargetMemberQualifier(n => n == "monoA", "monoA");

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetQualifier(_ => true, typeof(Transform)); // overlaps on targets
            b.AddInjectionTargetMemberQualifier(n => n == "monoB", "monoB"); // disjoint on names

            Assert.AreNotEqual(a, b, "Both dimensions must overlap for equality.");
        }

        // ---------- HashSet behavior (dedupe) ----------

        [Test]
        public void HashSet_Dedupes_OnOverlappingTargetFilters()
        {
            HashSet<Binding> set = new();

            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetQualifier(_ => true, typeof(Transform));

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetQualifier(_ => true, typeof(Transform));
            b.AddInjectionTargetQualifier(_ => true, typeof(MeshRenderer));

            bool addedA = set.Add(a);
            bool addedB = set.Add(b);

            Assert.IsTrue(addedA);
            Assert.IsFalse(addedB, "Overlapping target filters (subset/superset) should be considered equal for HashSet dedupe.");
            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void HashSet_Dedupes_OnOverlappingMemberNameFilters()
        {
            HashSet<Binding> set = new();

            Binding a = MakeBinding(scopeA);
            a.AddInjectionTargetMemberQualifier(n => n == "monoA", "monoA");

            Binding b = MakeBinding(scopeA);
            b.AddInjectionTargetMemberQualifier(n => n == "monoA", "monoA");
            b.AddInjectionTargetMemberQualifier(n => n == "monoB", "monoB");

            bool addedA = set.Add(a);
            bool addedB = set.Add(b);

            Assert.IsTrue(addedA);
            Assert.IsFalse(addedB, "Overlapping member-name filters (subset/superset) should be considered equal for HashSet dedupe.");
            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void HashSet_TreatsDifferentCoreSignatureAsUnique()
        {
            HashSet<Binding> set = new();

            // Different scopes
            set.Add(MakeBinding(scopeA, id: "a"));
            set.Add(MakeBinding(scopeB, id: "a"));

            // Same scope but different IDs
            set.Add(MakeBinding(scopeA, id: "b"));

            Assert.AreEqual(3, set.Count);
        }

        // ---------- Null handling / interface vs concrete fallback ----------

        [Test]
        public void Binding_WithNull_InterfaceType_UsesConcreteType_ForEquality()
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
        public void Binding_WithNullConcreteType_IsNotEqualToValidBinding()
        {
            Binding a = new(null, typeof(InjectableComponent), scopeA);
            Binding b = new(null, null, scopeA);

            Assert.AreNotEqual(a, b);
        }

        // ---------- Helpers ----------

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