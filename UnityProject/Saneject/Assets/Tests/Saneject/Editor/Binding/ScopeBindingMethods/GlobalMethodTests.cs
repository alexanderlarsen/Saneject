using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.ScopeBindingMethods
{
    public class GlobalMethodTests
    {
        [Test]
        public void BindGlobal_TConcrete_AddsTConcreteToScopeGlobalComponents()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestScope scope = scene.Add<TestScope>("Root 1");
            ComponentDependency dependency = scene.Add<ComponentDependency>("Root 1");
            List<Component> globalComponentsList = GetScopeGlobalComponentsList(scope);

            scope.BindGlobal<ComponentDependency>().FromSelf();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);

            Assert.That(dependency, Is.Not.Null);
            CollectionAssert.Contains(globalComponentsList, dependency);
        }

        private static List<Component> GetScopeGlobalComponentsList(Scope scope)
        {
            FieldInfo fieldInfo = typeof(Scope).GetField("globalComponents", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(fieldInfo, Is.Not.Null);
            return fieldInfo.GetValue(scope) as List<Component>;
        }
    }
}