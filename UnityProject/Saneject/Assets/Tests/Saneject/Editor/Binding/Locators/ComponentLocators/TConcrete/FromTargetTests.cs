using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators.TConcrete
{
    public class FromTargetTests
    {
        [Test]
        public void FromTargetSelf_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2");
        
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");

            // Bind
            scope.BindComponent<ComponentDependency>().FromTargetSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetParent_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2/Child 1");
        
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");

            // Bind
            scope.BindComponent<ComponentDependency>().FromTargetParent();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetAncestors_WHEN_IncludeSelfIsFalse_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1/Child 1");
            
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromTargetAncestors(includeSelf: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetAncestors_WHEN_IncludeSelfIsTrue_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1/Child 1");
      
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromTargetAncestors(includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetFirstChild_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
           
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromTargetFirstChild();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetLastChild_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
        
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 3");

            // Bind
            scope.BindComponent<ComponentDependency>().FromTargetLastChild();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetChildWithIndex_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1"); 
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
      
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");

            // Bind
            scope.BindComponent<ComponentDependency>().FromTargetChildWithIndex(1);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetDescendants_WHEN_IncludeSelfIsFalse_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
         
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromTargetDescendants(includeSelf: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetDescendants_WHEN_IncludeSelfIsTrue_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
          
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromTargetDescendants(includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromTargetSiblings_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2/Child 3");
          
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromTargetSiblings();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}
