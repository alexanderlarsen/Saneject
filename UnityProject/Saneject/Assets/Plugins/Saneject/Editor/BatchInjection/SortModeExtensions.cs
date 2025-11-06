namespace Plugins.Saneject.Editor.BatchInjection
{
    public static class SortModeExtensions
    {
        public static string GetDisplayString(this SortMode mode)
        {
            return mode switch
            {
                SortMode.PathAtoZ => "Path A-Z",
                SortMode.PathZtoA => "Path Z-A",
                SortMode.NameAtoZ => "Name A-Z",
                SortMode.NameZtoA => "Name Z-A",
                SortMode.EnabledToDisabled => "Enabled-Disabled",
                SortMode.DisabledToEnabled => "Disabled-Enabled",
                _ => "Custom"
            };
        }
    }
}