using NUnit.Framework;
using Plugins.Saneject.Editor.Utilities;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Utilities
{
    public class PathUtilityTests
    {
        [Test]
        public void SanitizeFolderPath_GivenNullOrEmpty_ReturnsOriginalValue()
        {
            // Sanitize paths
            string nullPath = PathUtility.SanitizeFolderPath(null);
            string emptyPath = PathUtility.SanitizeFolderPath(string.Empty);

            // Assert
            Assert.That(nullPath, Is.Null);
            Assert.That(emptyPath, Is.EqualTo(string.Empty));
        }

        [Test]
        public void SanitizeFolderPath_GivenIllegalCharsAndBackslashes_NormalizesAndRemovesInvalidCharacters()
        {
            // Sanitize path
            string sanitizedPath = PathUtility.SanitizeFolderPath(@"Assets\Tests\Bad<Folder>|Name\Sub?Folder");

            // Assert
            Assert.That(sanitizedPath, Is.EqualTo("Assets/Tests/BadFolderName/SubFolder"));
        }

        [Test]
        public void GetComponentPath_GivenNestedComponent_ReturnsTransformPathAndComponentName()
        {
            // Set up objects
            GameObject root = new("Root");

            try
            {
                GameObject child = new("Child");
                child.transform.SetParent(root.transform, false);
                GameObject leaf = new("Leaf");
                leaf.transform.SetParent(child.transform, false);
                ComponentDependency dependency = leaf.AddComponent<ComponentDependency>();

                // Find component path
                string componentPath = PathUtility.GetComponentPath(dependency);

                // Assert
                Assert.That(dependency, Is.Not.Null);
                Assert.That(componentPath, Is.EqualTo("Root/Child/Leaf/ComponentDependency"));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
    }
}
