using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Camera;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.PlayerSystems
{
    /// <summary>
    /// Runtime proxy stub for <see cref="Player" />.
    /// Used when <see cref="ICameraFollowTarget" /> or <see cref="IEnemyEvadeTarget" /> must cross
    /// a Saneject context boundary and resolve to the live player instance at runtime.
    /// </summary>
    [GenerateRuntimeProxy]
    public partial class PlayerProxy : RuntimeProxy<Player>
    {
    }
}
