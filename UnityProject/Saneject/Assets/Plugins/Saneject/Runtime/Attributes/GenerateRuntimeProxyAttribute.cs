using System;
using System.ComponentModel;
using Plugins.Saneject.Runtime.Proxy;

namespace Plugins.Saneject.Runtime.Attributes
{
    /// <summary>
    /// Marks a <see cref="RuntimeProxy{TComponent}"/> stub for Roslyn-generated interface implementations.
    /// </summary>
    /// <remarks>
    /// Saneject typically places this attribute on generated runtime proxy script stubs. The Roslyn generator then emits stub members for the
    /// target component's public non-generic interfaces. Those generated members throw until the proxy is swapped to a real instance during scope startup.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never), AttributeUsage(AttributeTargets.Class)]
    public sealed class GenerateRuntimeProxyAttribute : Attribute
    {
    }
}
