using System;

namespace Plugins.Saneject.Runtime.Attributes
{
    /// <summary>
    /// Instructs the <c>InterfaceProxyGenerator.dll</c> Roslyn generator to implement and forward all interface methods, properties, and events for an <see cref="Plugins.Saneject.Runtime.InterfaceProxy.InterfaceProxyObject{T}"/> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GenerateInterfaceProxyAttribute : Attribute
    {
    }
}