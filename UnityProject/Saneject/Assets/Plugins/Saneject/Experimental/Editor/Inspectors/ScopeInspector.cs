using Plugins.Saneject.Experimental.Runtime.Scopes;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.Inspectors
{
    [CustomEditor(typeof(Scope), true), CanEditMultipleObjects]
    public class ScopeInspector : UnityEditor.Editor
    {
    }
}