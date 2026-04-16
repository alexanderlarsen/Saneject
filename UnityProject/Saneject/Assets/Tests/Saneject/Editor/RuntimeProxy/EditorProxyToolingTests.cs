using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Proxy;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Settings;
using Tests.Saneject.Fixtures.Scripts.RuntimeProxy;
using UnityEditor;
using UnityEngine;

namespace Tests.Saneject.Editor.RuntimeProxy
{
    public class EditorProxyToolingTests
    {
        private const string TestAssetFolder = "Assets/Tests/Saneject/Fixtures/RuntimeProxyToolingAssets";

        private bool originalGenerateProxyScriptsOnDomainReload;
        private bool originalLogUnusedRuntimeProxiesOnDomainReload;
        private string originalProxyAssetGenerationFolder;

        [SetUp]
        public void SetUp()
        {
            originalGenerateProxyScriptsOnDomainReload = ProjectSettings.GenerateProxyScriptsOnDomainReload;
            originalLogUnusedRuntimeProxiesOnDomainReload = UserSettings.LogUnusedRuntimeProxiesOnDomainReload;
            originalProxyAssetGenerationFolder = ProjectSettings.ProxyAssetGenerationFolder;

            ProjectSettings.GenerateProxyScriptsOnDomainReload = false;
            UserSettings.LogUnusedRuntimeProxiesOnDomainReload = false;
            ProjectSettings.ProxyAssetGenerationFolder = TestAssetFolder;

            string[] generatedProxyGuids = AssetDatabase.FindAssets($"t:{nameof(GeneratedRuntimeProxy)}");

            foreach (string guid in generatedProxyGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            string[] unusedProxyGuids = AssetDatabase.FindAssets($"t:{nameof(UnusedRuntimeProxy)}");

            foreach (string guid in unusedProxyGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            string[] mutedUnusedProxyGuids = AssetDatabase.FindAssets($"t:{nameof(MutedUnusedRuntimeProxy)}");

            foreach (string guid in mutedUnusedProxyGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            string[] holderGuids = AssetDatabase.FindAssets($"t:{nameof(RuntimeProxyAssetReferenceHolder)}");

            foreach (string guid in holderGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            AssetDatabase.DeleteAsset(TestAssetFolder);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [TearDown]
        public void TearDown()
        {
            ProjectSettings.GenerateProxyScriptsOnDomainReload = originalGenerateProxyScriptsOnDomainReload;
            UserSettings.LogUnusedRuntimeProxiesOnDomainReload = originalLogUnusedRuntimeProxiesOnDomainReload;
            ProjectSettings.ProxyAssetGenerationFolder = originalProxyAssetGenerationFolder;

            string[] generatedProxyGuids = AssetDatabase.FindAssets($"t:{nameof(GeneratedRuntimeProxy)}");

            foreach (string guid in generatedProxyGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            string[] unusedProxyGuids = AssetDatabase.FindAssets($"t:{nameof(UnusedRuntimeProxy)}");

            foreach (string guid in unusedProxyGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            string[] mutedUnusedProxyGuids = AssetDatabase.FindAssets($"t:{nameof(MutedUnusedRuntimeProxy)}");

            foreach (string guid in mutedUnusedProxyGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            string[] holderGuids = AssetDatabase.FindAssets($"t:{nameof(RuntimeProxyAssetReferenceHolder)}");

            foreach (string guid in holderGuids)
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

            AssetDatabase.DeleteAsset(TestAssetFolder);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Test]
        public void EnumerateManifestTypes_GivenRuntimeProxyBindings_ReturnsRequiredConcreteTypes()
        {
            // Find manifest types
            Type[] manifestTypes = RuntimeProxyManifestUtility.EnumerateManifestTypes().ToArray();

            // Assert
            CollectionAssert.Contains(manifestTypes, typeof(RuntimeProxyTargetComponent));
            Assert.That(manifestTypes.Count(type => type == typeof(RuntimeProxyTargetComponent)), Is.EqualTo(1));
        }

        [Test]
        public void IsMissingScript_GivenExistingProxyStub_ReturnsFalse()
        {
            // Find helper
            MethodInfo isMissingScriptMethod = typeof(RuntimeProxyScriptGenerator).GetMethod("IsMissingScript", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.That(isMissingScriptMethod, Is.Not.Null);

            // Check proxy stub
            bool isMissingScript = (bool)isMissingScriptMethod.Invoke(null, new object[] { typeof(RuntimeProxyTargetComponent) });

            // Assert
            Assert.That(isMissingScript, Is.False);
        }

        [Test]
        public void IsMissingScript_GivenMissingProxyStub_ReturnsTrue()
        {
            // Find helper
            MethodInfo isMissingScriptMethod = typeof(RuntimeProxyScriptGenerator).GetMethod("IsMissingScript", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.That(isMissingScriptMethod, Is.Not.Null);

            // Check proxy stub
            bool isMissingScript = (bool)isMissingScriptMethod.Invoke(null, new object[] { typeof(MissingProxyStubComponent) });

            // Assert
            Assert.That(isMissingScript, Is.True);
        }

        [Test]
        public void GetScriptCode_GivenType_ReturnsExpectedRuntimeProxyScript()
        {
            // Find helper
            MethodInfo getScriptCodeMethod = typeof(RuntimeProxyScriptGenerator).GetMethod("GetScriptCode", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.That(getScriptCodeMethod, Is.Not.Null);

            // Generate script code
            string scriptCode = getScriptCodeMethod.Invoke
            (
                null,
                new object[]
                {
                    "GeneratedProxy",
                    typeof(RuntimeProxyTargetComponent).FullName
                }
            ) as string;

            // Assert
            Assert.That(scriptCode, Is.Not.Null);
            Assert.That(scriptCode, Does.Contain("namespace Tests.Saneject.Fixtures.RuntimeProxyToolingAssets"));
            Assert.That(scriptCode, Does.Contain("[Plugins.Saneject.Runtime.Attributes.GenerateRuntimeProxy]"));
            Assert.That(scriptCode, Does.Contain("public partial class GeneratedProxy : Plugins.Saneject.Runtime.Proxy.RuntimeProxy<Tests.Saneject.Fixtures.Scripts.RuntimeProxy.RuntimeProxyTargetComponent>"));
        }

        [Test]
        public void GetScriptCode_GivenNestedTypeName_ReplacesPlusWithDot()
        {
            // Find helper
            MethodInfo getScriptCodeMethod = typeof(RuntimeProxyScriptGenerator).GetMethod("GetScriptCode", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.That(getScriptCodeMethod, Is.Not.Null);

            // Generate script code
            string scriptCode = getScriptCodeMethod.Invoke
            (
                null,
                new object[]
                {
                    "NestedProxy",
                    "Tests.Saneject.Fixtures.Scripts.RuntimeProxy.Outer+NestedProxyTargetComponent"
                }
            ) as string;

            // Assert
            Assert.That(scriptCode, Is.Not.Null);
            Assert.That(scriptCode, Does.Contain("Plugins.Saneject.Runtime.Proxy.RuntimeProxy<Tests.Saneject.Fixtures.Scripts.RuntimeProxy.Outer.NestedProxyTargetComponent>"));
        }

        [Test]
        public void StableHash_GivenSameSimpleNameDifferentNamespaces_ReturnsDifferentValues()
        {
            // Find helper
            MethodInfo stableHashMethod = typeof(RuntimeProxyScriptGenerator).GetMethod("StableHash", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.That(stableHashMethod, Is.Not.Null);

            // Hash names
            string firstHash = stableHashMethod.Invoke(null, new object[] { "Tests.Saneject.First.SharedName" }) as string;
            string secondHash = stableHashMethod.Invoke(null, new object[] { "Tests.Saneject.Second.SharedName" }) as string;

            // Assert
            Assert.That(firstHash, Is.Not.Null);
            Assert.That(secondHash, Is.Not.Null);
            Assert.That(firstHash, Has.Length.EqualTo(8));
            Assert.That(secondHash, Has.Length.EqualTo(8));
            Assert.That(firstHash, Is.Not.EqualTo(secondHash));
        }

        [Test]
        public void RuntimeProxyCleaner_GivenUnusedUnmutedProxyType_FindsType()
        {
            // Find helper
            MethodInfo getUnusedTypesMethod = typeof(RuntimeProxyCleaner).GetMethod("GetUnusedTypes", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.That(getUnusedTypesMethod, Is.Not.Null);

            // Find unused types
            HashSet<Type> unusedTypes = getUnusedTypesMethod.Invoke(null, null) as HashSet<Type>;

            // Assert
            Assert.That(unusedTypes, Is.Not.Null);
            CollectionAssert.Contains(unusedTypes, typeof(UnusedRuntimeProxy));
        }

        [Test]
        public void RuntimeProxyCleaner_GivenMutedUnusedProxyType_DoesNotFindType()
        {
            // Find helper
            MethodInfo getUnusedTypesMethod = typeof(RuntimeProxyCleaner).GetMethod("GetUnusedTypes", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.That(getUnusedTypesMethod, Is.Not.Null);

            // Find unused types
            HashSet<Type> unusedTypes = getUnusedTypesMethod.Invoke(null, null) as HashSet<Type>;

            // Assert
            Assert.That(unusedTypes, Is.Not.Null);
            CollectionAssert.DoesNotContain(unusedTypes, typeof(MutedUnusedRuntimeProxy));
        }

        [Test]
        public void RuntimeProxyCleaner_GivenReferencedProxyAsset_DoesNotMarkAssetUnused()
        {
            // Set up assets
            AssetDatabase.CreateFolder("Assets/Tests/Saneject/Fixtures", "RuntimeProxyToolingAssets");

            GeneratedRuntimeProxy proxy = ScriptableObject.CreateInstance<GeneratedRuntimeProxy>();

            proxy.AssignConfig(new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromGlobalScope,
                instanceMode: RuntimeProxyInstanceMode.Singleton,
                prefab: null,
                dontDestroyOnLoad: false
            ));

            RuntimeProxyAssetReferenceHolder holder = ScriptableObject.CreateInstance<RuntimeProxyAssetReferenceHolder>();
            holder.proxy = proxy;

            string proxyPath = $"{TestAssetFolder}/Referenced Proxy.asset";
            AssetDatabase.CreateAsset(proxy, proxyPath);
            AssetDatabase.CreateAsset(holder, $"{TestAssetFolder}/Referenced Holder.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Find cleaner results
            MethodInfo getUnusedTypesMethod = typeof(RuntimeProxyCleaner).GetMethod("GetUnusedTypes", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo getUnusedAssetPathsMethod = typeof(RuntimeProxyCleaner).GetMethod("GetUnusedAssetPaths", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.That(getUnusedTypesMethod, Is.Not.Null);
            Assert.That(getUnusedAssetPathsMethod, Is.Not.Null);

            HashSet<Type> unusedTypes = getUnusedTypesMethod.Invoke(null, null) as HashSet<Type>;
            HashSet<string> unusedAssetPaths = getUnusedAssetPathsMethod.Invoke(null, new object[] { unusedTypes }) as HashSet<string>;

            // Assert
            Assert.That(unusedTypes, Is.Not.Null);
            Assert.That(unusedAssetPaths, Is.Not.Null);
            CollectionAssert.DoesNotContain(unusedTypes, typeof(GeneratedRuntimeProxy));
            CollectionAssert.DoesNotContain(unusedAssetPaths, proxyPath);
        }

        [Test]
        public void RuntimeProxyCleaner_GivenUnreferencedProxyAsset_MarksAssetUnused()
        {
            // Set up asset
            AssetDatabase.CreateFolder("Assets/Tests/Saneject/Fixtures", "RuntimeProxyToolingAssets");

            GeneratedRuntimeProxy proxy = ScriptableObject.CreateInstance<GeneratedRuntimeProxy>();

            proxy.AssignConfig(new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromGlobalScope,
                instanceMode: RuntimeProxyInstanceMode.Singleton,
                prefab: null,
                dontDestroyOnLoad: false
            ));

            string proxyPath = $"{TestAssetFolder}/Unreferenced Proxy.asset";
            AssetDatabase.CreateAsset(proxy, proxyPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Find cleaner results
            MethodInfo getUnusedTypesMethod = typeof(RuntimeProxyCleaner).GetMethod("GetUnusedTypes", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo getUnusedAssetPathsMethod = typeof(RuntimeProxyCleaner).GetMethod("GetUnusedAssetPaths", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.That(getUnusedTypesMethod, Is.Not.Null);
            Assert.That(getUnusedAssetPathsMethod, Is.Not.Null);

            HashSet<Type> unusedTypes = getUnusedTypesMethod.Invoke(null, null) as HashSet<Type>;
            HashSet<string> unusedAssetPaths = getUnusedAssetPathsMethod.Invoke(null, new object[] { unusedTypes }) as HashSet<string>;

            // Assert
            Assert.That(unusedTypes, Is.Not.Null);
            Assert.That(unusedAssetPaths, Is.Not.Null);
            CollectionAssert.DoesNotContain(unusedTypes, typeof(GeneratedRuntimeProxy));
            CollectionAssert.Contains(unusedAssetPaths, proxyPath);
        }
    }
}