using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Runtime.Settings;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class TransformNodeExtensions
    {
        public static ScopeNode FindParentScopeNode(this TransformNode transformNode)
        {
            TransformNode currentTransformNode = transformNode.ParentTransformNode;
            ScopeNode parentScope = null;

            while (currentTransformNode != null)
            {
                if (currentTransformNode.ScopeNode == null || (UserSettings.UseContextIsolation && currentTransformNode.ContextNode != transformNode.ContextNode))
                {
                    currentTransformNode = currentTransformNode.ParentTransformNode;
                    continue;
                }

                parentScope = currentTransformNode.ScopeNode;
                break;
            }

            return parentScope;
        }
    }
}