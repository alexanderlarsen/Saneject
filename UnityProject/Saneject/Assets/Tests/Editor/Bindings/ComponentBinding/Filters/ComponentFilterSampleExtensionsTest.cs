using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Filters
{
    public class ComponentFilterSampleExtensionsTests : BaseBindingTest
    {
        private GameObject root, childA, childB, childC;

        [Test]
        public void WhereIsActiveAndEnabled_FiltersDisabledBehaviour()
        {
            IgnoreErrorMessages();

            // Scope & requester live on root
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            // Two candidates: one enabled, one disabled
            InjectableComponent enabledComp = childA.AddComponent<InjectableComponent>();
            InjectableComponent disabledComp = childB.AddComponent<InjectableComponent>();
            disabledComp.enabled = false; // disabled behaviour
            childB.SetActive(true); // but GO still active
            // childA: enabled & active -> should win

            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereIsActiveAndEnabled(); // sample sugar

            DependencyInjector.InjectCurrentScene();

            Assert.AreEqual(enabledComp, requester.concreteComponent);
        }

        [Test]
        public void WhereParentNameIs_MatchesImmediateParentName()
        {
            IgnoreErrorMessages();

            // Make childA the parent of a nested candidate
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            GameObject nested = new("Nested");
            nested.transform.SetParent(childA.transform);
            InjectableComponent target = nested.AddComponent<InjectableComponent>();

            // Also add a distractor under childB
            GameObject other = new("Other");
            other.transform.SetParent(childB.transform);
            other.AddComponent<InjectableComponent>();

            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereParentNameIs("ChildA"); // sample sugar

            DependencyInjector.InjectCurrentScene();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void WhereAnyChildNameIs_PassesWhenCandidateHasMatchingChild()
        {
            IgnoreErrorMessages();

            // Candidate is root; it has ChildA and ChildB as children
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            InjectableComponent target = root.AddComponent<InjectableComponent>();

            // Ensure children are actually under root
            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);

            // This should include root only if it has a child named "ChildA"
            BindComponent<InjectableComponent>(scope)
                .FromScopeSelf()
                .WhereAnyChildNameIs("ChildA"); // sample sugar

            DependencyInjector.InjectCurrentScene();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void WhereAnyAncestorTagIs_MatchesBuiltInTagOnAncestorChain()
        {
            IgnoreErrorMessages();

            // Arrange a branch whose ancestor has tag "Untagged" (default)
            // and a distractor branch whose ancestors are non-matching tags.
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            // Make root a non-Untagged tag so it doesn't trivially satisfy both paths
            // (Unity has built-in tags like "Player", "Finish", etc.)
            root.tag = "Player";

            // Target path: ancestor "childA" remains "Untagged" (default)
            GameObject deep = new("Deep");
            deep.transform.SetParent(childA.transform);
            InjectableComponent target = deep.AddComponent<InjectableComponent>();

            // Distractor path: set both ancestors to "Player"
            childB.tag = "Player";
            GameObject deepOther = new("DeepOther");
            deepOther.transform.SetParent(childB.transform);
            deepOther.AddComponent<InjectableComponent>();

            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereAnyAncestorTagIs("Untagged"); // sample sugar

            DependencyInjector.InjectCurrentScene();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void WhereAnySiblingLayerIs_MatchesWhenAnySiblingIsOnLayer()
        {
            IgnoreErrorMessages();

            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            // Put target component on ChildB; mark ChildA as a UI-layer sibling
            InjectableComponent target = childB.AddComponent<InjectableComponent>();
            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer < 0) uiLayer = 5; // fallback to default UI index
            childA.layer = uiLayer;

            // Ensure they are siblings under root
            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);
            childC.transform.SetParent(root.transform);

            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereAnySiblingLayerIs(uiLayer); // sample sugar

            DependencyInjector.InjectCurrentScene();

            Assert.AreEqual(target, requester.concreteComponent);
        }

        protected override void CreateHierarchy()
        {
            // Base hierarchy; specific tests may re-parent as needed
            root = new GameObject("Root");
            childA = new GameObject("ChildA");
            childB = new GameObject("ChildB");
            childC = new GameObject("ChildC");

            // Default layout: children under root
            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);
            childC.transform.SetParent(root.transform);
        }
    }
}