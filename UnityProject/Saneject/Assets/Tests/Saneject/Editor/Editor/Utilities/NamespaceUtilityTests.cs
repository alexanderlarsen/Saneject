using System.IO;
using NUnit.Framework;
using Plugins.Saneject.Editor.Utilities;
using UnityEngine;

namespace Tests.Saneject.Editor.Editor.Utilities
{
    public class NamespaceUtilityTests
    {
        [Test]
        public void GetNamespaceFromFullPath_GivenNestedAssetsPath_ReturnsSanitizedNamespace()
        {
            // Build path
            string fullPath = Path.Combine
            (
                Path.GetFullPath(Application.dataPath),
                "Tests",
                "Saneject",
                "Fixtures",
                "123 Invalid-Folder",
                "@Special Folder"
            );

            // Find namespace
            string namespaceString = NamespaceUtility.GetNamespaceFromFullPath(fullPath);

            // Assert
            Assert.That(namespaceString, Is.EqualTo("namespace Tests.Saneject.Fixtures._123InvalidFolder.SpecialFolder"));
        }

        [Test]
        public void GetNamespaceFromFullPath_GivenAssetsRoot_ReturnsNull()
        {
            // Find namespace
            string namespaceString = NamespaceUtility.GetNamespaceFromFullPath(Path.GetFullPath(Application.dataPath));

            // Assert
            Assert.That(namespaceString, Is.Null);
        }

        [Test]
        public void GetNamespaceFromAssetsRelativePath_GivenNestedAssetsPath_ReturnsSanitizedNamespace()
        {
            // Find namespace
            string namespaceString = NamespaceUtility.GetNamespaceFromAssetsRelativePath
            (
                "Assets/Tests/Saneject/Fixtures/123 Invalid-Folder/@Special Folder"
            );

            // Assert
            Assert.That(namespaceString, Is.EqualTo("namespace Tests.Saneject.Fixtures._123InvalidFolder.SpecialFolder"));
        }

        [Test]
        public void GetNamespaceFromAssetsRelativePath_GivenOnlyInvalidSegments_ReturnsNull()
        {
            // Find namespace
            string namespaceString = NamespaceUtility.GetNamespaceFromAssetsRelativePath("Assets/---/$$$");

            // Assert
            Assert.That(namespaceString, Is.Null);
        }
    }
}
