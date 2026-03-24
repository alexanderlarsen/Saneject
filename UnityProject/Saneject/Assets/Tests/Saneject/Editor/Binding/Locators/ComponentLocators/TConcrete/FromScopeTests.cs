using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators.TConcrete
{
    public class FromScopeTests
    {
        [Test]
        public void FromScopeSelf_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
    
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeParent_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1/Child 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
      
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromParent();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeAncestors_WHEN_IncludeSelfIsFalse_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1/Child 1/Child 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1/Child 1");
         
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromAncestors(includeSelf: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeAncestors_WHEN_IncludeSelfIsTrue_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1/Child 1/Child 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1/Child 1");
        
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromAncestors(includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeFirstChild_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
          
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromFirstChild();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeLastChild_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
        
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 3");

            // Bind
            scope.BindComponent<ComponentDependency>().FromLastChild();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeChildWithIndex_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
          
            // Find dependency
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
      
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 2");

            // Bind
            scope.BindComponent<ComponentDependency>().FromChildWithIndex(1);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeDescendants_WHEN_IncludeSelfIsFalse_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToLeafs<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 1");
       
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromDescendants(includeSelf: false);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeDescendants_WHEN_IncludeSelfIsTrue_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1");
   
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromDescendants(includeSelf: true);

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }

        [Test]
        public void FromScopeSiblings_TConcrete_InjectsToConcreteField()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();
            TestScope scope = scene.Add<TestScope>("Root 1/Child 2");
            SingleConcreteComponentTarget target = scene.Add<SingleConcreteComponentTarget>("Root 1/Child 2");
       
            // Find dependency
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1/Child 1");

            // Bind
            scope.BindComponent<ComponentDependency>().FromSiblings();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            Assert.That(target.dependency, Is.EqualTo(dependency));
        }
    }
}
