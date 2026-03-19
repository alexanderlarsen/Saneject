using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Highscore
{
    /// <summary>
    /// Runtime proxy stub for <see cref="ScoreManager" />.
    /// Used when score interfaces must cross a Saneject context boundary and resolve to the
    /// real scene instance during runtime startup.
    /// </summary>
    [GenerateRuntimeProxy]
    public partial class ScoreManagerProxy : RuntimeProxy<ScoreManager>
    {
    }
}
