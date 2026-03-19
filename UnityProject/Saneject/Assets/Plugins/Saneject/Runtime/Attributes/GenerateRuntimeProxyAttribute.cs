using System;
using System.ComponentModel;
using Plugins.Saneject.Runtime.Proxy;

namespace Plugins.Saneject.Runtime.Attributes
{
    /// <summary>
    /// Instructs the Roslyn code generator to generate stub implementations of all interface members for a <see cref="RuntimeProxy{TComponent}"/> class.
    /// </summary>
    /// <remarks>
    /// This attribute is automatically applied by Saneject during code generation and is intended for internal use.
    /// The generator creates stub implementations (that throw exceptions until swapped) for all methods, properties, and events
    /// declared by the target type's interfaces. These stubs allow the proxy to act as a serializable placeholder at editor-time.
    /// At runtime, the Scope system replaces proxies with real instances via <see cref="IRuntimeProxySwapTarget"/>.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never), AttributeUsage(AttributeTargets.Class)]
    public sealed class GenerateRuntimeProxyAttribute : Attribute
    {
    }
}