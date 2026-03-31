using System.Collections;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using Tests.Saneject.Fixtures.Scripts.RuntimeProxy;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Runtime
{
    public class ScopePlayModeTests
    {
        private const string TestAssetFolder = "Assets/Tests/Saneject/Fixtures/ScopePlayModeAssets";

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (Application.isPlaying)
                yield return new ExitPlayMode();

            Assert.That(Application.isPlaying, Is.False);

            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(TestRuntimeProxy)}");

            foreach (string guid in assetGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            AssetDatabase.DeleteAsset(TestAssetFolder);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [UnityTest]
        public IEnumerator Scope_And_GlobalScope_RuntimeBehavior()
        {
            Assert.That(Application.isPlaying, Is.False);

            // Set up scene in edit mode
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            scene.Add<ComponentDependency>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");
            TrackingRuntimeProxySwapTarget trackingTarget = scene.Add<TrackingRuntimeProxySwapTarget>("Root 1");

            // Bind in edit mode
            scope.BindGlobal<ComponentDependency>().FromSelf();

            scope.BindComponent<IDependency, ComponentDependency>()
                .FromRuntimeProxy()
                .FromGlobalScope();

            // Inject in edit mode
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            scope.AddProxySwapTarget(trackingTarget);

            // Assert in edit mode
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.InstanceOf<TestRuntimeProxy>());

            // Save scene in edit mode
            scene.SaveToDisk(TestAssetFolder);

            // Enter play mode
            yield return new EnterPlayMode();
            Assert.That(Application.isPlaying, Is.True);

            // Wait one frame in play mode
            yield return new WaitForEndOfFrame();

            // Find dependencies in play mode
            TestScope runtimeScope = Object.FindFirstObjectByType<TestScope>();
            ComponentDependency runtimeDependency = Object.FindFirstObjectByType<ComponentDependency>();
            SingleInterfaceTarget runtimeTarget = Object.FindFirstObjectByType<SingleInterfaceTarget>();
            TrackingRuntimeProxySwapTarget runtimeTrackingTarget = Object.FindFirstObjectByType<TrackingRuntimeProxySwapTarget>();

            // Assert in play mode
            Assert.That(runtimeScope, Is.Not.Null);
            Assert.That(runtimeDependency, Is.Not.Null);
            Assert.That(runtimeTarget, Is.Not.Null);
            Assert.That(runtimeTrackingTarget, Is.Not.Null);
            Assert.That(GlobalScope.IsRegistered<ComponentDependency>(), Is.True);
            Assert.That(GlobalScope.GetComponent<ComponentDependency>(), Is.EqualTo(runtimeDependency));
            Assert.That(runtimeTrackingTarget.swapCount, Is.EqualTo(1));
            Assert.That(runtimeTarget.dependency, Is.Not.Null);
            Assert.That(runtimeTarget.dependency, Is.Not.InstanceOf<RuntimeProxyBase>());
            Assert.That(runtimeTarget.dependency as Object, Is.EqualTo(runtimeDependency));

            // Destroy scope in play mode and wait for Unity to catch up
            Object.DestroyImmediate(runtimeScope.gameObject);

            yield return new WaitForEndOfFrame();

            // Assert in play mode
            Assert.That(GlobalScope.IsRegistered<ComponentDependency>(), Is.False);
            Assert.That(GlobalScope.GetComponent<ComponentDependency>(), Is.Null);

            // Clear global scope in play mode
            GlobalScope.Clear();
        }
    }
}