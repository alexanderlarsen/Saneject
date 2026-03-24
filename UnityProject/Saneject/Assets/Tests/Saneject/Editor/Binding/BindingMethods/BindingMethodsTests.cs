using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Binding.BindingMethods
{
    public class BindingMethodsTests
    {
        [Test]
        public void BindComponent_TConcrete_InjectsConcrete_NotInterface()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));
            
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.AddToAllTransforms<ComponentDependency>();
            
            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteComponentTarget concreteTarget = scene.Add<ConcreteComponentTarget>("Root 1");
            InterfaceTarget interfaceTarget = scene.Add<InterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            scope.BindComponent<ComponentDependency>().FromSelf();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.NotNull(dependency);
            Assert.NotNull(concreteTarget.dependency);
            Assert.AreEqual(dependency, concreteTarget.dependency);
            Assert.IsNull(interfaceTarget.dependency);
        }
        
        [Test]
        public void BindComponent_TInterface_InjectsInterface_NotConcrete()
        {
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Missing binding"));
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Injection complete"));
            
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.AddToAllTransforms<ComponentDependency>();
            
            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteComponentTarget concreteTarget = scene.Add<ConcreteComponentTarget>("Root 1");
            InterfaceTarget interfaceTarget = scene.Add<InterfaceTarget>("Root 1");
            ComponentDependency dependency = scene.Get<ComponentDependency>("Root 1");

            scope.BindComponent<IDependency>().FromSelf();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.NotNull(dependency);
            Assert.NotNull(interfaceTarget.dependency);
            Assert.AreEqual(dependency, interfaceTarget.dependency);
            Assert.IsNull(concreteTarget.dependency);
        }

        [Test]
        public void BindComponents_TInterface_InjectsConcreteCollection_NotInterfaceCollection()
        {
            
        }
    }
}