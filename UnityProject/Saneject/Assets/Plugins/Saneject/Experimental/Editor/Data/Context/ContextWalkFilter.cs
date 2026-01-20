namespace Plugins.Saneject.Experimental.Editor.Data.Context
{
    /// <summary>
    /// Filter options for pre-filtering transforms in the injection graph based on their context, before injection starts.
    /// </summary>
    public enum ContextWalkFilter
    {
        /// <summary>
        /// Includes all transforms in the injection graph.
        /// Context isolation rules are unaffected by this setting.
        /// </summary>
        All,

        /// <summary>
        /// Includes transforms in the injection graph that belong to the same context(s) as the selected start objects.
        /// Context isolation rules are unaffected by this setting.
        /// </summary>
        SameAsStartObjects,

        /// <summary>
        /// Includes transforms in the injection graph that belong to a scene object.
        /// Context isolation rules are unaffected by this setting.
        /// </summary>
        SceneObject,

        /// <summary>
        /// Includes transforms in the injection graph that belong to a prefab instance.
        /// Context isolation rules are unaffected by this setting.
        /// </summary>
        PrefabInstance,

        /// <summary>
        /// Includes transforms in the injection graph that belong to a prefab asset (not instance).
        /// Context isolation rules are unaffected by this setting.
        /// </summary>
        PrefabAsset
    }
}