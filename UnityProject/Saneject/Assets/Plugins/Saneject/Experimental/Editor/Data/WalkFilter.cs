namespace Plugins.Saneject.Experimental.Editor.Data
{
    public enum WalkFilter
    {
        /// <summary>
        /// Includes the roots of the selected objects, then injects all transforms
        /// below those roots without context filtering.
        /// Context isolation rules are unaffected by this setting.
        /// </summary>
        All,

        /// <summary>
        /// Includes the roots of the selected objects, then injects all transforms
        /// below those roots that belong to the same context as the selection.
        /// Context isolation rules are unaffected by this setting.
        /// </summary>
        StartObjectsContext
    }
}