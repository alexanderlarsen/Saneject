using Plugins.Saneject.Experimental.Editor.RuntimeProxy;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class ProxyMenuItems
    {
        private const int MenuRoot = -9000;

        [MenuItem("Saneject/Runtime Proxy/Generate Missing Proxy Scripts", false, MenuRoot)]
        private static void GenerateMissingProxyScripts()
        {
            ProxyScriptGenerator.GenerateMissingProxyScripts();
        }
    }
}