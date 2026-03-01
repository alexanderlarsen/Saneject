using System;
using System.ComponentModel;
using Plugins.Saneject.Experimental.Runtime.Proxy;

namespace Plugins.Saneject.Experimental.Runtime.Attributes
{
    /// <summary>
    /// Instructs the <c>Saneject.RuntimeProxy.Generator.dll</c> Roslyn generator to implement and forward all interface methods, properties, and events for an <see cref="RuntimeProxy{TConcrete}" /> class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), AttributeUsage(AttributeTargets.Class)]
    public sealed class GenerateRuntimeProxyAttribute : Attribute
    {
    }
}