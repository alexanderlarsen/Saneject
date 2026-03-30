using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Runtime.Proxy;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Binding.RuntimeProxy
{
    public class RuntimeProxyBindingBuilderTests
    {
        [SetUp, TearDown]
        public void CleanUpExistingProxyAssets()
        {
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}");

            foreach (string guid in assetGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Test]
        public void FromRuntimeProxy_InjectsProxyAssetWithDefaultConfig()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Find dependency
            Object injectedDependency = target.dependency as Object;
            RuntimeProxyBase runtimeProxy = target.dependency as RuntimeProxyBase;
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}");
            Assert.That(assetGuids, Has.Length.EqualTo(1));

            TestRuntimeProxy proxyAsset = AssetDatabase.LoadAssetAtPath<TestRuntimeProxy>
            (
                AssetDatabase.GUIDToAssetPath(assetGuids[0])
            );

            // Assert
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(injectedDependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(injectedDependency, Is.SameAs(proxyAsset));
            Assert.That(runtimeProxy, Is.Not.Null);
            Assert.That(AssetDatabase.Contains(injectedDependency), Is.True);

            Assert.That(runtimeProxy.HasConfig(new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromGlobalScope,
                instanceMode: RuntimeProxyInstanceMode.Singleton,
                prefab: null,
                dontDestroyOnLoad: false
            )), Is.True);
        }

        [Test]
        public void FromRuntimeProxy_FromGlobalScope_InjectsProxyAssetWithExpectedConfig()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy()
                .FromGlobalScope();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Find dependency
            Object injectedDependency = target.dependency as Object;
            RuntimeProxyBase runtimeProxy = target.dependency as RuntimeProxyBase;
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}");
            Assert.That(assetGuids, Has.Length.EqualTo(1));

            TestRuntimeProxy proxyAsset = AssetDatabase.LoadAssetAtPath<TestRuntimeProxy>
            (
                AssetDatabase.GUIDToAssetPath(assetGuids[0])
            );

            // Assert
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(injectedDependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(injectedDependency, Is.SameAs(proxyAsset));
            Assert.That(runtimeProxy, Is.Not.Null);
            Assert.That(AssetDatabase.Contains(injectedDependency), Is.True);

            Assert.That(runtimeProxy.HasConfig(new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromGlobalScope,
                instanceMode: RuntimeProxyInstanceMode.Transient,
                prefab: null,
                dontDestroyOnLoad: false
            )), Is.True);
        }

        [Test]
        public void FromRuntimeProxy_FromAnywhereInLoadedScenes_InjectsProxyAssetWithExpectedConfig()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy()
                .FromAnywhereInLoadedScenes();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Find dependency
            Object injectedDependency = target.dependency as Object;
            RuntimeProxyBase runtimeProxy = target.dependency as RuntimeProxyBase;
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}");
            Assert.That(assetGuids, Has.Length.EqualTo(1));

            TestRuntimeProxy proxyAsset = AssetDatabase.LoadAssetAtPath<TestRuntimeProxy>
            (
                AssetDatabase.GUIDToAssetPath(assetGuids[0])
            );

            // Assert
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(injectedDependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(injectedDependency, Is.SameAs(proxyAsset));
            Assert.That(runtimeProxy, Is.Not.Null);
            Assert.That(AssetDatabase.Contains(injectedDependency), Is.True);

            Assert.That(runtimeProxy.HasConfig(new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromAnywhereInLoadedScenes,
                instanceMode: RuntimeProxyInstanceMode.Transient,
                prefab: null,
                dontDestroyOnLoad: false
            )), Is.True);
        }

        [Test]
        public void FromRuntimeProxy_FromComponentOnPrefab_InjectsProxyAssetWithExpectedConfig()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            GameObject prefab = new("Runtime Proxy Prefab");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy()
                .FromComponentOnPrefab(prefab, dontDestroyOnLoad: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Find dependency
            Object injectedDependency = target.dependency as Object;
            RuntimeProxyBase runtimeProxy = target.dependency as RuntimeProxyBase;
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}");
            Assert.That(assetGuids, Has.Length.EqualTo(1));

            TestRuntimeProxy proxyAsset = AssetDatabase.LoadAssetAtPath<TestRuntimeProxy>
            (
                AssetDatabase.GUIDToAssetPath(assetGuids[0])
            );

            // Assert
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(injectedDependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(injectedDependency, Is.SameAs(proxyAsset));
            Assert.That(runtimeProxy, Is.Not.Null);
            Assert.That(AssetDatabase.Contains(injectedDependency), Is.True);

            Assert.That(runtimeProxy.HasConfig(new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromComponentOnPrefab,
                instanceMode: RuntimeProxyInstanceMode.Transient,
                prefab: prefab,
                dontDestroyOnLoad: false
            )), Is.True);

            // Clean up
            Object.DestroyImmediate(prefab);
        }

        [Test]
        public void FromRuntimeProxy_FromComponentOnPrefab_WithDontDestroyOnLoad_InjectsProxyAssetWithExpectedConfig()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            GameObject prefab = new("Runtime Proxy Prefab");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy()
                .FromComponentOnPrefab(prefab, dontDestroyOnLoad: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Find dependency
            Object injectedDependency = target.dependency as Object;
            RuntimeProxyBase runtimeProxy = target.dependency as RuntimeProxyBase;
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}");
            Assert.That(assetGuids, Has.Length.EqualTo(1));

            TestRuntimeProxy proxyAsset = AssetDatabase.LoadAssetAtPath<TestRuntimeProxy>
            (
                AssetDatabase.GUIDToAssetPath(assetGuids[0])
            );

            // Assert
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(injectedDependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(injectedDependency, Is.SameAs(proxyAsset));
            Assert.That(runtimeProxy, Is.Not.Null);
            Assert.That(AssetDatabase.Contains(injectedDependency), Is.True);

            Assert.That(runtimeProxy.HasConfig(new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromComponentOnPrefab,
                instanceMode: RuntimeProxyInstanceMode.Transient,
                prefab: prefab,
                dontDestroyOnLoad: true
            )), Is.True);

            // Clean up
            Object.DestroyImmediate(prefab);
        }

        [Test]
        public void FromRuntimeProxy_FromNewComponentOnNewGameObject_InjectsProxyAssetWithExpectedConfig()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy()
                .FromNewComponentOnNewGameObject(dontDestroyOnLoad: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Find dependency
            Object injectedDependency = target.dependency as Object;
            RuntimeProxyBase runtimeProxy = target.dependency as RuntimeProxyBase;
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}");
            Assert.That(assetGuids, Has.Length.EqualTo(1));

            TestRuntimeProxy proxyAsset = AssetDatabase.LoadAssetAtPath<TestRuntimeProxy>
            (
                AssetDatabase.GUIDToAssetPath(assetGuids[0])
            );

            // Assert
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(injectedDependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(injectedDependency, Is.SameAs(proxyAsset));
            Assert.That(runtimeProxy, Is.Not.Null);
            Assert.That(AssetDatabase.Contains(injectedDependency), Is.True);

            Assert.That(runtimeProxy.HasConfig(new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromNewComponentOnNewGameObject,
                instanceMode: RuntimeProxyInstanceMode.Transient,
                prefab: null,
                dontDestroyOnLoad: false
            )), Is.True);
        }

        [Test]
        public void FromRuntimeProxy_FromNewComponentOnNewGameObject_WithDontDestroyOnLoad_InjectsProxyAssetWithExpectedConfig()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy()
                .FromNewComponentOnNewGameObject(dontDestroyOnLoad: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Find dependency
            Object injectedDependency = target.dependency as Object;
            RuntimeProxyBase runtimeProxy = target.dependency as RuntimeProxyBase;
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}");
            Assert.That(assetGuids, Has.Length.EqualTo(1));

            TestRuntimeProxy proxyAsset = AssetDatabase.LoadAssetAtPath<TestRuntimeProxy>
            (
                AssetDatabase.GUIDToAssetPath(assetGuids[0])
            );

            // Assert
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(injectedDependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(injectedDependency, Is.SameAs(proxyAsset));
            Assert.That(runtimeProxy, Is.Not.Null);
            Assert.That(AssetDatabase.Contains(injectedDependency), Is.True);

            Assert.That(runtimeProxy.HasConfig(new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromNewComponentOnNewGameObject,
                instanceMode: RuntimeProxyInstanceMode.Transient,
                prefab: null,
                dontDestroyOnLoad: true
            )), Is.True);
        }
    }
}