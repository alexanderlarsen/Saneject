using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Global;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Editor.BindingResolution.Global
{
    public class GlobalBindingEditorTest
    {
        [SetUp]
        public void Setup()
        {
            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject go = new("Service");
            go.AddComponent<InjectableService>();

            GameObject scopeHolder = new("ScopeHolder");
            scopeHolder.AddComponent<TestScope>();

            DependencyInjector.InjectSceneDependencies();
        }

        [Test]
        public void SerializedToSceneGlobalContainer()
        {
            SceneGlobalContainer container = Object.FindFirstObjectByType<SceneGlobalContainer>();
            Assert.NotNull(container);

            Type type = typeof(SceneGlobalContainer);
            FieldInfo field = type.GetField("globalBindings", BindingFlags.NonPublic | BindingFlags.Instance);
            IEnumerable list = field.GetValue(container) as IEnumerable;

            Assert.NotNull(list);

            bool found = false;

            foreach (object item in list)
            {
                PropertyInfo instanceProp = item.GetType().GetProperty("Instance", BindingFlags.Public | BindingFlags.Instance);
                Object instance = instanceProp?.GetValue(item) as Object;

                if (instance is InjectableService)
                {
                    found = true;
                    break;
                }
            }

            Assert.IsTrue(found, "Expected global binding not found in SceneGlobalContainer.");
        }

        public class TestScope : Scope
        {
            public override void Configure()
            {
                Bind<InjectableService>().AsGlobal().FromAnywhereInScene();
            }
        }
    }
}