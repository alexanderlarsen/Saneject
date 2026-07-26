using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.Context;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Injection
{
    public class InterfaceConcreteResolutionTests
    {
        [Test]
        public void Inject_TInterfaceTConcrete_WHEN_MultipleInterfaceImplementers_InjectsConcreteComponentToInterfaceField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 2);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1");

            // Find dependency
            scene.Add<ComponentDependency>("Root 1/Child 1");
            IDependency dependency = scene.Add<SecondComponentDependency>("Root 1/Child 2");

            // Bind
            scope.BindComponent<IDependency, SecondComponentDependency>().FromDescendants(includeSelf: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Inject_TConcrete_WHEN_CandidateIsSubclass_ResolvesSubclassToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");

            // Find dependency (a subclass of the bound concrete type)
            DerivedComponentDependency dependency = scene.Add<DerivedComponentDependency>("Root 1/Child 1/Child 1");

            // Bind the base type; resolution should behave like GetComponent<ComponentDependency>() and match the subclass
            scope.BindComponent<ComponentDependency>().FromDescendants(includeSelf: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void Inject_TInterfaceTConcrete_WHEN_CandidateIsSubclassOfConcrete_ResolvesSubclassFulfillingBoth()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleInterfaceTarget target = scene.Add<SingleInterfaceTarget>("Root 1/Child 1");

            // Find dependency (subclass of the bound concrete, also implements the interface)
            DerivedComponentDependency dependency = scene.Add<DerivedComponentDependency>("Root 1/Child 1/Child 1");

            // Bind interface + concrete; the candidate must be assignable to both
            scope.BindComponent<IDependency, ComponentDependency>().FromDescendants(includeSelf: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}