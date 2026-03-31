using System.Collections;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using Tests.Saneject.Fixtures.Scripts.RuntimeProxy.PlayMode;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Runtime
{
    public class RuntimeProxyPlayModeTests
    {
        private const string TestAssetFolder = "Assets/Tests/Saneject/Fixtures/RuntimeProxyPlayModeAssets";

        [SetUp]
        public void SetUp()
        {
            if (!AssetDatabase.IsValidFolder(TestAssetFolder))
                AssetDatabase.CreateFolder("Assets/Tests/Saneject/Fixtures", "RuntimeProxyPlayModeAssets");
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (Application.isPlaying)
                yield return new ExitPlayMode();

            Assert.That(Application.isPlaying, Is.False);

            string[] proxyTypeNames =
            {
                nameof(TestRuntimeProxy),
                nameof(GlobalScopeComponentDependencyRuntimeProxy),
                nameof(LoadedSceneComponentDependencyRuntimeProxy),
                nameof(PrefabComponentDependencyRuntimeProxy),
                nameof(NewGameObjectComponentDependencyRuntimeProxy)
            };

            foreach (string proxyTypeName in proxyTypeNames)
            {
                string[] assetGuids = AssetDatabase.FindAssets($"t:{proxyTypeName}");

                foreach (string guid in assetGuids)
                    AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
            }

            AssetDatabase.DeleteAsset(TestAssetFolder);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [UnityTest]
        public IEnumerator RuntimeProxy_ResolveMethods_RuntimeBehavior()
        {
            Assert.That(Application.isPlaying, Is.False);

            // Set up scene in edit mode
            TestScene scene = TestScene.Create(roots: 4, width: 1, depth: 2);
            TestScope globalScope = scene.Add<TestScope>("Root 1");
            GlobalScopeComponentDependency globalDependency = scene.Add<GlobalScopeComponentDependency>("Root 1");
            SingleInterfaceTarget globalTarget = scene.Add<SingleInterfaceTarget>("Root 1");
            TestScope anywhereScope = scene.Add<TestScope>("Root 2");
            LoadedSceneComponentDependency anywhereDependency = scene.Add<LoadedSceneComponentDependency>("Root 2/Child 1");
            SingleInterfaceTarget anywhereTarget = scene.Add<SingleInterfaceTarget>("Root 2");
            TestScope prefabScope = scene.Add<TestScope>("Root 3");
            SingleInterfaceTarget prefabTarget = scene.Add<SingleInterfaceTarget>("Root 3");
            TestScope newGameObjectScope = scene.Add<TestScope>("Root 4");
            SingleInterfaceTarget newGameObjectTarget = scene.Add<SingleInterfaceTarget>("Root 4");
            GameObject prefabRoot = new("Runtime Proxy Prefab");
            prefabRoot.AddComponent<PrefabComponentDependency>();
            string prefabPath = AssetDatabase.GenerateUniqueAssetPath($"{TestAssetFolder}/Runtime Proxy Prefab.prefab");
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            Object.DestroyImmediate(prefabRoot);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Bind in edit mode
            globalScope.BindGlobal<GlobalScopeComponentDependency>().FromSelf();

            globalScope.BindComponent<IDependency, GlobalScopeComponentDependency>()
                .FromRuntimeProxy()
                .FromGlobalScope();

            anywhereScope.BindComponent<IDependency, LoadedSceneComponentDependency>()
                .FromRuntimeProxy()
                .FromAnywhereInLoadedScenes();

            prefabScope.BindComponent<IDependency, PrefabComponentDependency>()
                .FromRuntimeProxy()
                .FromComponentOnPrefab(prefab, dontDestroyOnLoad: false)
                .AsTransient();

            newGameObjectScope.BindComponent<IDependency, NewGameObjectComponentDependency>()
                .FromRuntimeProxy()
                .FromNewComponentOnNewGameObject(dontDestroyOnLoad: false)
                .AsTransient();

            // Inject in edit mode
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert in edit mode
            Assert.That(globalDependency, Is.Not.Null);
            Assert.That(anywhereDependency, Is.Not.Null);
            Assert.That(prefab, Is.Not.Null);
            Assert.That(globalTarget.dependency, Is.InstanceOf<GlobalScopeComponentDependencyRuntimeProxy>());
            Assert.That(anywhereTarget.dependency, Is.InstanceOf<LoadedSceneComponentDependencyRuntimeProxy>());
            Assert.That(prefabTarget.dependency, Is.InstanceOf<PrefabComponentDependencyRuntimeProxy>());
            Assert.That(newGameObjectTarget.dependency, Is.InstanceOf<NewGameObjectComponentDependencyRuntimeProxy>());

            // Save scene in edit mode
            scene.SaveToDisk(TestAssetFolder);

            // Enter play mode
            yield return new EnterPlayMode();
            Assert.That(Application.isPlaying, Is.True);

            // Wait one frame in play mode
            yield return new WaitForEndOfFrame();

            // Find dependencies in play mode
            GameObject runtimeGlobalRoot = GameObject.Find("Root 1");
            GameObject runtimeAnywhereRoot = GameObject.Find("Root 2");
            GameObject runtimePrefabRoot = GameObject.Find("Root 3");
            GameObject runtimeNewGameObjectRoot = GameObject.Find("Root 4");
            GlobalScopeComponentDependency runtimeGlobalDependency = runtimeGlobalRoot.GetComponent<GlobalScopeComponentDependency>();
            SingleInterfaceTarget runtimeGlobalTarget = runtimeGlobalRoot.GetComponent<SingleInterfaceTarget>();
            LoadedSceneComponentDependency runtimeAnywhereDependency = runtimeAnywhereRoot.transform.GetChild(0).GetComponent<LoadedSceneComponentDependency>();
            SingleInterfaceTarget runtimeAnywhereTarget = runtimeAnywhereRoot.GetComponent<SingleInterfaceTarget>();
            SingleInterfaceTarget runtimePrefabTarget = runtimePrefabRoot.GetComponent<SingleInterfaceTarget>();
            SingleInterfaceTarget runtimeNewGameObjectTarget = runtimeNewGameObjectRoot.GetComponent<SingleInterfaceTarget>();
            PrefabComponentDependency[] runtimePrefabDependencies = Object.FindObjectsByType<PrefabComponentDependency>(FindObjectsSortMode.None);
            NewGameObjectComponentDependency[] runtimeNewGameObjectDependencies = Object.FindObjectsByType<NewGameObjectComponentDependency>(FindObjectsSortMode.None);

            // Assert in play mode
            Assert.That(runtimeGlobalRoot, Is.Not.Null);
            Assert.That(runtimeAnywhereRoot, Is.Not.Null);
            Assert.That(runtimePrefabRoot, Is.Not.Null);
            Assert.That(runtimeNewGameObjectRoot, Is.Not.Null);
            Assert.That(runtimeGlobalDependency, Is.Not.Null);
            Assert.That(runtimeAnywhereDependency, Is.Not.Null);
            Assert.That(runtimeGlobalTarget, Is.Not.Null);
            Assert.That(runtimeAnywhereTarget, Is.Not.Null);
            Assert.That(runtimePrefabTarget, Is.Not.Null);
            Assert.That(runtimeNewGameObjectTarget, Is.Not.Null);
            Assert.That(runtimePrefabDependencies, Has.Length.EqualTo(1));
            Assert.That(runtimeNewGameObjectDependencies, Has.Length.EqualTo(1));
            CollectionAssert.AllItemsAreNotNull(runtimePrefabDependencies);
            CollectionAssert.AllItemsAreNotNull(runtimeNewGameObjectDependencies);
            Assert.That(runtimeGlobalTarget.dependency, Is.Not.InstanceOf<RuntimeProxyBase>());
            Assert.That(runtimeAnywhereTarget.dependency, Is.Not.InstanceOf<RuntimeProxyBase>());
            Assert.That(runtimePrefabTarget.dependency, Is.Not.InstanceOf<RuntimeProxyBase>());
            Assert.That(runtimeNewGameObjectTarget.dependency, Is.Not.InstanceOf<RuntimeProxyBase>());
            Assert.That(runtimeGlobalTarget.dependency as Object, Is.EqualTo(runtimeGlobalDependency));
            Assert.That(runtimeAnywhereTarget.dependency as Object, Is.EqualTo(runtimeAnywhereDependency));
            Assert.That(runtimePrefabTarget.dependency as Object, Is.EqualTo(runtimePrefabDependencies[0]));
            Assert.That(runtimeNewGameObjectTarget.dependency as Object, Is.EqualTo(runtimeNewGameObjectDependencies[0]));
            Assert.That(runtimePrefabTarget.dependency as Object, Is.Not.EqualTo(runtimeNewGameObjectTarget.dependency as Object));
            Assert.That(GlobalScope.IsRegistered<GlobalScopeComponentDependency>(), Is.True);
            Assert.That(GlobalScope.GetComponent<GlobalScopeComponentDependency>(), Is.EqualTo(runtimeGlobalDependency));

            // Clear global scope in play mode
            GlobalScope.Clear();
        }

        [UnityTest]
        public IEnumerator RuntimeProxy_InstanceModes_RuntimeBehavior()
        {
            Assert.That(Application.isPlaying, Is.False);

            // Set up scene in edit mode
            TestScene scene = TestScene.Create(roots: 2, width: 1, depth: 2);
            TestScope transientScope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget transientFirstTarget = scene.Add<SingleInterfaceTarget>("Root 1");
            SingleInterfaceTarget transientSecondTarget = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");
            TestScope singletonScope = scene.Add<TestScope>("Root 2");
            SingleInterfaceTarget singletonFirstTarget = scene.Add<SingleInterfaceTarget>("Root 2");
            SingleInterfaceTarget singletonSecondTarget = scene.Add<SingleInterfaceTarget>("Root 2/Child 1");

            // Bind in edit mode
            transientScope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy()
                .FromNewComponentOnNewGameObject(dontDestroyOnLoad: false)
                .AsTransient();

            singletonScope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy()
                .FromNewComponentOnNewGameObject(dontDestroyOnLoad: false)
                .AsSingleton();

            // Inject in edit mode
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert in edit mode
            Assert.That(transientFirstTarget.dependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(transientSecondTarget.dependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(singletonFirstTarget.dependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(singletonSecondTarget.dependency, Is.InstanceOf<TestRuntimeProxy>());

            // Save scene in edit mode
            scene.SaveToDisk(TestAssetFolder);

            // Enter play mode
            yield return new EnterPlayMode();
            Assert.That(Application.isPlaying, Is.True);

            // Wait one frame in play mode
            yield return new WaitForEndOfFrame();

            // Find dependencies in play mode
            GameObject runtimeTransientRoot = GameObject.Find("Root 1");
            GameObject runtimeSingletonRoot = GameObject.Find("Root 2");
            SingleInterfaceTarget runtimeTransientFirstTarget = runtimeTransientRoot.GetComponent<SingleInterfaceTarget>();
            SingleInterfaceTarget runtimeTransientSecondTarget = runtimeTransientRoot.transform.GetChild(0).GetComponent<SingleInterfaceTarget>();
            SingleInterfaceTarget runtimeSingletonFirstTarget = runtimeSingletonRoot.GetComponent<SingleInterfaceTarget>();
            SingleInterfaceTarget runtimeSingletonSecondTarget = runtimeSingletonRoot.transform.GetChild(0).GetComponent<SingleInterfaceTarget>();
            ComponentDependency[] runtimeDependencies = Object.FindObjectsByType<ComponentDependency>(FindObjectsSortMode.None);

            // Assert in play mode
            Assert.That(runtimeTransientRoot, Is.Not.Null);
            Assert.That(runtimeSingletonRoot, Is.Not.Null);
            Assert.That(runtimeTransientFirstTarget, Is.Not.Null);
            Assert.That(runtimeTransientSecondTarget, Is.Not.Null);
            Assert.That(runtimeSingletonFirstTarget, Is.Not.Null);
            Assert.That(runtimeSingletonSecondTarget, Is.Not.Null);
            Assert.That(runtimeDependencies, Has.Length.EqualTo(3));
            CollectionAssert.AllItemsAreNotNull(runtimeDependencies);
            CollectionAssert.AllItemsAreUnique(runtimeDependencies);
            CollectionAssert.AllItemsAreInstancesOfType(runtimeDependencies, typeof(ComponentDependency));
            Assert.That(runtimeTransientFirstTarget.dependency, Is.Not.InstanceOf<RuntimeProxyBase>());
            Assert.That(runtimeTransientSecondTarget.dependency, Is.Not.InstanceOf<RuntimeProxyBase>());
            Assert.That(runtimeSingletonFirstTarget.dependency, Is.Not.InstanceOf<RuntimeProxyBase>());
            Assert.That(runtimeSingletonSecondTarget.dependency, Is.Not.InstanceOf<RuntimeProxyBase>());
            Assert.That(runtimeTransientFirstTarget.dependency as Object, Is.Not.EqualTo(runtimeTransientSecondTarget.dependency as Object));
            Assert.That(runtimeSingletonFirstTarget.dependency as Object, Is.EqualTo(runtimeSingletonSecondTarget.dependency as Object));

            CollectionAssert.AreEquivalent(runtimeDependencies, new[]
            {
                runtimeTransientFirstTarget.dependency as Object,
                runtimeTransientSecondTarget.dependency as Object,
                runtimeSingletonFirstTarget.dependency as Object
            });

            Assert.That(GlobalScope.IsRegistered<ComponentDependency>(), Is.True);
            Assert.That(GlobalScope.GetComponent<ComponentDependency>(), Is.EqualTo(runtimeSingletonFirstTarget.dependency as Object));

            // Clear global scope in play mode
            GlobalScope.Clear();
        }
    }
}