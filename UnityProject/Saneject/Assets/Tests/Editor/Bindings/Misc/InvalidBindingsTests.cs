using NUnit.Framework;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using Tests.Runtime.Asset;
using Tests.Runtime.Component;
using UnityEngine;

namespace Tests.Editor.Bindings.Misc
{
    public class InvalidBindingTests : BaseTest
    {
        private Scope dummyScope;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            dummyScope = new GameObject("Scope").AddComponent<TestScope>();
        }

        [Test]
        public void AssetBinding_WithComponentType_IsInvalid()
        {
            IgnoreErrorMessages();

            Binding binding = new(null, typeof(Transform), dummyScope);
            binding.MarkAssetBinding();
            binding.SetLocator(_ => null);

            Assert.IsFalse(BindingValidator.IsBindingValid(binding), "Asset binding with Component type should be invalid.");
        }

        [Test]
        public void ComponentBinding_WithNonComponentType_IsInvalid()
        {
            IgnoreErrorMessages();

            Binding binding = new(typeof(IInjectable), typeof(ScriptableObject), dummyScope);
            binding.MarkComponentBinding();
            binding.SetLocator(_ => null); // Set to avoid no-locator fail

            Assert.IsFalse(BindingValidator.IsBindingValid(binding), "Component binding with non-Component type should be invalid.");
        }

        [Test]
        public void CollectionBinding_WithGlobal_IsInvalid()
        {
            IgnoreErrorMessages();

            Binding binding = new(typeof(IInjectable), typeof(InjectableComponent), dummyScope);
            binding.MarkComponentBinding();
            binding.MarkCollectionBinding();
            binding.MarkGlobal();
            binding.SetLocator(_ => null);

            Assert.IsFalse(BindingValidator.IsBindingValid(binding), "Global + collection binding should be invalid.");
        }

        [Test]
        public void GlobalBinding_WithId_IsInvalid()
        {
            IgnoreErrorMessages();

            Binding binding = new(typeof(IInjectable), typeof(InjectableComponent), dummyScope);
            binding.MarkComponentBinding();
            binding.MarkGlobal();
            binding.SetId("someId");
            binding.SetLocator(_ => null);

            Assert.IsFalse(BindingValidator.IsBindingValid(binding), "Global binding with ID should be invalid.");
        }

        [Test]
        public void InterfaceType_IsNotInterface_IsInvalid()
        {
            IgnoreErrorMessages();

            Binding binding = new(typeof(GameObject), typeof(GameObject), dummyScope);
            binding.MarkAssetBinding();
            binding.SetLocator(_ => null);

            Assert.IsFalse(BindingValidator.IsBindingValid(binding), "Binding with interfaceType not being interface should be invalid.");
        }

        [Test]
        public void Binding_WithNoLocator_IsInvalid()
        {
            IgnoreErrorMessages();

            Binding binding = new(typeof(IInjectable), typeof(InjectableComponent), dummyScope);
            binding.MarkComponentBinding();
            // No SetLocator

            Assert.IsFalse(BindingValidator.IsBindingValid(binding), "Binding without locator should be invalid.");
        }

        [Test]
        public void AssetBinding_WithoutLocator_IsInvalid()
        {
            IgnoreErrorMessages();

            Binding binding = new(typeof(IInjectable), typeof(InjectableScriptableObject), dummyScope);
            binding.MarkAssetBinding();
            // No SetLocator

            Assert.IsFalse(BindingValidator.IsBindingValid(binding), "Asset binding without locator should be invalid.");
        }

        [Test]
        public void CollectionAssetBinding_Global_IsInvalid()
        {
            IgnoreErrorMessages();

            Binding binding = new(typeof(IInjectable), typeof(InjectableScriptableObject), dummyScope);
            binding.MarkAssetBinding();
            binding.MarkCollectionBinding();
            binding.MarkGlobal();
            binding.SetLocator(_ => null);

            Assert.IsFalse(BindingValidator.IsBindingValid(binding), "Global asset collection binding should be invalid.");
        }

        [Test]
        public void CollectionBinding_WithInvalidConcreteType_IsInvalid()
        {
            IgnoreErrorMessages();

            Binding binding = new(typeof(IInjectable), typeof(ScriptableObject), dummyScope);
            binding.MarkComponentBinding();
            binding.MarkCollectionBinding();
            binding.SetLocator(_ => null);

            Assert.IsFalse(BindingValidator.IsBindingValid(binding), "Collection binding with invalid concrete type should be invalid.");
        }
    }
}