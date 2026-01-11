using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Component;
using Tests.Legacy.Runtime.NestedSerializable;
using UnityEngine;

namespace Tests.Legacy.Editor.Bindings.ComponentBinding.Qualifiers
{
    public class ToTargetNestedTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsToRoot_OnlyRootGetsInjected()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            RootClass rootClass = root.AddComponent<RootClass>();
            InjectableComponent injectable = root.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .ToTarget<RootClass>()
                .ToMember("injectableComponent")
                .FromInstance(injectable);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNotNull(GetPrivateField<InjectableComponent>(rootClass, "injectableComponent"));
            object nested = GetPrivateField<object>(rootClass, "nestedClass");
            Assert.NotNull(nested);
            Assert.IsNull(GetPrivateField<InjectableComponent>(nested, "injectableComponent"));
            object deepNested = GetPrivateField<object>(nested, "deepNestedClass");
            Assert.NotNull(deepNested);
            Assert.IsNull(GetPrivateField<InjectableComponent>(deepNested, "injectableComponent"));
        }

        [Test]
        public void InjectsToNestedClass_OnlyNestedGetsInjected()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            RootClass rootClass = root.AddComponent<RootClass>();
            InjectableComponent injectable = root.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .ToTarget<NestedClass>()
                .ToMember("injectableComponent")
                .FromInstance(injectable);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(GetPrivateField<InjectableComponent>(rootClass, "injectableComponent"));
            object nested = GetPrivateField<object>(rootClass, "nestedClass");
            Assert.NotNull(nested);
            Assert.IsNotNull(GetPrivateField<InjectableComponent>(nested, "injectableComponent"));
            object deepNested = GetPrivateField<object>(nested, "deepNestedClass");
            Assert.NotNull(deepNested);
            Assert.IsNull(GetPrivateField<InjectableComponent>(deepNested, "injectableComponent"));
        }

        [Test]
        public void InjectsToDeepNestedClass_OnlyDeepNestedGetsInjected()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            RootClass rootClass = root.AddComponent<RootClass>();
            InjectableComponent injectable = root.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .ToTarget<DeepNestedClass>()
                .ToMember("injectableComponent")
                .FromInstance(injectable);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(GetPrivateField<InjectableComponent>(rootClass, "injectableComponent"));
            object nested = GetPrivateField<object>(rootClass, "nestedClass");
            Assert.NotNull(nested);
            Assert.IsNull(GetPrivateField<InjectableComponent>(nested, "injectableComponent"));
            object deepNested = GetPrivateField<object>(nested, "deepNestedClass");
            Assert.NotNull(deepNested);
            Assert.IsNotNull(GetPrivateField<InjectableComponent>(deepNested, "injectableComponent"));
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
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