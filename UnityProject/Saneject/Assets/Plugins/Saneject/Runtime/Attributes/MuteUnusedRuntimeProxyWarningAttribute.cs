using System;
using System.ComponentModel;

namespace Plugins.Saneject.Runtime.Attributes
{
    [EditorBrowsable(EditorBrowsableState.Never), AttributeUsage(AttributeTargets.Class)]
    public class MuteUnusedRuntimeProxyWarningAttribute : Attribute
    {
    }
}