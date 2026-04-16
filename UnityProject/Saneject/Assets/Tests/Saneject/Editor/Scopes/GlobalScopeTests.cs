using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Scopes
{
    public class GlobalScopeTests
    {
        private FieldInfo allowUseInEditModeField;
        private bool previousAllowUseInEditMode;

        [SetUp]
        public void SetUp()
        {
            allowUseInEditModeField = typeof(GlobalScope).GetField("allowUseInEditMode", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.That(allowUseInEditModeField, Is.Not.Null);

            previousAllowUseInEditMode = (bool)allowUseInEditModeField.GetValue(null);
            allowUseInEditModeField.SetValue(null, true);
            GlobalScope.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            GlobalScope.Clear();
            allowUseInEditModeField.SetValue(null, previousAllowUseInEditMode);
        }

        [Test]
        public void RegisterComponent_GivenUnregisteredComponent_RegistersComponent()
        {
            // Set up component
            GameObject caller = new("Caller");
            GameObject dependencyObject = new("Dependency");
            ComponentDependency dependency = dependencyObject.AddComponent<ComponentDependency>();

            try
            {
                // Register
                GlobalScope.RegisterComponent(dependency, caller);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(GlobalScope.IsRegistered<ComponentDependency>(), Is.True);
                Assert.That(GlobalScope.GetComponent<ComponentDependency>(), Is.EqualTo(dependency));
            }
            finally
            {
                Object.DestroyImmediate(caller);
                Object.DestroyImmediate(dependencyObject);
            }
        }

        [Test]
        public void UnregisterComponent_GivenRegisteredComponentAndOriginalCaller_UnregistersComponent()
        {
            // Set up component
            GameObject caller = new("Caller");
            GameObject dependencyObject = new("Dependency");
            ComponentDependency dependency = dependencyObject.AddComponent<ComponentDependency>();

            try
            {
                // Register and unregister
                GlobalScope.RegisterComponent(dependency, caller);
                GlobalScope.UnregisterComponent(dependency, caller);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(GlobalScope.IsRegistered<ComponentDependency>(), Is.False);
                Assert.That(GlobalScope.GetComponent<ComponentDependency>(), Is.Null);
            }
            finally
            {
                Object.DestroyImmediate(caller);
                Object.DestroyImmediate(dependencyObject);
            }
        }

        [Test]
        public void GetComponent_GivenRegisteredComponent_ReturnsComponent()
        {
            // Set up component
            GameObject caller = new("Caller");
            GameObject dependencyObject = new("Dependency");
            ComponentDependency dependency = dependencyObject.AddComponent<ComponentDependency>();

            try
            {
                // Register and get
                GlobalScope.RegisterComponent(dependency, caller);
                ComponentDependency result = GlobalScope.GetComponent<ComponentDependency>();

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo(dependency));
            }
            finally
            {
                Object.DestroyImmediate(caller);
                Object.DestroyImmediate(dependencyObject);
            }
        }

        [Test]
        public void TryGetComponent_GivenRegisteredComponent_ReturnsTrueAndOutputsComponent()
        {
            // Set up component
            GameObject caller = new("Caller");
            GameObject dependencyObject = new("Dependency");
            ComponentDependency dependency = dependencyObject.AddComponent<ComponentDependency>();

            try
            {
                // Register and try get
                GlobalScope.RegisterComponent(dependency, caller);
                bool found = GlobalScope.TryGetComponent(out ComponentDependency result);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(found, Is.True);
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo(dependency));
            }
            finally
            {
                Object.DestroyImmediate(caller);
                Object.DestroyImmediate(dependencyObject);
            }
        }

        [Test]
        public void TryGetComponent_GivenUnregisteredComponent_ReturnsFalseAndOutputsNull()
        {
            // Try get
            bool found = GlobalScope.TryGetComponent(out ComponentDependency result);

            // Assert
            Assert.That(found, Is.False);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Clear_GivenRegisteredComponents_RemovesAllComponents()
        {
            // Set up components
            GameObject firstCaller = new("First Caller");
            GameObject secondCaller = new("Second Caller");
            GameObject firstDependencyObject = new("First Dependency");
            GameObject secondDependencyObject = new("Second Dependency");
            ComponentDependency componentDependency = firstDependencyObject.AddComponent<ComponentDependency>();
            SingleConcreteComponentTarget targetDependency = secondDependencyObject.AddComponent<SingleConcreteComponentTarget>();

            try
            {
                // Register and clear
                GlobalScope.RegisterComponent(componentDependency, firstCaller);
                GlobalScope.RegisterComponent(targetDependency, secondCaller);
                GlobalScope.Clear();

                // Assert
                Assert.That(componentDependency, Is.Not.Null);
                Assert.That(targetDependency, Is.Not.Null);
                Assert.That(GlobalScope.IsRegistered<ComponentDependency>(), Is.False);
                Assert.That(GlobalScope.IsRegistered<SingleConcreteComponentTarget>(), Is.False);
                Assert.That(GlobalScope.GetComponent<ComponentDependency>(), Is.Null);
                Assert.That(GlobalScope.GetComponent<SingleConcreteComponentTarget>(), Is.Null);
            }
            finally
            {
                Object.DestroyImmediate(firstCaller);
                Object.DestroyImmediate(secondCaller);
                Object.DestroyImmediate(firstDependencyObject);
                Object.DestroyImmediate(secondDependencyObject);
            }
        }

        [Test]
        public void UnregisterComponent_GivenDifferentCaller_DoesNotUnregisterComponent()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: GlobalScope unregistration failed\\."));

            // Set up component
            GameObject originalCaller = new("Original Caller");
            GameObject differentCaller = new("Different Caller");
            GameObject dependencyObject = new("Dependency");
            ComponentDependency dependency = dependencyObject.AddComponent<ComponentDependency>();

            try
            {
                // Register and unregister
                GlobalScope.RegisterComponent(dependency, originalCaller);
                GlobalScope.UnregisterComponent(dependency, differentCaller);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(GlobalScope.IsRegistered<ComponentDependency>(), Is.True);
                Assert.That(GlobalScope.GetComponent<ComponentDependency>(), Is.EqualTo(dependency));
            }
            finally
            {
                Object.DestroyImmediate(originalCaller);
                Object.DestroyImmediate(differentCaller);
                Object.DestroyImmediate(dependencyObject);
            }
        }

        [Test]
        public void RegisterComponent_GivenDuplicateComponentType_DoesNotReplaceOriginalComponent()
        {
            // Expect logs
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: GlobalScope registration failed\\."));

            // Set up components
            GameObject originalCaller = new("Original Caller");
            GameObject duplicateCaller = new("Duplicate Caller");
            GameObject originalDependencyObject = new("Original Dependency");
            GameObject duplicateDependencyObject = new("Duplicate Dependency");
            ComponentDependency originalDependency = originalDependencyObject.AddComponent<ComponentDependency>();
            ComponentDependency duplicateDependency = duplicateDependencyObject.AddComponent<ComponentDependency>();

            try
            {
                // Register duplicates
                GlobalScope.RegisterComponent(originalDependency, originalCaller);
                GlobalScope.RegisterComponent(duplicateDependency, duplicateCaller);

                // Assert
                Assert.That(originalDependency, Is.Not.Null);
                Assert.That(duplicateDependency, Is.Not.Null);
                Assert.That(GlobalScope.IsRegistered<ComponentDependency>(), Is.True);
                Assert.That(GlobalScope.GetComponent<ComponentDependency>(), Is.EqualTo(originalDependency));
                Assert.That(GlobalScope.GetComponent<ComponentDependency>(), Is.Not.EqualTo(duplicateDependency));
            }
            finally
            {
                Object.DestroyImmediate(originalCaller);
                Object.DestroyImmediate(duplicateCaller);
                Object.DestroyImmediate(originalDependencyObject);
                Object.DestroyImmediate(duplicateDependencyObject);
            }
        }
    }
}
