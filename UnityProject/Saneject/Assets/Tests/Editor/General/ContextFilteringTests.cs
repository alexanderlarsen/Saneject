using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Settings;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEditor;
using UnityEngine;

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
            UserSettings.FilterBySameContext = prevFilterBySameContext;
        }

        [Test]
        public void RespectsContextFiltering_WhenEnabled()
        {
            IgnoreErrorMessages();
            UserSettings.FilterBySameContext = true;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();

            // Prefab instance with InjectableComponent
            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/Prefab 1")) as GameObject;

            InjectableComponent injectable = prefabInstance.AddComponent<InjectableComponent>();

            // Bind explicitly to that instance
            BindComponent<IInjectable, InjectableComponent>(scope).FromInstance(injectable);

            DependencyInjector.InjectSceneDependencies();

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

            // Prefab instance with InjectableComponent
            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/Prefab 1")) as GameObject;

            InjectableComponent injectable = prefabInstance.AddComponent<InjectableComponent>();

            // Bind explicitly to that instance
            BindComponent<IInjectable, InjectableComponent>(scope).FromInstance(injectable);

            DependencyInjector.InjectSceneDependencies();

            Assert.AreEqual(injectable, requester.interfaceComponent,
                "Scene requester should resolve from prefab instance when filtering is disabled.");
        }

        protected override void CreateHierarchy()
        {
            sceneRoot = new GameObject("SceneRoot");
        }
    }
}