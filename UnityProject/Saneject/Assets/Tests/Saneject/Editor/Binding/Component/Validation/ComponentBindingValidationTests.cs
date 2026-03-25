using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Binding.Component.Validation
{
    public class ComponentBindingValidationTests
    {
        [Test]
        public void BindComponent_TConcreteTConcrete_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency, ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindComponent_TConcrete_WithoutLocatorStrategy_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindComponent_TAsset_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<AssetDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindComponent_TConcrete_FromRuntimeProxy_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromRuntimeProxy();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindComponent_TInterface_FromRuntimeProxy_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<IDependency>().FromRuntimeProxy();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindMultipleComponents_TConcreteTConcrete_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 5);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponents<ComponentDependency, ComponentDependency>().FromDescendants(includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindMultipleComponents_TInterfaceTConcrete_FromRuntimeProxy_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 5);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponents<IDependency, ComponentDependency>().FromRuntimeProxy();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindComponent_TConcrete_DuplicateWithinSameScope_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }

        [Test]
        public void BindGlobal_TConcrete_DuplicateAcrossScopes_IsInvalid()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Invalid binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));

            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 1, depth: 1);
            TestScope firstScope = scene.Add<TestScope>("Root 1");
            TestScope secondScope = scene.Add<TestScope>("Root 2");
            scene.Add<ComponentDependency>("Root 1");
            scene.Add<ComponentDependency>("Root 2");

            // Bind
            firstScope.BindGlobal<ComponentDependency>().FromSelf();
            secondScope.BindGlobal<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
        }
    }
}