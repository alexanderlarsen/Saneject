using System.ComponentModel;

namespace Plugins.Saneject.Editor.Data.Context
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum ContextType
    {
        SceneObject,
        PrefabAsset,
        PrefabInstance
    }
}