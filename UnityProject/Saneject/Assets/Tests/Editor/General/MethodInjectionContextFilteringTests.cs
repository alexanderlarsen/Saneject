using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Settings;
using Tests.Runtime;
using Tests.Runtime.MethodInjection;
using UnityEditor;
using UnityEngine;

namespace Tests.Editor.General
{
    public class MethodInjectionContextFilteringTests : BaseBindingTest
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
        public void RespectsContextFiltering_ForMethodInjection_WhenEnabled()
        {
            // Suppress expected errors from filtered dependencies
            IgnoreErrorMessages();
            UserSettings.FilterBySameContext = true;

            // Add components
            TestScope scope = sceneRoot.AddComponent<TestScope>();
            MyClass target = sceneRoot.AddComponent<MyClass>();

            // Create prefab instance (different context)
            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/Prefab 2")) as GameObject;

            MyDependency dep = prefabInstance.AddComponent<MyDependency>();

            // Set up bindings — bind to prefab dependency
            BindComponent<MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(dep);

            BindComponent<IDependency, MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(dep);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(GetPrivateField<MyDependency>(target, "myDependency2"),
                "Scene target should NOT resolve from prefab instance when filtering is enabled.");

            Assert.IsNull(GetPrivateField<IDependency>(target, "dependency"),
                "Scene target should NOT resolve interface from prefab instance when filtering is enabled.");
        }

        [Test]
        public void AllowsCrossContext_ForMethodInjection_WhenFilteringDisabled()
        {
            // Suppress expected errors from unfiltered contexts
            IgnoreErrorMessages();
            UserSettings.FilterBySameContext = false;

            // Add components
            TestScope scope = sceneRoot.AddComponent<TestScope>();
            MyClass target = sceneRoot.AddComponent<MyClass>();

            // Create prefab instance (different context)
            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(
                Resources.Load<GameObject>("Test/Prefab 2")) as GameObject;

            MyDependency dep = prefabInstance.AddComponent<MyDependency>();

            // Set up bindings — bind to prefab dependency
            BindComponent<MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(dep);

            BindComponent<IDependency, MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(dep);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(dep, GetPrivateField<MyDependency>(target, "myDependency2"),
                "Scene target should resolve from prefab instance when filtering is disabled.");

            Assert.AreEqual(dep, GetPrivateField<IDependency>(target, "dependency"),
                "Scene target should resolve interface from prefab instance when filtering is disabled.");
        }

        [Test]
        public void RejectsPrefabAsset_ForMethodInjection_WhenFilteringEnabled()
        {
            // Suppress expected errors
            IgnoreErrorMessages();
            UserSettings.FilterBySameContext = true;

            // Add components
            TestScope scope = sceneRoot.AddComponent<TestScope>();
            MyClass target = sceneRoot.AddComponent<MyClass>();

            // Load prefab asset directly (different context)
            GameObject prefabAsset = Resources.Load<GameObject>("Test/Prefab 2");
            MyDependency depOnAsset = prefabAsset.AddComponent<MyDependency>();

            // Set up bindings
            BindComponent<MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(depOnAsset);

            BindComponent<IDependency, MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(depOnAsset);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(GetPrivateField<MyDependency>(target, "myDependency2"),
                "Scene target should NOT resolve from prefab asset when filtering is enabled.");

            Assert.IsNull(GetPrivateField<IDependency>(target, "dependency"),
                "Scene target should NOT resolve interface from prefab asset when filtering is enabled.");
        }

        protected override void CreateHierarchy()
        {
            sceneRoot = new GameObject("SceneRoot");
        }

        private static T GetPrivateField<T>(
            object instance,
            string fieldName)
        {
            if (instance == null) return default;
            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field?.GetValue(instance);
        }
    }
}