using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Scopes
{
    public class ScopeTests
    {
        [Test]
        public void Scope_ResolvesFromParent()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 4);
            TestScope rootScope = scene.Add<TestScope>("Root 1");
            TestScope middleScope = scene.Add<TestScope>("Root 1/Child 1");
            TestScope lowerScope = scene.Add<TestScope>("Root 1/Child 1/Child 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1/Child 1/Child 1");

            // Find dependencies
            ComponentDependency rootDependency = scene.Add<ComponentDependency>("Root 1");
            ComponentDependency middleDependency = scene.Add<ComponentDependency>("Root 1/Child 1");
            ComponentDependency lowerDependency = scene.Add<ComponentDependency>("Root 1/Child 1/Child 1");

            // Bind
            rootScope.BindComponent<ComponentDependency>().FromInstance(rootDependency);
            middleScope.BindComponent<IDependency>().FromInstance(middleDependency);
            lowerScope.BindComponent<IDependency>().FromInstance(lowerDependency);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(rootDependency, Is.Not.Null);
            Assert.That(middleDependency, Is.Not.Null);
            Assert.That(lowerDependency, Is.Not.Null);
            Assert.That(target.dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(rootDependency));
            Assert.That(target.dependency, Is.Not.EqualTo(middleDependency));
            Assert.That(target.dependency, Is.Not.EqualTo(lowerDependency));
        }

        [Test]
        public void Scope_FallsBackToParent_WhenNearestScopeHasNoMatchingBinding()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            TestScope rootScope = scene.Add<TestScope>("Root 1");
            scene.Add<TestScope>("Root 1/Child 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1/Child 1");

            // Find dependency
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");

            // Bind
            rootScope.BindComponent<ComponentDependency>().FromInstance(dependency);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Scope_ChildOverridesParent_WhenBothBindSameConcreteRequestedType()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            TestScope rootScope = scene.Add<TestScope>("Root 1");
            TestScope childScope = scene.Add<TestScope>("Root 1/Child 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1/Child 1");

            // Find dependencies
            ComponentDependency rootDependency = scene.Add<ComponentDependency>("Root 1");
            ComponentDependency childDependency = scene.Add<ComponentDependency>("Root 1/Child 1");

            // Bind
            rootScope.BindComponent<ComponentDependency>().FromInstance(rootDependency);
            childScope.BindComponent<ComponentDependency>().FromInstance(childDependency);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(rootDependency, Is.Not.Null);
            Assert.That(childDependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(childDependency));
            Assert.That(target.dependency, Is.Not.EqualTo(rootDependency));
        }

        [Test]
        public void Scope_ChildOverridesParent_WhenBothBindSameInterfaceRequestedType()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            TestScope rootScope = scene.Add<TestScope>("Root 1");
            TestScope childScope = scene.Add<TestScope>("Root 1/Child 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1/Child 1");

            // Find dependencies
            ComponentDependency rootDependency = scene.Add<ComponentDependency>("Root 1");
            ComponentDependency childDependency = scene.Add<ComponentDependency>("Root 1/Child 1");

            // Bind
            rootScope.BindComponent<IDependency>().FromInstance(rootDependency);
            childScope.BindComponent<IDependency>().FromInstance(childDependency);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(rootDependency, Is.Not.Null);
            Assert.That(childDependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(childDependency));
            Assert.That(target.dependency, Is.Not.EqualTo(rootDependency));
        }

        [Test]
        public void Scope_AwakeAndOnDestroy_RegistersAndUnregistersGlobalComponents()
        {
            // Set up components
            GameObject root = new("Root");

            try
            {
                TestScope scope = root.AddComponent<TestScope>();
                ComponentDependency dependency = root.AddComponent<ComponentDependency>();
                FieldInfo allowUseInEditModeField = typeof(GlobalScope).GetField("allowUseInEditMode", BindingFlags.NonPublic | BindingFlags.Static);
                MethodInfo awakeMethod = typeof(Scope).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo onDestroyMethod = typeof(Scope).GetMethod("OnDestroy", BindingFlags.NonPublic | BindingFlags.Instance);

                Assert.That(allowUseInEditModeField, Is.Not.Null);
                Assert.That(awakeMethod, Is.Not.Null);
                Assert.That(onDestroyMethod, Is.Not.Null);

                bool previousAllowUseInEditMode = (bool)allowUseInEditModeField.GetValue(null);

                try
                {
                    // Update globals and invoke lifecycle
                    allowUseInEditModeField.SetValue(null, true);
                    GlobalScope.Clear();
                    scope.UpdateGlobalComponents(new[] { dependency });
                    awakeMethod.Invoke(scope, null);

                    // Assert
                    Assert.That(dependency, Is.Not.Null);
                    Assert.That(GlobalScope.IsRegistered<ComponentDependency>(), Is.True);
                    Assert.That(GlobalScope.GetComponent<ComponentDependency>(), Is.EqualTo(dependency));

                    // OnDestroy
                    onDestroyMethod.Invoke(scope, null);

                    // Assert
                    Assert.That(GlobalScope.IsRegistered<ComponentDependency>(), Is.False);
                    Assert.That(GlobalScope.GetComponent<ComponentDependency>(), Is.Null);
                }
                finally
                {
                    GlobalScope.Clear();
                    allowUseInEditModeField.SetValue(null, previousAllowUseInEditMode);
                }
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
    }
}
