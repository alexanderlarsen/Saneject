using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using Tests.Saneject.Fixtures.Scripts.RuntimeProxy;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.RuntimeProxy
{
    public class RuntimeProxyIntegrationTests
    {
        private const string TestAssetFolder = "Assets/Tests/Saneject/Fixtures/RuntimeProxyIntegrationAssets";

        private string originalProxyAssetGenerationFolder;

        [SetUp]
        public void SetUp()
        {
            originalProxyAssetGenerationFolder = ProjectSettings.ProxyAssetGenerationFolder;
            ProjectSettings.ProxyAssetGenerationFolder = TestAssetFolder;

            string[] proxyGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}");

            foreach (string guid in proxyGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            AssetDatabase.DeleteAsset(TestAssetFolder);

            if (!AssetDatabase.IsValidFolder(TestAssetFolder))
                AssetDatabase.CreateFolder("Assets/Tests/Saneject/Fixtures", "RuntimeProxyIntegrationAssets");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [TearDown]
        public void TearDown()
        {
            ProjectSettings.ProxyAssetGenerationFolder = originalProxyAssetGenerationFolder;

            string[] proxyGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}");

            foreach (string guid in proxyGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            AssetDatabase.DeleteAsset(TestAssetFolder);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Test]
        public void FromRuntimeProxy_GivenExistingMatchingProxyAsset_ReusesExistingAsset()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            RuntimeProxyConfig config = new
            (
                resolveMethod: RuntimeProxyResolveMethod.FromGlobalScope,
                instanceMode: RuntimeProxyInstanceMode.Singleton,
                prefab: null,
                dontDestroyOnLoad: false
            );

            TestRuntimeProxy existingProxy = ScriptableObject.CreateInstance<TestRuntimeProxy>();
            existingProxy.AssignConfig(config);
            AssetDatabase.CreateAsset(existingProxy, $"{TestAssetFolder}/Existing Proxy.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Find dependency
            Object injectedDependency = target.dependency as Object;
            RuntimeProxyBase runtimeProxy = target.dependency as RuntimeProxyBase;
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}", new[] { TestAssetFolder });
            Assert.That(assetGuids, Has.Length.EqualTo(1));

            TestRuntimeProxy storedProxy = AssetDatabase.LoadAssetAtPath<TestRuntimeProxy>
            (
                AssetDatabase.GUIDToAssetPath(assetGuids[0])
            );

            // Assert
            Assert.That(existingProxy, Is.Not.Null);
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(injectedDependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(injectedDependency, Is.SameAs(storedProxy));
            Assert.That(runtimeProxy, Is.Not.Null);
            Assert.That(runtimeProxy.HasConfig(config), Is.True);
        }

        [Test]
        public void FromRuntimeProxy_GivenExistingProxyAssetWithDifferentConfig_CreatesNewAsset()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            RuntimeProxyConfig existingConfig = new
            (
                resolveMethod: RuntimeProxyResolveMethod.FromAnywhereInLoadedScenes,
                instanceMode: RuntimeProxyInstanceMode.Transient,
                prefab: null,
                dontDestroyOnLoad: false
            );

            RuntimeProxyConfig injectedConfig = new
            (
                resolveMethod: RuntimeProxyResolveMethod.FromGlobalScope,
                instanceMode: RuntimeProxyInstanceMode.Singleton,
                prefab: null,
                dontDestroyOnLoad: false
            );

            TestRuntimeProxy existingProxy = ScriptableObject.CreateInstance<TestRuntimeProxy>();
            existingProxy.AssignConfig(existingConfig);
            AssetDatabase.CreateAsset(existingProxy, $"{TestAssetFolder}/Existing Proxy.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Find dependency
            Object injectedDependency = target.dependency as Object;
            RuntimeProxyBase runtimeProxy = target.dependency as RuntimeProxyBase;
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}", new[] { TestAssetFolder });
            Assert.That(assetGuids, Has.Length.EqualTo(2));

            // Assert
            Assert.That(existingProxy, Is.Not.Null);
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(injectedDependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(injectedDependency, Is.Not.SameAs(existingProxy));
            Assert.That(runtimeProxy, Is.Not.Null);
            Assert.That(runtimeProxy.HasConfig(injectedConfig), Is.True);
            Assert.That(existingProxy.HasConfig(existingConfig), Is.True);
        }

        [Test]
        public void FromRuntimeProxy_GivenSerializeInterfaceField_RegistersSwapTargetOnNearestScope()
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

            // Find registered targets
            FieldInfo proxySwapTargetsField = typeof(Scope).GetField("proxySwapTargets", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(proxySwapTargetsField, Is.Not.Null);

            List<Component> proxySwapTargets = proxySwapTargetsField.GetValue(scope) as List<Component>;

            // Assert
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(proxySwapTargets, Is.Not.Null);
            Assert.That(proxySwapTargets, Has.Count.EqualTo(1));
            CollectionAssert.Contains(proxySwapTargets, target);
        }

        [Test]
        public void FromRuntimeProxy_GivenSerializeInterfaceProperty_RegistersSwapTargetOnNearestScope()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfacePropertyTarget target = scene.Add<SingleInterfacePropertyTarget>("Root 1");

            // Bind
            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Find registered targets
            FieldInfo proxySwapTargetsField = typeof(Scope).GetField("proxySwapTargets", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(proxySwapTargetsField, Is.Not.Null);

            List<Component> proxySwapTargets = proxySwapTargetsField.GetValue(scope) as List<Component>;

            // Assert
            Assert.That(target.Dependency, Is.Not.Null);
            Assert.That(target.Dependency, Is.InstanceOf<TestRuntimeProxy>());
            Assert.That(proxySwapTargets, Is.Not.Null);
            Assert.That(proxySwapTargets, Has.Count.EqualTo(1));
            CollectionAssert.Contains(proxySwapTargets, target);
        }

        [Test]
        public void FromRuntimeProxy_GivenPreviouslyRegisteredSwapTargetThatNoLongerHasProxy_ClearsStaleSwapTarget()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SuppressedSingleInterfaceTarget target = scene.Add<SuppressedSingleInterfaceTarget>("Root 1/Child 1");
            TestRuntimeProxy proxy = ScriptableObject.CreateInstance<TestRuntimeProxy>();
            FieldInfo proxySwapTargetsField = typeof(Scope).GetField("proxySwapTargets", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(proxySwapTargetsField, Is.Not.Null);

            try
            {
                target.dependency = proxy;
                scope.AddProxySwapTarget(target);
                target.dependency = null;

                // Inject
                Assert.That(() => InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects), Throws.Nothing);

                // Find registered targets
                List<Component> proxySwapTargets = proxySwapTargetsField.GetValue(scope) as List<Component>;

                // Assert
                Assert.That(proxy, Is.Not.Null);
                Assert.That(target.dependency, Is.Null);
                Assert.That(proxySwapTargets, Is.Not.Null);
                Assert.That(proxySwapTargets, Is.Empty);
            }
            finally
            {
                Object.DestroyImmediate(proxy);
            }
        }

        [Test]
        public void AddProxySwapTarget_GivenDuplicateTarget_AddsTargetOnce()
        {
            // Set up components
            GameObject root = new("Root");

            try
            {
                TestScope scope = root.AddComponent<TestScope>();
                TrackingRuntimeProxySwapTarget target = root.AddComponent<TrackingRuntimeProxySwapTarget>();
                FieldInfo proxySwapTargetsField = typeof(Scope).GetField("proxySwapTargets", BindingFlags.NonPublic | BindingFlags.Instance);

                Assert.That(proxySwapTargetsField, Is.Not.Null);

                // Add duplicate targets
                scope.AddProxySwapTarget(target);
                scope.AddProxySwapTarget(target);

                List<Component> proxySwapTargets = proxySwapTargetsField.GetValue(scope) as List<Component>;

                // Assert
                Assert.That(target, Is.Not.Null);
                Assert.That(proxySwapTargets, Is.Not.Null);
                Assert.That(proxySwapTargets, Has.Count.EqualTo(1));
                CollectionAssert.Contains(proxySwapTargets, target);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void AddProxySwapTarget_GivenNonSwapTarget_ThrowsArgumentException()
        {
            // Set up components
            GameObject root = new("Root");

            try
            {
                TestScope scope = root.AddComponent<TestScope>();
                ComponentDependency dependency = root.AddComponent<ComponentDependency>();

                // Add invalid target
                ArgumentException exception = Assert.Throws<ArgumentException>(() => scope.AddProxySwapTarget(dependency));

                // Assert
                Assert.That(scope, Is.Not.Null);
                Assert.That(dependency, Is.Not.Null);
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.Message, Does.Contain(nameof(IRuntimeProxySwapTarget)));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void ClearProxySwapTargets_GivenAddedTargets_RemovesAllTargets()
        {
            // Set up components
            GameObject root = new("Root");
            GameObject child = new("Child");
            child.transform.SetParent(root.transform);

            try
            {
                TestScope scope = root.AddComponent<TestScope>();
                TrackingRuntimeProxySwapTarget firstTarget = root.AddComponent<TrackingRuntimeProxySwapTarget>();
                TrackingRuntimeProxySwapTarget secondTarget = child.AddComponent<TrackingRuntimeProxySwapTarget>();
                FieldInfo proxySwapTargetsField = typeof(Scope).GetField("proxySwapTargets", BindingFlags.NonPublic | BindingFlags.Instance);

                Assert.That(proxySwapTargetsField, Is.Not.Null);

                // Add and clear targets
                scope.AddProxySwapTarget(firstTarget);
                scope.AddProxySwapTarget(secondTarget);
                scope.ClearProxySwapTargets();

                List<Component> proxySwapTargets = proxySwapTargetsField.GetValue(scope) as List<Component>;

                // Assert
                Assert.That(firstTarget, Is.Not.Null);
                Assert.That(secondTarget, Is.Not.Null);
                Assert.That(proxySwapTargets, Is.Not.Null);
                Assert.That(proxySwapTargets, Is.Empty);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void Awake_GivenRegisteredProxySwapTargets_InvokesSwapTargets()
        {
            // Set up components
            GameObject root = new("Root");

            try
            {
                TestScope scope = root.AddComponent<TestScope>();
                TrackingRuntimeProxySwapTarget target = root.AddComponent<TrackingRuntimeProxySwapTarget>();
                MethodInfo awakeMethod = typeof(Scope).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);

                Assert.That(awakeMethod, Is.Not.Null);

                scope.AddProxySwapTarget(target);

                // Awake
                awakeMethod.Invoke(scope, null);

                // Assert
                Assert.That(target, Is.Not.Null);
                Assert.That(target.swapCount, Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
    }
}