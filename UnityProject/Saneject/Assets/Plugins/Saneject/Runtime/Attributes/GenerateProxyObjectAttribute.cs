using System;

namespace Plugins.Saneject.Runtime.Attributes
{
    /// <summary>
    /// Instructs the <c>ProxyObjectGenerator.dll</c> Roslyn generator to implement and forward all interface methods, properties, and events for an <see cref="Plugins.Saneject.Runtime.Proxy.ProxyObject{TConcrete}" /> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GenerateProxyObjectAttribute : Attribute
    {
    }
}