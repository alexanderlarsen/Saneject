using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.SceneManagement
{
    /// <summary>
    /// Runtime proxy stub for <see cref="SceneManager" />.
    /// Used when <see cref="ISceneManager" /> must cross a Saneject context boundary and resolve
    /// to the live runtime instance during scope startup.
    /// </summary>
    [GenerateRuntimeProxy]
    public partial class SceneManagerProxy : RuntimeProxy<SceneManager>
    {
    }
}
