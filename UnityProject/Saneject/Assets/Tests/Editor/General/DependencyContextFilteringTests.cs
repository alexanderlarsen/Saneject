using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Settings;
using Tests.Runtime;
using Tests.Runtime.Asset;
using Tests.Runtime.Component;
using UnityEditor;
using UnityEngine;

namespace Tests.Editor.General
{
    public class DependencyContextFilteringTests : BaseBindingTest
    {
        private bool prevFilter;
        private GameObject sceneRoot;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            prevFilter = UserSettings.UseContextIsolation;
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            UserSettings.UseContextIsolation = prevFilter;
        }

        //---------------------------------------------------------------------
        // 1. SCENE → SCENE (allowed)
        //---------------------------------------------------------------------
        [Test]
        public void SceneAllowsSceneDependency()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();
            requester.interfaceComponent = null;
            InjectableComponent injectable = sceneRoot.AddComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scope).FromSelf();

            DependencyInjector.InjectCurrentScene();

            Assert.NotNull(injectable);
            Assert.NotNull(requester.interfaceComponent);
            Assert.AreEqual(injectable, requester.interfaceComponent);
        }

        [Test]
        public void SceneAllowsSceneDependency_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();
            requester.interfaceComponent = null;
            InjectableComponent injectable = sceneRoot.AddComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scope).FromSelf();

            DependencyInjector.InjectCurrentScene();

            Assert.NotNull(injectable);
            Assert.NotNull(requester.interfaceComponent);
            Assert.AreEqual(injectable, requester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // 2. SCENE → DIFFERENT SCENE (simulated rejection)
        //---------------------------------------------------------------------
        // [Test]
        // public void SceneRejectsObjectFromDifferentScene()
        // {
        //     IgnoreErrorMessages();
        //     UserSettings.FilterBySameContext = true;
        //
        //     TestScope scope = sceneRoot.AddComponent<TestScope>();
        //     ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();
        //     requester.interfaceComponent = null;
        //
        //     // Create another scene
        //     Scene otherScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        //
        //     GameObject otherSceneGameObject = new GameObject("OtherSceneGameObject");
        //     InjectableComponent injectable = otherSceneGameObject.AddComponent<InjectableComponent>();
        //     SceneManager.MoveGameObjectToScene(otherSceneGameObject, otherScene);
        //
        //     // Bind instance from other scene
        //     BindComponent<IInjectable, InjectableComponent>(scope).FromInstance(injectable);
        //
        //     DependencyInjector.InjectCurrentScene();
        //
        //     Assert.IsNull(requester.interfaceComponent);
        // }

        //---------------------------------------------------------------------
        // 3. SCENE → PREFAB INSTANCE (rejected)
        //---------------------------------------------------------------------
        [Test]
        public void SceneRejectsPrefabInstance()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();
            requester.interfaceComponent = null;

            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            InjectableComponent injectable = prefabInstance.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scope).FromInstance(injectable);

            DependencyInjector.InjectCurrentScene();

            Assert.IsNull(requester.interfaceComponent);
        }

        [Test]
        public void SceneAllowsPrefabInstance_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();
            requester.interfaceComponent = null;

            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            InjectableComponent injectable = prefabInstance.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scope).FromInstance(injectable);

            DependencyInjector.InjectCurrentScene();

            Assert.AreEqual(injectable, requester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // 4. SCENE → PREFAB ASSET (rejected)
        //---------------------------------------------------------------------
        [Test]
        public void SceneRejectsPrefabAsset()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();
            requester.interfaceComponent = null;

            GameObject prefabAsset = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1");
            InjectableComponent injectable = prefabAsset.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scope).FromInstance(injectable);

            DependencyInjector.InjectCurrentScene();

            Assert.IsNull(requester.interfaceComponent);
        }

        [Test]
        public void SceneAllowsPrefabAsset_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();
            requester.interfaceComponent = null;

            GameObject prefabAsset = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1");
            InjectableComponent injectable = prefabAsset.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scope).FromInstance(injectable);

            DependencyInjector.InjectCurrentScene();

            Assert.AreEqual(injectable, requester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // 5. PREFAB A → PREFAB A (allowed)
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInstanceAllowsSameInstanceDependency()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            GameObject prefabA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope scopeA = prefabA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            InjectableComponent injectableA = prefabA.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromSelf();

            DependencyInjector.InjectPrefab(scopeA);

            Assert.NotNull(injectableA);
            Assert.NotNull(requesterA.interfaceComponent);
            Assert.AreEqual(injectableA, requesterA.interfaceComponent);
        }

        [Test]
        public void PrefabInstanceAllowsSameInstanceDependency_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            GameObject prefabA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope scopeA = prefabA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            InjectableComponent injectableA = prefabA.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromSelf();

            DependencyInjector.InjectPrefab(scopeA);

            Assert.AreEqual(injectableA, requesterA.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // 6. PREFAB A → PREFAB B (rejected)
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInstanceRejectsOtherPrefabInstance()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            GameObject prefabA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            GameObject prefabB = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope scopeA = prefabA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            InjectableComponent injectableB = prefabB.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromInstance(injectableB);

            DependencyInjector.InjectPrefab(scopeA);

            Assert.IsNull(requesterA.interfaceComponent);
        }

        [Test]
        public void PrefabInstanceAllowsOtherPrefabInstance_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            GameObject prefabA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            GameObject prefabB = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope scopeA = prefabA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            InjectableComponent injectableB = prefabB.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromInstance(injectableB);

            DependencyInjector.InjectPrefab(scopeA);

            Assert.AreEqual(injectableB, requesterA.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // 7. PREFAB A → SCENE (rejected)
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInstanceRejectsSceneObject()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            GameObject prefabA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope scopeA = prefabA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            InjectableComponent sceneInjectable = sceneRoot.AddComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromInstance(sceneInjectable);

            DependencyInjector.InjectPrefab(scopeA);

            Assert.IsNull(requesterA.interfaceComponent);
        }

        [Test]
        public void PrefabInstanceAllowsSceneObject_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            GameObject prefabA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope scopeA = prefabA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            InjectableComponent sceneInjectable = sceneRoot.AddComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromInstance(sceneInjectable);

            DependencyInjector.InjectPrefab(scopeA);

            Assert.AreEqual(sceneInjectable, requesterA.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // 8. PREFAB A → PREFAB ASSET (rejected)
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInstanceRejectsPrefabAsset()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            GameObject prefabA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope scopeA = prefabA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            GameObject prefabAsset = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1");
            InjectableComponent injectableAsset = prefabAsset.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromInstance(injectableAsset);

            DependencyInjector.InjectPrefab(scopeA);

            Assert.IsNull(requesterA.interfaceComponent);
        }

        [Test]
        public void PrefabInstanceAllowsPrefabAsset_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            GameObject prefabA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope scopeA = prefabA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            GameObject prefabAsset = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1");
            InjectableComponent injectableAsset = prefabAsset.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromInstance(injectableAsset);

            DependencyInjector.InjectPrefab(scopeA);

            Assert.AreEqual(injectableAsset, requesterA.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // 9. PREFAB ASSET → SAME PREFAB ASSET (allowed)
        //---------------------------------------------------------------------
        [Test]
        public void PrefabAssetAllowsSameAssetDependency()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            GameObject prefabAsset = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1");
            TestScope scopeAsset = prefabAsset.GetComponent<TestScope>();
            ComponentRequester requesterAsset = prefabAsset.GetComponent<ComponentRequester>();
            requesterAsset.interfaceComponent = null;

            InjectableComponent injectable = prefabAsset.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeAsset).FromSelf();

            DependencyInjector.InjectPrefab(scopeAsset);

            Assert.NotNull(injectable);
            Assert.NotNull(requesterAsset.interfaceComponent);
            Assert.AreEqual(injectable, requesterAsset.interfaceComponent);
        }

        [Test]
        public void PrefabAssetAllowsSameAssetDependency_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            GameObject prefabAsset = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1");
            TestScope scopeAsset = prefabAsset.GetComponent<TestScope>();
            ComponentRequester requesterAsset = prefabAsset.GetComponent<ComponentRequester>();
            requesterAsset.interfaceComponent = null;

            InjectableComponent injectable = prefabAsset.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeAsset).FromSelf();

            DependencyInjector.InjectPrefab(scopeAsset);

            Assert.AreEqual(injectable, requesterAsset.interfaceComponent);
        }

        //---------------------------------------------------------------------
        //10. PREFAB ASSET → DIFFERENT PREFAB ASSET (rejected)
        //---------------------------------------------------------------------
        [Test]
        public void PrefabAssetRejectsOtherPrefabAsset()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            GameObject prefabAssetA = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1");
            GameObject prefabAssetB = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 2");

            TestScope scopeA = prefabAssetA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabAssetA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            InjectableComponent bInjectable = prefabAssetB.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromInstance(bInjectable);

            DependencyInjector.InjectPrefab(scopeA);

            Assert.IsNull(requesterA.interfaceComponent);
        }

        [Test]
        public void PrefabAssetAllowsOtherPrefabAsset_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            GameObject prefabAssetA = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1");
            GameObject prefabAssetB = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 2");

            TestScope scopeA = prefabAssetA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabAssetA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            InjectableComponent bInjectable = prefabAssetB.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromInstance(bInjectable);

            DependencyInjector.InjectPrefab(scopeA);

            Assert.AreEqual(bInjectable, requesterA.interfaceComponent);
        }

        //---------------------------------------------------------------------
        //11. PREFAB ASSET → PREFAB INSTANCE (rejected)
        //---------------------------------------------------------------------
        [Test]
        public void PrefabAssetRejectsPrefabInstance()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            GameObject prefabAsset = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1");
            TestScope assetScope = prefabAsset.GetComponent<TestScope>();
            ComponentRequester requester = prefabAsset.GetComponent<ComponentRequester>();
            requester.interfaceComponent = null;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            InjectableComponent instanceInjectable = instance.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(assetScope).FromInstance(instanceInjectable);

            DependencyInjector.InjectPrefab(assetScope);

            Assert.IsNull(requester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        //12. PREFAB ASSET → SCENE (rejected)
        //---------------------------------------------------------------------
        [Test]
        public void PrefabAssetRejectsSceneObject()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            GameObject prefabAsset = Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1");
            TestScope assetScope = prefabAsset.GetComponent<TestScope>();
            ComponentRequester requester = prefabAsset.GetComponent<ComponentRequester>();
            requester.interfaceComponent = null;

            InjectableComponent sceneInjectable = sceneRoot.AddComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(assetScope).FromInstance(sceneInjectable);

            DependencyInjector.InjectPrefab(assetScope);

            Assert.IsNull(requester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        //13. SCENE → SCRIPTABLE OBJECT (allowed)
        //---------------------------------------------------------------------
        [Test]
        public void SceneAllowsScriptableObjectDependency()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();
            requester.interfaceComponent = null;

            // Load the SO from Resources (implements IInjectable)
            InjectableScriptableObject so = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 1");
            Assert.NotNull(so);

            // Bind SO instance
            BindAsset<IInjectable, InjectableScriptableObject>(scope).FromInstance(so);

            DependencyInjector.InjectCurrentScene();

            Assert.NotNull(so);
            Assert.NotNull(requester.interfaceComponent);
            Assert.AreEqual(so, requester.interfaceComponent);
        }

        [Test]
        public void SceneAllowsScriptableObjectDependency_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester requester = sceneRoot.AddComponent<ComponentRequester>();
            requester.interfaceComponent = null;

            InjectableScriptableObject so =
                Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 1");

            BindAsset<IInjectable, InjectableScriptableObject>(scope).FromInstance(so);

            DependencyInjector.InjectCurrentScene();

            Assert.AreEqual(so, requester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        //14. PREFAB INSTANCE → SCRIPTABLE OBJECT (allowed)
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInstanceAllowsScriptableObjectDependency()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            // Instantiate prefab instance A
            GameObject prefabA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope scopeA = prefabA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            // Load the SO from Resources
            InjectableScriptableObject so = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 1");
            Assert.NotNull(so);

            // Bind SO to this prefab instance scope
            BindAsset<IInjectable, InjectableScriptableObject>(scopeA).FromInstance(so);

            DependencyInjector.InjectPrefab(scopeA);

            Assert.NotNull(requesterA.interfaceComponent);
            Assert.AreEqual(so, requesterA.interfaceComponent);
        }

        [Test]
        public void PrefabInstanceAllowsScriptableObjectDependency_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            GameObject prefabA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope scopeA = prefabA.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabA.GetComponent<ComponentRequester>();
            requesterA.interfaceComponent = null;

            InjectableScriptableObject so =
                Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 1");

            BindAsset<IInjectable, InjectableScriptableObject>(scopeA).FromInstance(so);

            DependencyInjector.InjectPrefab(scopeA);

            Assert.AreEqual(so, requesterA.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            sceneRoot = new GameObject("SceneRoot");
        }
    }
}