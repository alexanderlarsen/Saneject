using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Global;
using Tests.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Editor.Binding.Global
{
    public class GlobalBindingEditorTest : BaseBindingTest
    {
        private GameObject root, globalObject;

        [Test]
        public void SerializedToSceneGlobalContainer()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            globalObject.AddComponent<InjectableComponent>();

            // Set up bindings
            BindGlobal<InjectableComponent>(scope)
                .FromAnywhereInScene();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            SceneGlobalContainer container = Object.FindFirstObjectByType<SceneGlobalContainer>();
            Assert.NotNull(container);

            Type containerType = typeof(SceneGlobalContainer);
            FieldInfo field = containerType.GetField("globalBindings", BindingFlags.NonPublic | BindingFlags.Instance);
            IEnumerable list = field.GetValue(container) as IEnumerable;

            Assert.NotNull(list);

            bool found = false;

            foreach (object item in list)
            {
                PropertyInfo instanceProp = item.GetType().GetProperty("Instance", BindingFlags.Public | BindingFlags.Instance);
                Object instance = instanceProp?.GetValue(item) as Object;

                if (instance is InjectableComponent)
                {
                    found = true;
                    break;
                }
            }

            Assert.IsTrue(found, "Expected global binding not found in SceneGlobalContainer.");
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            globalObject = new GameObject();
        }
    }
}