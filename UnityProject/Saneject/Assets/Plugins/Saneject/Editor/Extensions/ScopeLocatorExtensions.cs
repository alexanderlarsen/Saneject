using System.ComponentModel;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Runtime.Settings;

namespace Plugins.Saneject.Editor.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ScopeLocatorExtensions
    {
        public static ScopeNode FindNearestScope(this TransformNode start)
        {
            TransformNode current = start;

            while (current != null)
            {
                ScopeNode scope = current.DeclaredScopeNode;

                if (scope != null &&
                    (!ProjectSettings.UseContextIsolation ||
                     current.ContextIdentity.Equals(start.ContextIdentity)))
                    return scope;

                current = current.ParentTransformNode;
            }

            return null;
        }
    }
}