using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Menus.SanejectMenuItems;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Menus
{
    public class CreateScopeMenuItemsTests
    {
        [TearDown]
        public void TearDown()
        {
            Selection.objects = Array.Empty<Object>();
        }

        [Test]
        public void GetSelectedFolder_GivenNoSelection_ReturnsAssetsFolderFullPath()
        {
            // Find helper
            MethodInfo getSelectedFolderMethod = typeof(CreateScopeMenuItems).GetMethod
            (
                "GetSelectedFolder",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            // Get selected folder
            string selectedFolder = getSelectedFolderMethod!.Invoke(null, null) as string;

            // Assert
            Assert.That(getSelectedFolderMethod, Is.Not.Null);
            Assert.That(selectedFolder, Is.EqualTo(Path.GetFullPath("Assets")));
        }

        [Test]
        public void GetSelectedFolder_GivenSelectedAssetFile_ReturnsContainingFolderFullPath()
        {
            // Set up asset
            AssetDependency dependency = ScriptableObject.CreateInstance<AssetDependency>();
            string assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Tests/Saneject/Fixtures/CreateScopeMenuItemsTests.asset");
            AssetDatabase.CreateAsset(dependency, assetPath);
            MethodInfo getSelectedFolderMethod = typeof(CreateScopeMenuItems).GetMethod
            (
                "GetSelectedFolder",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            try
            {
                // Select asset
                Selection.objects = new Object[] { dependency };

                // Get selected folder
                string selectedFolder = getSelectedFolderMethod!.Invoke(null, null) as string;

                // Assert
                Assert.That(getSelectedFolderMethod, Is.Not.Null);
                Assert.That(selectedFolder, Is.EqualTo(Path.GetFullPath(Path.GetDirectoryName(assetPath) ?? string.Empty)));
            }
            finally
            {
                AssetDatabase.DeleteAsset(assetPath);
            }
        }

        [Test]
        public void GetSelectedFolder_GivenSelectedFolder_ReturnsSelectedFolderFullPath()
        {
            // Set up folder
            Object folder = AssetDatabase.LoadAssetAtPath<Object>("Assets/Tests/Saneject/Fixtures");
            MethodInfo getSelectedFolderMethod = typeof(CreateScopeMenuItems).GetMethod
            (
                "GetSelectedFolder",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            // Select folder
            Selection.objects = new[] { folder };

            // Get selected folder
            string selectedFolder = getSelectedFolderMethod!.Invoke(null, null) as string;

            // Assert
            Assert.That(folder, Is.Not.Null);
            Assert.That(getSelectedFolderMethod, Is.Not.Null);
            Assert.That(selectedFolder, Is.EqualTo(Path.GetFullPath("Assets/Tests/Saneject/Fixtures")));
        }

        [Test]
        public void GetCodeString_GivenNoNamespace_ReturnsScopeClassWithoutNamespace()
        {
            // Find helper
            MethodInfo getCodeStringMethod = typeof(CreateScopeMenuItems).GetMethod
            (
                "GetCodeString",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            // Generate code
            string codeString = getCodeStringMethod!.Invoke(null, new object[] { "MyScope", string.Empty }) as string;

            // Assert
            Assert.That(getCodeStringMethod, Is.Not.Null);
            Assert.That(codeString?.Replace("\r\n", "\n"), Is.EqualTo(
$@"using Plugins.Saneject.Runtime.Scopes;

public class MyScope : Scope
{{
    protected override void DeclareBindings()
    {{
    }}
}}"));
        }

        [Test]
        public void GetCodeString_GivenNamespace_ReturnsScopeClassWithNamespace()
        {
            // Find helper
            MethodInfo getCodeStringMethod = typeof(CreateScopeMenuItems).GetMethod
            (
                "GetCodeString",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            // Generate code
            string codeString = getCodeStringMethod!.Invoke(null, new object[] { "MyScope", "namespace Tests.Generated" }) as string;

            // Assert
            Assert.That(getCodeStringMethod, Is.Not.Null);
            Assert.That(codeString?.Replace("\r\n", "\n"), Is.EqualTo(
$@"using Plugins.Saneject.Runtime.Scopes;

namespace Tests.Generated
{{
    public class MyScope : Scope
    {{
        protected override void DeclareBindings()
        {{
        }}
    }}
}}"));
        }
    }
}
