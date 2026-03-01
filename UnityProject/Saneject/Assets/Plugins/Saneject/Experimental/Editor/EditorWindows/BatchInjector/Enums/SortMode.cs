using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums
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