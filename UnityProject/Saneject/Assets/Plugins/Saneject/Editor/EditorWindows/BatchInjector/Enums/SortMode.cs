using System.ComponentModel;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector.Enums
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum SortMode
    {
        NameAtoZ,
        NameZtoA,
        PathAtoZ,
        PathZtoA,
        EnabledToDisabled,
        DisabledToEnabled,
        StatusSuccessToError,
        StatusErrorToSuccess,
        Custom
    }
}