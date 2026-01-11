using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Plugins.Saneject.Legacy.Runtime.Settings;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.MethodInjection;
using UnityEditor;
using UnityEngine;

namespace Tests.Legacy.Editor.General
{
    public class MethodInjectionContextFilteringTests : BaseBindingTest
    {
        private bool prevFilterBySameContext;
        private GameObject sceneRoot;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            prevFilterBySameContext = UserSettings.UseContextIsolation;
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            UserSettings.UseContextIsolation = prevFilterBySameContext;
        }

        [Test]
        public void RespectsContextFiltering_ForMethodInjection_WhenIsolationEnabled()
        {
            // Suppress expected errors from filtered dependencies
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

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
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(GetPrivateField<MyDependency>(target, "myDependency2"),
                "Scene target should NOT resolve from prefab instance when filtering is enabled.");

            Assert.IsNull(GetPrivateField<IDependency>(target, "dependency"),
                "Scene target should NOT resolve interface from prefab instance when filtering is enabled.");
        }

        [Test]
        public void AllowsCrossContext_ForMethodInjection_WhenIsolationDisabled()
        {
            // Suppress expected errors from unfiltered contexts
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

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
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(dep, GetPrivateField<MyDependency>(target, "myDependency2"),
                "Scene target should resolve from prefab instance when filtering is disabled.");

            Assert.AreEqual(dep, GetPrivateField<IDependency>(target, "dependency"),
                "Scene target should resolve interface from prefab instance when filtering is disabled.");
        }

        [Test]
        public void RejectsPrefabAsset_ForMethodInjection_WhenIsolationEnabled()
        {
            // Suppress expected errors
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

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
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(GetPrivateField<MyDependency>(target, "myDependency2"),
                "Scene target should NOT resolve from prefab asset when filtering is enabled.");

            Assert.IsNull(GetPrivateField<IDependency>(target, "dependency"),
                "Scene target should NOT resolve interface from prefab asset when filtering is enabled.");
        }

        [Test]
        public void AllowsPrefabAsset_ForMethodInjection_WhenIsolationDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            TestScope scope = sceneRoot.AddComponent<TestScope>();
            MyClass target = sceneRoot.AddComponent<MyClass>();

            GameObject prefabAsset = Resources.Load<GameObject>("Test/Prefab 2");
            MyDependency depOnAsset = prefabAsset.AddComponent<MyDependency>();

            BindComponent<MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(depOnAsset);

            BindComponent<IDependency, MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(depOnAsset);

            DependencyInjector.InjectCurrentScene();

            Assert.AreEqual(depOnAsset, GetPrivateField<MyDependency>(target, "myDependency2"));
            Assert.AreEqual(depOnAsset, GetPrivateField<IDependency>(target, "dependency"));
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