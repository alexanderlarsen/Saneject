using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Plugins.Saneject.Legacy.Runtime.Attributes;
using Plugins.Saneject.Legacy.Runtime.Bindings;
using Plugins.Saneject.Legacy.Runtime.Scopes;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Asset;
using Tests.Legacy.Runtime.Component;
using UnityEngine;

namespace Tests.Legacy.Editor.Bindings.Misc
{
    public class BindingIsUsedTest : BaseBindingTest
    {
        private GameObject root, child;

        [Test]
        public void BindComponent_IsUsedAfterInjection()
        {
            IgnoreErrorMessages();

            // Add components
            InjectableComponent injectable = child.AddComponent<InjectableComponent>();
            ComponentConcreteOnlyRequester requester = child.AddComponent<ComponentConcreteOnlyRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up binding
            BindComponent<InjectableComponent>(scope).FromRootDescendants();

            // Grab binding before injection
            Binding binding = GetBinding(scope);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsTrue(binding.IsUsed, "Component binding should be marked as used after injection.");
            Assert.AreEqual(injectable, requester.concreteComponent);
        }

        [Test]
        public void BindAsset_IsUsedAfterInjection()
        {
            IgnoreErrorMessages();

            // Add components
            AssetConcreteOnlyRequester requester = root.AddComponent<AssetConcreteOnlyRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up binding
            BindAsset<InjectableScriptableObject>(scope)
                .FromResources("Test/InjectableScriptableObject 1");

            // Grab binding before injection
            Binding binding = GetBinding(scope);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsTrue(binding.IsUsed, "Asset binding should be marked as used after injection.");
            Assert.IsNotNull(requester.concreteAsset);
        }

        [Test]
        public void BindGlobal_IsUsedAfterInjection()
        {
            IgnoreErrorMessages();

            // Add components
            InjectableComponent injectable = root.AddComponent<InjectableComponent>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up binding
            BindGlobal<InjectableComponent>(scope).FromInstance(injectable);

            // Grab binding before injection
            Binding binding = GetBinding(scope);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert only binding usage (global bindings don’t inject into requesters directly)
            Assert.IsTrue(binding.IsUsed, "Global binding should be marked as used after injection.");
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
            child = new GameObject("Child");
            child.transform.SetParent(root.transform);
        }

        private static Binding GetBinding(TestScope scope)
        {
            FieldInfo field = typeof(Scope)
                .GetField("unvalidatedBindings", BindingFlags.NonPublic | BindingFlags.Instance);

            IEnumerable list = (IEnumerable)field.GetValue(scope);
            Binding binding = list.Cast<Binding>().FirstOrDefault();
            Assert.IsNotNull(binding, "No bindings found in unvalidatedBindings before injection.");
            return binding;
        }
    }

    // Minimal requesters for test isolation
    public class ComponentConcreteOnlyRequester : MonoBehaviour
    {
        [Inject]
        public InjectableComponent concreteComponent;
    }

    public class AssetConcreteOnlyRequester : MonoBehaviour
    {
        [Inject]
        public InjectableScriptableObject concreteAsset;
    }
}