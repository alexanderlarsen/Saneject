using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEditor;
using UnityEngine;

namespace Tests.Editor.General
{
    public class ScopeContextIsolationTests : BaseBindingTest
    {
        private GameObject sceneRoot;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        //---------------------------------------------------------------------
        // S1: SceneScope injects, nested PrefabScope skipped
        //---------------------------------------------------------------------
        [Test]
        public void SceneInjection_SkipsNestedPrefabScope()
        {
            IgnoreErrorMessages();

            // Scene scope
            TestScope sceneScope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester sceneRequester = sceneRoot.AddComponent<ComponentRequester>();
            InjectableComponent sceneInjectable = sceneRoot.AddComponent<InjectableComponent>();
            BindComponent<IInjectable, InjectableComponent>(sceneScope).FromSelf();

            // Prefab instance nested under scene
            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            prefabInstance.transform.SetParent(sceneRoot.transform);

            TestScope prefabScope = prefabInstance.GetComponent<TestScope>();
            ComponentRequester prefabRequester = prefabInstance.GetComponent<ComponentRequester>();
            BindComponent<IInjectable, InjectableComponent>(prefabScope).FromSelf();

            DependencyInjector.InjectCurrentScene();

            Assert.NotNull(sceneInjectable);
            Assert.NotNull(sceneRequester.interfaceComponent);
            Assert.AreEqual(sceneInjectable, sceneRequester.interfaceComponent);

            Assert.IsNull(prefabRequester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // S2: SceneScope injects, multiple nested PrefabScopes skipped
        //---------------------------------------------------------------------
        [Test]
        public void SceneInjection_SkipsMultipleNestedPrefabScopes()
        {
            IgnoreErrorMessages();

            TestScope sceneScope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester sceneRequester = sceneRoot.AddComponent<ComponentRequester>();
            InjectableComponent sceneInjectable = sceneRoot.AddComponent<InjectableComponent>();
            BindComponent<IInjectable, InjectableComponent>(sceneScope).FromSelf();

            GameObject prefabInstanceA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            GameObject prefabInstanceB = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 2"));

            prefabInstanceA.transform.SetParent(sceneRoot.transform);
            prefabInstanceB.transform.SetParent(sceneRoot.transform);

            TestScope scopeA = prefabInstanceA.GetComponent<TestScope>();
            TestScope scopeB = prefabInstanceB.GetComponent<TestScope>();
            ComponentRequester requesterA = prefabInstanceA.GetComponent<ComponentRequester>();
            ComponentRequester requesterB = prefabInstanceB.GetComponent<ComponentRequester>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromSelf();
            BindComponent<IInjectable, InjectableComponent>(scopeB).FromSelf();

            DependencyInjector.InjectCurrentScene();

            Assert.NotNull(sceneInjectable);
            Assert.NotNull(sceneRequester.interfaceComponent);
            Assert.AreEqual(sceneInjectable, sceneRequester.interfaceComponent);

            Assert.IsNull(requesterA.interfaceComponent);
            Assert.IsNull(requesterB.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // S3: SceneScope nested under prefab is injected during scene injection
        //---------------------------------------------------------------------
        [Test]
        public void SceneInjection_InjectsSceneScopeNestedUnderPrefab()
        {
            IgnoreErrorMessages();

            // Prefab instance with its own scope (should be skipped)
            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope prefabScope = prefabInstance.GetComponent<TestScope>();
            ComponentRequester prefabRequester = prefabInstance.GetComponent<ComponentRequester>();
            BindComponent<IInjectable, InjectableComponent>(prefabScope).FromSelf();

            // Scene scope nested under prefab instance
            GameObject childSceneObject = new("SceneChildScope");
            childSceneObject.transform.SetParent(prefabInstance.transform);
            TestScope childSceneScope = childSceneObject.AddComponent<TestScope>();
            ComponentRequester childSceneRequester = childSceneObject.AddComponent<ComponentRequester>();
            InjectableComponent childSceneInjectable = childSceneObject.AddComponent<InjectableComponent>();
            BindComponent<IInjectable, InjectableComponent>(childSceneScope).FromSelf();

            // Scene root (without scope is fine, injection is scene-wide)
            sceneRoot.AddComponent<TestScope>();

            DependencyInjector.InjectCurrentScene();

            Assert.NotNull(childSceneInjectable);
            Assert.NotNull(childSceneRequester.interfaceComponent);
            Assert.AreEqual(childSceneInjectable, childSceneRequester.interfaceComponent);

            Assert.IsNull(prefabRequester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // S4: SceneScope hierarchy (parent-child-grandchild) all injected
        //---------------------------------------------------------------------
        [Test]
        public void SceneInjection_InjectsAllSceneScopesInHierarchy()
        {
            IgnoreErrorMessages();

            GameObject child = new("SceneChild");
            GameObject grandChild = new("SceneGrandChild");

            child.transform.SetParent(sceneRoot.transform);
            grandChild.transform.SetParent(child.transform);

            TestScope rootScope = sceneRoot.AddComponent<TestScope>();
            TestScope childScope = child.AddComponent<TestScope>();
            TestScope grandChildScope = grandChild.AddComponent<TestScope>();

            ComponentRequester rootRequester = sceneRoot.AddComponent<ComponentRequester>();
            ComponentRequester childRequester = child.AddComponent<ComponentRequester>();
            ComponentRequester grandChildRequester = grandChild.AddComponent<ComponentRequester>();

            InjectableComponent rootInjectable = sceneRoot.AddComponent<InjectableComponent>();
            InjectableComponent childInjectable = child.AddComponent<InjectableComponent>();
            InjectableComponent grandChildInjectable = grandChild.AddComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(rootScope).FromSelf();
            BindComponent<IInjectable, InjectableComponent>(childScope).FromSelf();
            BindComponent<IInjectable, InjectableComponent>(grandChildScope).FromSelf();

            DependencyInjector.InjectCurrentScene();

            Assert.NotNull(rootInjectable);
            Assert.NotNull(rootRequester.interfaceComponent);
            Assert.AreEqual(rootInjectable, rootRequester.interfaceComponent);

            Assert.NotNull(childInjectable);
            Assert.NotNull(childRequester.interfaceComponent);
            Assert.AreEqual(childInjectable, childRequester.interfaceComponent);

            Assert.NotNull(grandChildInjectable);
            Assert.NotNull(grandChildRequester.interfaceComponent);
            Assert.AreEqual(grandChildInjectable, grandChildRequester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // P1: PrefabScope injects its own hierarchy (same instance)
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInjection_InjectsOwnPrefabHierarchy()
        {
            IgnoreErrorMessages();

            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope rootScope = prefabInstance.GetComponent<TestScope>();
            ComponentRequester rootRequester = prefabInstance.GetComponent<ComponentRequester>();
            InjectableComponent rootInjectable = prefabInstance.GetComponent<InjectableComponent>();

            GameObject child = new("ChildScope");
            child.transform.SetParent(prefabInstance.transform);

            TestScope childScope = child.AddComponent<TestScope>();
            ComponentRequester childRequester = child.AddComponent<ComponentRequester>();
            InjectableComponent childInjectable = child.AddComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(rootScope).FromSelf();
            BindComponent<IInjectable, InjectableComponent>(childScope).FromSelf();

            DependencyInjector.InjectPrefab(rootScope);

            Assert.NotNull(rootInjectable);
            Assert.NotNull(rootRequester.interfaceComponent);
            Assert.AreEqual(rootInjectable, rootRequester.interfaceComponent);

            Assert.NotNull(childInjectable);
            Assert.NotNull(childRequester.interfaceComponent);
            Assert.AreEqual(childInjectable, childRequester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // P2: Prefab under scene injects normally, scene scope skipped
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInjection_InjectsPrefabUnderScene_SkipsSceneScope()
        {
            IgnoreErrorMessages();

            // Scene scope that should be ignored during prefab injection
            TestScope sceneScope = sceneRoot.AddComponent<TestScope>();
            ComponentRequester sceneRequester = sceneRoot.AddComponent<ComponentRequester>();
            InjectableComponent sceneInjectable = sceneRoot.AddComponent<InjectableComponent>();
            BindComponent<IInjectable, InjectableComponent>(sceneScope).FromSelf();

            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            prefabInstance.transform.SetParent(sceneRoot.transform);

            TestScope prefabScope = prefabInstance.GetComponent<TestScope>();
            ComponentRequester prefabRequester = prefabInstance.GetComponent<ComponentRequester>();
            InjectableComponent prefabInjectable = prefabInstance.GetComponent<InjectableComponent>();
            BindComponent<IInjectable, InjectableComponent>(prefabScope).FromSelf();

            DependencyInjector.InjectPrefab(prefabScope);

            Assert.NotNull(prefabInjectable);
            Assert.NotNull(prefabRequester.interfaceComponent);
            Assert.AreEqual(prefabInjectable, prefabRequester.interfaceComponent);

            Assert.IsNull(sceneRequester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // P3: Prefab injection skips SceneScope below prefab
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInjection_SkipsSceneScopeNestedUnderPrefab()
        {
            IgnoreErrorMessages();

            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope prefabScope = prefabInstance.GetComponent<TestScope>();
            ComponentRequester prefabRequester = prefabInstance.GetComponent<ComponentRequester>();
            InjectableComponent prefabInjectable = prefabInstance.GetComponent<InjectableComponent>();
            BindComponent<IInjectable, InjectableComponent>(prefabScope).FromSelf();

            GameObject sceneChild = new("SceneChildScope");
            sceneChild.transform.SetParent(prefabInstance.transform);

            TestScope sceneChildScope = sceneChild.AddComponent<TestScope>();
            ComponentRequester sceneChildRequester = sceneChild.AddComponent<ComponentRequester>();
            InjectableComponent sceneChildInjectable = sceneChild.AddComponent<InjectableComponent>();
            BindComponent<IInjectable, InjectableComponent>(sceneChildScope).FromSelf();

            DependencyInjector.InjectPrefab(prefabScope);

            Assert.NotNull(prefabInjectable);
            Assert.NotNull(prefabRequester.interfaceComponent);
            Assert.AreEqual(prefabInjectable, prefabRequester.interfaceComponent);

            Assert.IsNull(sceneChildRequester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // P4: Prefab injection skips nested different prefab instance
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInjection_SkipsNestedDifferentPrefabInstance()
        {
            IgnoreErrorMessages();

            GameObject parentPrefab = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope parentScope = parentPrefab.GetComponent<TestScope>();
            ComponentRequester parentRequester = parentPrefab.GetComponent<ComponentRequester>();
            InjectableComponent parentInjectable = parentPrefab.GetComponent<InjectableComponent>();
            BindComponent<IInjectable, InjectableComponent>(parentScope).FromSelf();

            GameObject childPrefab = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 2"));

            childPrefab.transform.SetParent(parentPrefab.transform);

            TestScope childScope = childPrefab.GetComponent<TestScope>();
            ComponentRequester childRequester = childPrefab.GetComponent<ComponentRequester>();
            BindComponent<IInjectable, InjectableComponent>(childScope).FromSelf();

            DependencyInjector.InjectPrefab(parentScope);

            Assert.NotNull(parentInjectable);
            Assert.NotNull(parentRequester.interfaceComponent);
            Assert.AreEqual(parentInjectable, parentRequester.interfaceComponent);

            Assert.IsNull(childRequester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // P5: Injecting nested prefab instance only injects that instance
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInjection_InjectsOnlyNestedPrefabInstance()
        {
            IgnoreErrorMessages();

            GameObject parentPrefab = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope parentScope = parentPrefab.GetComponent<TestScope>();
            ComponentRequester parentRequester = parentPrefab.GetComponent<ComponentRequester>();
            BindComponent<IInjectable, InjectableComponent>(parentScope).FromSelf();

            GameObject childPrefab = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 2"));

            childPrefab.transform.SetParent(parentPrefab.transform);

            TestScope childScope = childPrefab.GetComponent<TestScope>();
            ComponentRequester childRequester = childPrefab.GetComponent<ComponentRequester>();
            InjectableComponent childInjectable = childPrefab.GetComponent<InjectableComponent>();
            BindComponent<IInjectable, InjectableComponent>(childScope).FromSelf();

            DependencyInjector.InjectPrefab(childScope);

            Assert.NotNull(childInjectable);
            Assert.NotNull(childRequester.interfaceComponent);
            Assert.AreEqual(childInjectable, childRequester.interfaceComponent);

            Assert.IsNull(parentRequester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // M1: Two instances of same prefab, injecting one skips the other
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInstanceIsolation_SamePrefabSiblings()
        {
            IgnoreErrorMessages();

            GameObject instance1 = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            GameObject instance2 = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            instance1.transform.SetParent(sceneRoot.transform);
            instance2.transform.SetParent(sceneRoot.transform);

            TestScope scope1 = instance1.GetComponent<TestScope>();
            TestScope scope2 = instance2.GetComponent<TestScope>();

            ComponentRequester requester1 = instance1.GetComponent<ComponentRequester>();
            ComponentRequester requester2 = instance2.GetComponent<ComponentRequester>();

            InjectableComponent injectable1 = instance1.GetComponent<InjectableComponent>();
            InjectableComponent injectable2 = instance2.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scope1).FromSelf();
            BindComponent<IInjectable, InjectableComponent>(scope2).FromSelf();

            DependencyInjector.InjectPrefab(scope1);

            Assert.NotNull(injectable1);
            Assert.NotNull(requester1.interfaceComponent);
            Assert.AreEqual(injectable1, requester1.interfaceComponent);

            Assert.IsNull(requester2.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // M2: Two instances of different prefabs, injecting one skips the other
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInstanceIsolation_DifferentPrefabs()
        {
            IgnoreErrorMessages();

            GameObject instanceA = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            GameObject instanceB = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 2"));

            instanceA.transform.SetParent(sceneRoot.transform);
            instanceB.transform.SetParent(sceneRoot.transform);

            TestScope scopeA = instanceA.GetComponent<TestScope>();
            TestScope scopeB = instanceB.GetComponent<TestScope>();

            ComponentRequester requesterA = instanceA.GetComponent<ComponentRequester>();
            ComponentRequester requesterB = instanceB.GetComponent<ComponentRequester>();

            InjectableComponent injectableA = instanceA.GetComponent<InjectableComponent>();
            InjectableComponent injectableB = instanceB.GetComponent<InjectableComponent>();

            BindComponent<IInjectable, InjectableComponent>(scopeA).FromSelf();
            BindComponent<IInjectable, InjectableComponent>(scopeB).FromSelf();

            DependencyInjector.InjectPrefab(scopeA);

            Assert.NotNull(injectableA);
            Assert.NotNull(requesterA.interfaceComponent);
            Assert.AreEqual(injectableA, requesterA.interfaceComponent);

            Assert.IsNull(requesterB.interfaceComponent);

            // sanity: B still injectable if injected explicitly
            requesterB.interfaceComponent = null;
            DependencyInjector.InjectPrefab(scopeB);

            Assert.NotNull(injectableB);
            Assert.NotNull(requesterB.interfaceComponent);
            Assert.AreEqual(injectableB, requesterB.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // M3a: Same-prefab nested instance, injecting parent skips child
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInstanceIsolation_NestedSamePrefab_ParentInject()
        {
            IgnoreErrorMessages();

            GameObject parentInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope parentScope = parentInstance.GetComponent<TestScope>();
            ComponentRequester parentRequester = parentInstance.GetComponent<ComponentRequester>();
            InjectableComponent parentInjectable = parentInstance.GetComponent<InjectableComponent>();
            BindComponent<IInjectable, InjectableComponent>(parentScope).FromSelf();

            GameObject childInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            childInstance.transform.SetParent(parentInstance.transform);

            TestScope childScope = childInstance.GetComponent<TestScope>();
            ComponentRequester childRequester = childInstance.GetComponent<ComponentRequester>();
            BindComponent<IInjectable, InjectableComponent>(childScope).FromSelf();

            DependencyInjector.InjectPrefab(parentScope);

            Assert.NotNull(parentInjectable);
            Assert.NotNull(parentRequester.interfaceComponent);
            Assert.AreEqual(parentInjectable, parentRequester.interfaceComponent);

            Assert.IsNull(childRequester.interfaceComponent);
        }

        //---------------------------------------------------------------------
        // M3b: Same-prefab nested instance, injecting child skips parent
        //---------------------------------------------------------------------
        [Test]
        public void PrefabInstanceIsolation_NestedSamePrefab_ChildInject()
        {
            IgnoreErrorMessages();

            GameObject parentInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            TestScope parentScope = parentInstance.GetComponent<TestScope>();
            ComponentRequester parentRequester = parentInstance.GetComponent<ComponentRequester>();
            BindComponent<IInjectable, InjectableComponent>(parentScope).FromSelf();

            GameObject childInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/PrefabWithScopeRequesterAndInjectable 1"));

            childInstance.transform.SetParent(parentInstance.transform);

            TestScope childScope = childInstance.GetComponent<TestScope>();
            ComponentRequester childRequester = childInstance.GetComponent<ComponentRequester>();
            InjectableComponent childInjectable = childInstance.GetComponent<InjectableComponent>();
            BindComponent<IInjectable, InjectableComponent>(childScope).FromSelf();

            DependencyInjector.InjectPrefab(childScope);

            Assert.NotNull(childInjectable);
            Assert.NotNull(childRequester.interfaceComponent);
            Assert.AreEqual(childInjectable, childRequester.interfaceComponent);

            Assert.IsNull(parentRequester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            sceneRoot = new GameObject("SceneRoot");
        }
    }
}