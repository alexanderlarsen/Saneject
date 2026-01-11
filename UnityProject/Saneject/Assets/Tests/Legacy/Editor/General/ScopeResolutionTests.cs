using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Component;
using UnityEditor;
using UnityEngine;
using ComponentRequester = Tests.Legacy.Runtime.Component.ComponentRequester;

namespace Tests.Legacy.Editor.General
{
    public class ScopeResolutionTests : BaseBindingTest
    {
        private GameObject root, child, grandchild;

        [Test]
        public void UsesLowerLevelScope_WhenMultipleScopesBindSameType()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope rootScope = root.AddComponent<TestScope>();
            TestScope childScope = child.AddComponent<TestScope>();
            ComponentRequester requester = grandchild.AddComponent<ComponentRequester>();

            InjectableComponent rootComponent = root.AddComponent<InjectableComponent>();
            InjectableComponent childComponent = child.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(rootScope).FromSelf();
            BindComponent<InjectableComponent>(childScope).FromSelf();

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.NotNull(requester.concreteComponent);
            Assert.AreEqual(childComponent, requester.concreteComponent);
        }

        [Test]
        public void FallsBackToParentScope_WhenNotBoundLocally()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope rootScope = root.AddComponent<TestScope>();
            TestScope childScope = child.AddComponent<TestScope>();
            ComponentRequester requester = grandchild.AddComponent<ComponentRequester>();

            InjectableComponent rootComponent = root.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(rootScope).FromSelf();

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.NotNull(requester.concreteComponent);
            Assert.AreEqual(rootComponent, requester.concreteComponent);
        }

        [Test]
        public void SceneInjection_SkipsPrefabs()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();

            GameObject prefab = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Test/Prefab 1")) as GameObject;
            ComponentRequester prefabRequester = prefab.GetComponent<ComponentRequester>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromSelf();

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(prefabRequester.concreteComponent);
            Assert.IsNull(prefabRequester.interfaceComponent);
        }

        [Test]
        public void InjectPrefab_ExplicitlyWithPrefabInjection()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add prefab root with requester
            GameObject prefab = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Test/Prefab 1")) as GameObject;
            TestScope scope = prefab.AddComponent<TestScope>();
            ComponentRequester requester = prefab.AddComponent<ComponentRequester>();
            InjectableComponent component = prefab.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromSelf();

            // Inject
            DependencyInjector.InjectPrefab(scope);

            // Assert
            Assert.AreEqual(component, requester.concreteComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
            child = new GameObject();
            grandchild = new GameObject();

            child.transform.SetParent(root.transform);
            grandchild.transform.SetParent(child.transform);
        }
    }
}