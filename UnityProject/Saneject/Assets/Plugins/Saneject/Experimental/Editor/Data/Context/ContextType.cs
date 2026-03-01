using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Editor.Data.Context
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum ContextType
    {
        Global,
        PrefabAsset,
        PrefabInstance,
        SceneObject
    }
}