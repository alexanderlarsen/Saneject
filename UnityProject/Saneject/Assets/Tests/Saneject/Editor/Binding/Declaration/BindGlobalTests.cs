using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;

namespace Tests.Saneject.Editor.Binding.Declaration
{
    public class GlobalBindingDeclarationTests
    {
        [Test]
        public void BindGlobal_TConcrete_AddsTConcreteToScopeGlobalComponents()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");
            List<UnityEngine.Component> globalComponentsList = GetScopeGlobalComponentsList(scope);

            // Bind
            scope.BindGlobal<ComponentDependency>().FromSelf();

            // Inject
            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(dependency, Is.Not.Null);
            CollectionAssert.Contains(globalComponentsList, dependency);
        }

        private static List<UnityEngine.Component> GetScopeGlobalComponentsList(Scope scope)
        {
            FieldInfo fieldInfo = typeof(Scope).GetField("globalComponents", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(fieldInfo, Is.Not.Null);
            return fieldInfo.GetValue(scope) as List<UnityEngine.Component>;
        }
    }
}