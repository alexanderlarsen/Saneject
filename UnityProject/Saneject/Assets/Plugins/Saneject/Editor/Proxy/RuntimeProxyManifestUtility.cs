using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Runtime.Scopes;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Editor.Proxy
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RuntimeProxyManifestUtility
    {
        public static IEnumerable<Type> EnumerateManifestTypes()
        {
            const string manifestFullName = "Saneject.RuntimeProxy.Generator.AssemblyProxyManifest";
            string scopeAssemblyName = typeof(Scope).Assembly.GetName().Name;
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            // Assemblies that reference the Scope's assembly (Saneject.Runtime)
            IEnumerable<Assembly> assemblies = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(asm => asm
                    .GetReferencedAssemblies()
                    .Any(r => r.Name == scopeAssemblyName)
                );

            foreach (Assembly assembly in assemblies)
            {
                FieldInfo proxyTargetsField = assembly
                    .GetType(manifestFullName, throwOnError: false)?
                    .GetField("RequiredProxyTargets", bindingFlags);

                if (proxyTargetsField?.GetValue(null) is Type[] targets)
                    foreach (Type t in targets)
                        yield return t;
            }
        }
    }
}