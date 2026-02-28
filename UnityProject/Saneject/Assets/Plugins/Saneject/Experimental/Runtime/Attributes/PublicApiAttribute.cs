using System;

namespace Plugins.Saneject.Experimental.Runtime.Attributes
{
    /// <summary>
    /// Marks public API to be included in generated Saneject API documentation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Field)]
    public sealed class PublicApiAttribute : Attribute
    {
    }
}