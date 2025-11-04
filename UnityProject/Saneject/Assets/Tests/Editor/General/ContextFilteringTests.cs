using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Settings;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests.Editor.General
{
    public class ContextFilteringTests : BaseBindingTest
    {
        private GameObject sceneRoot;
        private bool prevFilterBySameContext;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            prevFilterBySameContext = UserSettings.FilterBySameContext;
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            UserSettings.FilterBySameContext = true;
        }

        [Test]
        public void RespectsContextFiltering_WhenEnabled()
        {
            IgnoreErrorMessages();
            UserSettings.FilterBySameContext = true;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();

            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/Prefab 2")) as GameObject;

            InjectableComponent injectable = prefabInstance.AddComponent<InjectableComponent>();

            // Bind the actual prefab instance explicitly
            BindComponent<IInjectable, InjectableComponent>(scope).FromInstance(injectable);

            DependencyInjector.InjectCurrentScene();

            Assert.IsNull(requester.interfaceComponent,
                "Scene requester should not resolve from prefab instance when filtering is enabled.");
        }

        [Test]
        public void AllowsCrossContext_WhenFilteringDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.FilterBySameContext = false;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();

            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/Prefab 2")) as GameObject;

            InjectableComponent injectable = prefabInstance.AddComponent<InjectableComponent>();

            // Bind the actual prefab instance explicitly
            BindComponent<IInjectable, InjectableComponent>(scope).FromInstance(injectable);

            DependencyInjector.InjectCurrentScene();

            Assert.AreEqual(injectable, requester.interfaceComponent,
                "Scene requester should resolve from prefab instance when filtering is disabled.");
        }

        [Test]
        public void RejectsDifferentPrefabInstances_WhenFilteringEnabled()
        {
            IgnoreErrorMessages();
            UserSettings.FilterBySameContext = true;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();

            GameObject prefab1 = PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/Prefab 2")) as GameObject;

            prefab1.AddComponent<InjectableComponent>();

            GameObject prefab2 = PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/Prefab 2")) as GameObject;

            InjectableComponent injectable2 = prefab2.AddComponent<InjectableComponent>();

            // Bind to a specific instance (different root than requester)
            BindComponent<IInjectable, InjectableComponent>(scope).FromInstance(injectable2);

            DependencyInjector.InjectCurrentScene();

            Assert.IsNull(requester.interfaceComponent,
                "Requester should not resolve from a different prefab instance when filtering is enabled.");
        }

        [Test]
        public void RejectsPrefabAsset_WhenFilteringEnabled()
        {
            IgnoreErrorMessages();
            UserSettings.FilterBySameContext = true;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();

            GameObject prefabAsset = Resources.Load<GameObject>("Test/Prefab 2");
            InjectableComponent injectableOnAsset = prefabAsset.AddComponent<InjectableComponent>();

            // Bind directly to the asset component (editor-only scenario)
            BindComponent<IInjectable, InjectableComponent>(scope).FromInstance(injectableOnAsset);

            DependencyInjector.InjectCurrentScene();

            Assert.IsNull(requester.interfaceComponent,
                "Requester should not resolve from a prefab asset when filtering is enabled.");
        }
        
        protected override void CreateHierarchy()
        {
            sceneRoot = new GameObject("SceneRoot");
        }
    }
}