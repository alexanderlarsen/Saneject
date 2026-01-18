using Plugins.Saneject.Experimental.Editor.RuntimeProxy;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class ProxyMenuItems
    {
        [MenuItem("Saneject/RuntimeProxy/Generate Missing Proxy Scripts", false, -10050)]
        private static void GenerateMissingProxyScripts()
        {
            ProxyScriptGenerator.GenerateMissingProxyScripts();
        }
    }
}