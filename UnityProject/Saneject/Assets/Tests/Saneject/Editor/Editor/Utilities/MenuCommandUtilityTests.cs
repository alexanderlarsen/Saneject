using System;
using NUnit.Framework;
using Plugins.Saneject.Editor.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Utilities
{
    public class MenuCommandUtilityTests
    {
        [TearDown]
        public void TearDown()
        {
            Selection.objects = Array.Empty<Object>();
        }

        [Test]
        public void IsFirstInvocation_GivenNoSelection_ReturnsTrue()
        {
            // Set up command
            MenuCommand cmd = new(null);

            // Check first invocation
            bool isFirstInvocation = MenuCommandUtility.IsFirstInvocation(cmd);

            // Assert
            Assert.That(isFirstInvocation, Is.True);
        }

        [Test]
        public void IsFirstInvocation_GivenSingleSelection_ReturnsTrue()
        {
            // Set up object
            GameObject firstObject = new("First");

            try
            {
                // Select object
                Selection.objects = new Object[] { firstObject };

                // Check first invocation
                bool isFirstInvocation = MenuCommandUtility.IsFirstInvocation(new MenuCommand(firstObject));

                // Assert
                Assert.That(isFirstInvocation, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(firstObject);
            }
        }

        [Test]
        public void IsFirstInvocation_GivenFirstSelectedContext_ReturnsTrue()
        {
            // Set up objects
            GameObject firstObject = new("First");
            GameObject secondObject = new("Second");

            try
            {
                // Select objects
                Selection.objects = new Object[]
                {
                    firstObject,
                    secondObject
                };

                // Check first invocation
                bool isFirstInvocation = MenuCommandUtility.IsFirstInvocation(new MenuCommand(firstObject));

                // Assert
                Assert.That(isFirstInvocation, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(firstObject);
                Object.DestroyImmediate(secondObject);
            }
        }

        [Test]
        public void IsFirstInvocation_GivenNonFirstSelectedContext_ReturnsFalse()
        {
            // Set up objects
            GameObject firstObject = new("First");
            GameObject secondObject = new("Second");

            try
            {
                // Select objects
                Selection.objects = new Object[]
                {
                    firstObject,
                    secondObject
                };

                // Check first invocation
                bool isFirstInvocation = MenuCommandUtility.IsFirstInvocation(new MenuCommand(secondObject));

                // Assert
                Assert.That(isFirstInvocation, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(firstObject);
                Object.DestroyImmediate(secondObject);
            }
        }
    }
}
