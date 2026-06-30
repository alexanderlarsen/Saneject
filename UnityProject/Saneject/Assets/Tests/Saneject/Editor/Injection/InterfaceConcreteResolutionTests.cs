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
    }
}