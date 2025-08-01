using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.AssetBinding.Locators.Special
{
    public class FromInstanceTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_FromInstance()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();
            InjectableScriptableObject instance = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope).FromInstance(instance);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(instance, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_FromInstance()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();
            InjectableScriptableObject instance = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope).FromInstance(instance);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(instance, requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }
    }
}