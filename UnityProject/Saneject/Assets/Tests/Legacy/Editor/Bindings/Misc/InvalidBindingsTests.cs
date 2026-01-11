using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Legacy.Runtime.Bindings;
using Plugins.Saneject.Legacy.Runtime.Scopes;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Asset;
using Tests.Legacy.Runtime.Component;
using UnityEditor;
using UnityEngine;

namespace Tests.Legacy.Editor.Bindings.Misc
{
    public class InvalidBindingTests : BaseBindingTest
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
            const string id = "someId";
            Binding binding = new(typeof(IInjectable), typeof(InjectableComponent), dummyScope);
            binding.MarkComponentBinding();
            binding.MarkGlobal();
            binding.AddIdQualifier(fieldId => fieldId == id, id);
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

        [Test]
        public void GlobalBinding_OnPrefab_IsInvalid()
        {
            IgnoreErrorMessages();

            GameObject prefab = Resources.Load<GameObject>("Test/Prefab 1");
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            // Add components
            TestScope scope = instance.AddComponent<TestScope>();
            InjectableComponent injectableComponent = instance.AddComponent<InjectableComponent>();

            // Set up binding
            BindGlobal<InjectableComponent>(scope).FromInstance(injectableComponent);

            // Grab unvalidatedBindings directly via reflection
            FieldInfo field = typeof(Scope).GetField("unvalidatedBindings", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(field, "Field 'unvalidatedBindings' should not be null.");

            IEnumerable unvalidated = (IEnumerable)field.GetValue(scope);
            Binding binding = unvalidated.Cast<Binding>().FirstOrDefault();

            Assert.IsNotNull(binding, "Binding shouldn't be null.");
            Assert.IsFalse(BindingValidator.IsBindingValid(binding), "Global binding on prefab instance should be invalid.");
        }

        protected override void CreateHierarchy()
        {
        }
    }
}