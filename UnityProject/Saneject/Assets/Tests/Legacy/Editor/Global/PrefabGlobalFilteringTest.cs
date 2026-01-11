using System.Collections;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Plugins.Saneject.Legacy.Runtime.Global;
using Plugins.Saneject.Legacy.Runtime.Settings;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Component;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Legacy.Editor.Global
{
    public class PrefabGlobalFilteringTest : BaseBindingTest
    {
        private GameObject root;
        private bool prevFilterBySameContext;

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
        public void PrefabComponent_NotRegisteredAsGlobal_WhenFilteringEnabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = true;

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Tests/Legacy/Runtime/Resources/Test/Prefab 1.prefab");

            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;
            InjectableComponent injectable = prefabInstance.AddComponent<InjectableComponent>();

            // Set up bindings
            BindGlobal<InjectableComponent>(scope).FromInstance(injectable);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            SceneGlobalContainer container = Object.FindFirstObjectByType<SceneGlobalContainer>();

            if (container != null)
            {
                FieldInfo field = typeof(SceneGlobalContainer)
                    .GetField("globalBindings", BindingFlags.NonPublic | BindingFlags.Instance);

                IEnumerable list = field.GetValue(container) as IEnumerable;

                bool found = false;

                foreach (object item in list)
                {
                    PropertyInfo instanceProp = item.GetType().GetProperty("Instance", BindingFlags.Public | BindingFlags.Instance);
                    Object instance = instanceProp?.GetValue(item) as Object;

                    if (instance == injectable)
                    {
                        found = true;
                        break;
                    }
                }

                Assert.IsFalse(found, "Prefab component should not be present in globalBindings when filtering is enabled.");
            }

            Assert.IsNull(requester.interfaceComponent,
                "Requester should not resolve from prefab global binding when filtering is enabled.");
        }

        [Test]
        public void PrefabComponent_RegisteredAsGlobal_WhenFilteringDisabled()
        {
            IgnoreErrorMessages();
            UserSettings.UseContextIsolation = false;

            // Add components
            TestScope scope = root.AddComponent<TestScope>();

            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Tests/Legacy/Runtime/Resources/Test/Prefab 1.prefab");

            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;
            InjectableComponent injectable = prefabInstance.AddComponent<InjectableComponent>();

            // Set up bindings
            BindGlobal<InjectableComponent>(scope).FromInstance(injectable);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            SceneGlobalContainer container = Object.FindFirstObjectByType<SceneGlobalContainer>();
            Assert.NotNull(container, "SceneGlobalContainer should exist when global binding is allowed.");

            FieldInfo field = typeof(SceneGlobalContainer)
                .GetField("globalBindings", BindingFlags.NonPublic | BindingFlags.Instance);

            IEnumerable list = field.GetValue(container) as IEnumerable;

            bool found = false;

            foreach (object item in list)
            {
                PropertyInfo instanceProp = item.GetType().GetProperty("Instance", BindingFlags.Public | BindingFlags.Instance);
                Object instance = instanceProp?.GetValue(item) as Object;

                if (instance == injectable)
                {
                    found = true;
                    break;
                }
            }

            Assert.IsTrue(found, "Prefab component should be present in globalBindings when filtering is disabled.");
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }
    }
}