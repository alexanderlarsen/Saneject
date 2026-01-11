using System;
using Plugins.Saneject.Experimental.Runtime.Proxy;

namespace Plugins.Saneject.Experimental.Runtime.Attributes
{
    /// <summary>
    /// Instructs the <c>ProxyObjectGenerator.dll</c> Roslyn generator to implement and forward all interface methods, properties, and events for an <see cref="ProxyObject{TConcrete}" /> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GenerateProxyObjectAttribute : Attribute
    {
    }
}