using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.MethodInjection;
using UnityEngine;

namespace Tests.Legacy.Editor.MethodInjection
{
    public class MethodInjectionTest : BaseBindingTest
    {
        private GameObject root, childA, childB, childC;

        [Test]
        public void InjectsMethods_OnRoot_MultipleParameters()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            MyClass rootClass = root.AddComponent<MyClass>();
            MyDependency dependency = root.AddComponent<MyDependency>();

            // Set up bindings
            BindComponent<MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(dependency);

            BindComponent<IDependency, MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(dependency);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNotNull(GetPrivateField<MyDependency>(rootClass, "myDependency2"));
            Assert.IsNotNull(GetPrivateField<IDependency>(rootClass, "dependency"));

            Assert.AreEqual(
                GetPrivateField<MyDependency>(rootClass, "myDependency2"),
                GetPrivateField<MyDependency>(rootClass, "myDependency"));
        }

        [Test]
        public void InjectsMethods_OnNestedClass_Collections()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            MyClass rootClass = root.AddComponent<MyClass>();
            MyDependency depA = childA.AddComponent<MyDependency>();
            MyDependency depB = childB.AddComponent<MyDependency>();
            MyDependency depC = childC.AddComponent<MyDependency>();

            // Set up bindings
            BindMultipleComponents<MyDependency>(scope)
                .ToTarget<MyNestedClass>()
                .FromRootDescendants();

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            object nested = GetPrivateField<object>(rootClass, "nestedClass");
            Assert.NotNull(nested);
            MyDependency[] injected = GetPrivateField<MyDependency[]>(nested, "myDependencies");
            Assert.NotNull(injected);
            Assert.AreEqual(3, injected.Length);
            CollectionAssert.Contains(injected, depA);
            CollectionAssert.Contains(injected, depB);
            CollectionAssert.Contains(injected, depC);
        }

        [Test]
        public void InjectsMethods_WithMixedParameterOrder()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            MyClass rootClass = root.AddComponent<MyClass>();
            MyDependency dependency = root.AddComponent<MyDependency>();

            // Set up bindings
            BindComponent<IDependency, MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(dependency);

            BindComponent<MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(dependency);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNotNull(GetPrivateField<MyDependency>(rootClass, "myDependency2"));
            Assert.IsNotNull(GetPrivateField<IDependency>(rootClass, "dependency"));
        }

        [Test]
        public void InjectsMethods_IntoBothRootAndNestedClasses()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            MyClass rootClass = root.AddComponent<MyClass>();
            MyDependency depA = childA.AddComponent<MyDependency>();
            MyDependency depB = childB.AddComponent<MyDependency>();

            // Set up bindings
            BindComponent<MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(depA);

            BindMultipleComponents<MyDependency>(scope)
                .ToTarget<MyNestedClass>()
                .FromRootDescendants();

            BindComponent<IDependency, MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(depA);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert root injection
            Assert.IsNotNull(GetPrivateField<MyDependency>(rootClass, "myDependency2"));
            Assert.IsNotNull(GetPrivateField<IDependency>(rootClass, "dependency"));

            // Assert nested injection
            object nested = GetPrivateField<object>(rootClass, "nestedClass");
            MyDependency[] injected = GetPrivateField<MyDependency[]>(nested, "myDependencies");
            Assert.NotNull(injected);
            CollectionAssert.Contains(injected, depA);
            CollectionAssert.Contains(injected, depB);
        }

        [Test]
        public void Injects_AllAnnotatedMethods_RegardlessOfOrder()
        {
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            MyClassWithMultipleInjects instance = root.AddComponent<MyClassWithMultipleInjects>();
            MyDependency dep = root.AddComponent<MyDependency>();

            // Set up bindings
            BindComponent<MyDependency>(scope).FromInstance(dep);
            BindComponent<IDependency, MyDependency>(scope).FromInstance(dep);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsTrue(instance.inject1Called);
            Assert.IsTrue(instance.inject2Called);
            Assert.AreEqual(dep, instance.dependency1);
            Assert.AreEqual(dep, instance.dependency2);
        }

        [Test]
        public void Injects_Collections_AsListAndArray()
        {
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            MyClassWithCollectionMethods instance = root.AddComponent<MyClassWithCollectionMethods>();
            MyDependency depA = childA.AddComponent<MyDependency>();
            MyDependency depB = childB.AddComponent<MyDependency>();

            // Set up bindings
            BindMultipleComponents<MyDependency>(scope).FromRootDescendants();

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(2, instance.arrayDeps.Length);
            Assert.AreEqual(2, instance.listDeps.Count);
            CollectionAssert.Contains(instance.arrayDeps, depA);
            CollectionAssert.Contains(instance.listDeps, depB);
        }

        [Test]
        public void Injects_PrivateAndProtectedMethods()
        {
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            MyClassWithPrivateInject instance = root.AddComponent<MyClassWithPrivateInject>();
            MyDependency dep = root.AddComponent<MyDependency>();

            // Set up bindings
            BindComponent<MyDependency>(scope).FromInstance(dep);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsTrue(instance.privateInjected);
            Assert.IsTrue(instance.protectedInjected);
        }

        [Test]
        public void DoesNotInvokeMethod_WhenAnyParameterMissing()
        {
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            MyClassPartialInjection instance = root.AddComponent<MyClassPartialInjection>();
            MyDependency dep = root.AddComponent<MyDependency>();

            // Set up bindings
            BindComponent<MyDependency>(scope).FromInstance(dep);
            // Missing IDependency binding intentionally

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsFalse(instance.methodCalled);
        }

        [Test]
        public void Injects_IntoDeepNestedSerializableClass()
        {
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            MyClassWithDeepNested rootClass = root.AddComponent<MyClassWithDeepNested>();

            // Create descendant dependencies
            GameObject childA = new("ChildA");
            GameObject childB = new("ChildB");
            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);

            MyDependency depA = childA.AddComponent<MyDependency>();
            MyDependency depB = childB.AddComponent<MyDependency>();

            // Set up bindings
            BindMultipleComponents<MyDependency>(scope)
                .ToTarget<MyDeepNestedClass>()
                .FromRootDescendants();

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            object nested = GetPrivateField<object>(rootClass, "nestedClass");
            object deep = GetPrivateField<object>(nested, "deepNested");
            MyDependency[] injected = GetPrivateField<MyDependency[]>(deep, "deps");

            Assert.NotNull(injected, "Expected injected array not null");
            Assert.Greater(injected.Length, 0, "Expected at least one dependency");
            CollectionAssert.Contains(injected, depA);
            CollectionAssert.Contains(injected, depB);
        }

        [Test]
        public void Injects_MethodsAndFields_Together()
        {
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            MyClass rootClass = root.AddComponent<MyClass>();
            MyDependency dep = root.AddComponent<MyDependency>();

            // Set up bindings
            BindComponent<MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(dep);

            BindComponent<IDependency, MyDependency>(scope)
                .ToTarget<MyClass>()
                .FromInstance(dep);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert both field and method-based values exist
            Assert.IsNotNull(GetPrivateField<MyDependency>(rootClass, "myDependency"));
            Assert.IsNotNull(GetPrivateField<MyDependency>(rootClass, "myDependency2"));
            Assert.IsNotNull(GetPrivateField<IDependency>(rootClass, "dependency"));
        }

        [Test]
        public void Injects_Methods_InParentAndChildScopes()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope parentScope = root.AddComponent<TestScope>();
            GameObject childRoot = new("ChildRoot");
            childRoot.transform.SetParent(root.transform);
            TestScope childScope = childRoot.AddComponent<TestScope>();
            childScope.ParentScope = parentScope;

            MyClass parent = root.AddComponent<MyClass>();
            MyClass child = childRoot.AddComponent<MyClass>();
            MyDependency depParent = root.AddComponent<MyDependency>();
            MyDependency depChild = childRoot.AddComponent<MyDependency>();

            // Set up bindings
            // Parent binds both concrete and interface for parent MyClass
            BindComponent<MyDependency>(parentScope)
                .ToTarget<MyClass>()
                .FromInstance(depParent);

            BindComponent<IDependency, MyDependency>(parentScope)
                .ToTarget<MyClass>()
                .FromInstance(depParent);

            // Child binds both concrete and interface for child MyClass
            BindComponent<MyDependency>(childScope)
                .ToTarget<MyClass>()
                .FromInstance(depChild);

            BindComponent<IDependency, MyDependency>(childScope)
                .ToTarget<MyClass>()
                .FromInstance(depChild);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert both injected properly in their respective scopes
            Assert.IsNotNull(GetPrivateField<MyDependency>(parent, "myDependency2"),
                "Expected parent MyClass to receive MyDependency from parent scope.");

            Assert.IsNotNull(GetPrivateField<IDependency>(parent, "dependency"),
                "Expected parent MyClass to receive IDependency from parent scope.");

            Assert.IsNotNull(GetPrivateField<MyDependency>(child, "myDependency2"),
                "Expected child MyClass to receive MyDependency from child scope.");

            Assert.IsNotNull(GetPrivateField<IDependency>(child, "dependency"),
                "Expected child MyClass to receive IDependency from child scope.");
        }

        [Test]
        public void DoesNotInject_WhenBindingsMissing()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            MyClass rootClass = root.AddComponent<MyClass>();

            // Set up bindings
            // intentionally empty to simulate missing dependencies

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(GetPrivateField<MyDependency>(rootClass, "myDependency2"));
            Assert.IsNull(GetPrivateField<IDependency>(rootClass, "dependency"));
            object nested = GetPrivateField<object>(rootClass, "nestedClass");
            MyDependency[] injected = GetPrivateField<MyDependency[]>(nested, "myDependencies");
            Assert.IsNull(injected);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            childA = new GameObject();
            childB = new GameObject();
            childC = new GameObject();

            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);
            childC.transform.SetParent(root.transform);
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