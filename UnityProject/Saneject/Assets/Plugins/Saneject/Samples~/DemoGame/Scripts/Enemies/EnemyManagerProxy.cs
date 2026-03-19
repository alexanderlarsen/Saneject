using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies
{
    /// <summary>
    /// Runtime proxy stub for <see cref="EnemyManager" />.
    /// Used when <see cref="IEnemyObservable" /> must cross a Saneject context boundary and resolve
    /// to the real manager during runtime startup.
    /// </summary>
    [GenerateRuntimeProxy]
    public partial class EnemyManagerProxy : RuntimeProxy<EnemyManager>
    {
    }
}
