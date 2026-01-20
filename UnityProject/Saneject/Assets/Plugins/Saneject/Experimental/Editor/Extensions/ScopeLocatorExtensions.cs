using Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Experimental.Runtime.Settings;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class ScopeLocatorExtensions
    {
        public static ScopeNode FindNearestScope(this TransformNode start)
        {
            TransformNode current = start;

            while (current != null)
            {
                ScopeNode scope = current.DeclaredScopeNode;

                if (scope != null &&
                    (!UserSettings.UseContextIsolation ||
                     current.ContextIdentity.Equals(start.ContextIdentity)))
                    return scope;

                current = current.ParentTransformNode;
            }

            return null;
        }
    }
}