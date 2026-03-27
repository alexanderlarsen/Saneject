using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Runtime.Proxy;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.RuntimeProxy
{
    public class RuntimeProxyCoreTests
    {
        [Test]
        public void AssignConfig_GivenConfig_CopiesAllValues()
        {
            // Set up proxy
            TestRuntimeProxy proxy = ScriptableObject.CreateInstance<TestRuntimeProxy>();
            GameObject prefab = new("Proxy Prefab");

            try
            {
                RuntimeProxyConfig config = new
                (
                    resolveMethod: RuntimeProxyResolveMethod.FromComponentOnPrefab,
                    instanceMode: RuntimeProxyInstanceMode.Singleton,
                    prefab: prefab,
                    dontDestroyOnLoad: true
                );

                // Assign
                proxy.AssignConfig(config);

                // Find fields
                FieldInfo resolveMethodField = typeof(RuntimeProxyBase).GetField("resolveMethod", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo instanceModeField = typeof(RuntimeProxyBase).GetField("instanceMode", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo prefabField = typeof(RuntimeProxyBase).GetField("prefab", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo dontDestroyOnLoadField = typeof(RuntimeProxyBase).GetField("dontDestroyOnLoad", BindingFlags.NonPublic | BindingFlags.Instance);

                // Assert
                Assert.That(resolveMethodField, Is.Not.Null);
                Assert.That(instanceModeField, Is.Not.Null);
                Assert.That(prefabField, Is.Not.Null);
                Assert.That(dontDestroyOnLoadField, Is.Not.Null);
                Assert.That(resolveMethodField.GetValue(proxy), Is.EqualTo(RuntimeProxyResolveMethod.FromComponentOnPrefab));
                Assert.That(instanceModeField.GetValue(proxy), Is.EqualTo(RuntimeProxyInstanceMode.Singleton));
                Assert.That(prefabField.GetValue(proxy), Is.EqualTo(prefab));
                Assert.That(dontDestroyOnLoadField.GetValue(proxy), Is.EqualTo(true));
                Assert.That(proxy.HasConfig(config), Is.True);

                Assert.That(proxy.HasConfig(new RuntimeProxyConfig
                (
                    resolveMethod: RuntimeProxyResolveMethod.FromGlobalScope,
                    instanceMode: RuntimeProxyInstanceMode.Singleton,
                    prefab: prefab,
                    dontDestroyOnLoad: true
                )), Is.False);

                Assert.That(proxy.HasConfig(new RuntimeProxyConfig
                (
                    resolveMethod: RuntimeProxyResolveMethod.FromComponentOnPrefab,
                    instanceMode: RuntimeProxyInstanceMode.Transient,
                    prefab: prefab,
                    dontDestroyOnLoad: true
                )), Is.False);

                Assert.That(proxy.HasConfig(new RuntimeProxyConfig
                (
                    resolveMethod: RuntimeProxyResolveMethod.FromComponentOnPrefab,
                    instanceMode: RuntimeProxyInstanceMode.Singleton,
                    prefab: null,
                    dontDestroyOnLoad: true
                )), Is.False);

                Assert.That(proxy.HasConfig(new RuntimeProxyConfig
                (
                    resolveMethod: RuntimeProxyResolveMethod.FromComponentOnPrefab,
                    instanceMode: RuntimeProxyInstanceMode.Singleton,
                    prefab: prefab,
                    dontDestroyOnLoad: false
                )), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(proxy);
                Object.DestroyImmediate(prefab);
            }
        }

        [Test]
        public void ResolveInstance_GivenEditMode_ReturnsNullAndLogsWarning()
        {
            // Expect logs
            LogAssert.Expect(LogType.Warning, new Regex("^Saneject: 'TestRuntimeProxy\\.ResolveInstance\\(\\)' called in editor\\. This is not allowed\\.$"));

            // Set up proxy
            TestRuntimeProxy proxy = ScriptableObject.CreateInstance<TestRuntimeProxy>();

            try
            {
                // Resolve
                object resolved = proxy.ResolveInstance();

                // Assert
                Assert.That(resolved, Is.Null);
            }
            finally
            {
                Object.DestroyImmediate(proxy);
            }
        }

        [Test]
        public void ResolveInstance_GivenEditMode_ClearsResolvedInstance()
        {
            // Expect logs
            LogAssert.Expect(LogType.Warning, new Regex("^Saneject: 'TestRuntimeProxy\\.ResolveInstance\\(\\)' called in editor\\. This is not allowed\\.$"));

            // Set up proxy
            TestRuntimeProxy proxy = ScriptableObject.CreateInstance<TestRuntimeProxy>();
            GameObject resolvedInstanceObject = new("Resolved Instance");

            try
            {
                FieldInfo resolvedInstanceField = typeof(RuntimeProxyBase).GetField("resolvedInstance", BindingFlags.NonPublic | BindingFlags.Instance);

                Assert.That(resolvedInstanceField, Is.Not.Null);

                resolvedInstanceField.SetValue(proxy, resolvedInstanceObject);

                // Resolve
                proxy.ResolveInstance();

                // Assert
                Assert.That(resolvedInstanceField.GetValue(proxy), Is.Null);
            }
            finally
            {
                Object.DestroyImmediate(proxy);
                Object.DestroyImmediate(resolvedInstanceObject);
            }
        }
    }
}