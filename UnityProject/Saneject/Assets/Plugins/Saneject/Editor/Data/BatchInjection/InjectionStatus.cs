using System.ComponentModel;

namespace Plugins.Saneject.Editor.Data.BatchInjection
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum InjectionStatus
    {
        Unknown,
        Success,
        Warning,
        Error
    }
}