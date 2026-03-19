using Plugins.Saneject.Legacy.Runtime.Attributes;
using Plugins.Saneject.Legacy.Runtime.Proxy;
using Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Camera;
using Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Enemies;

namespace Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.PlayerSystems
{
    /// <summary>
    /// Proxy ScriptableObject for <see cref="Player" />.
    /// Enables cross-scene or cross-prefab serialization of <see cref="Player" /> and <see cref="ICameraFollowTarget" /> references.
    /// The Roslyn generator generates a <c>partial class</c> implementing the interfaces and forwards all calls/events to the resolved <see cref="IEnemyEvadeTarget" /> instance at runtime.
    /// </summary>
    [GenerateProxyObject]
    public partial class PlayerProxy : ProxyObject<Player>
    {
    }

    /*
    Roslyn generated partial:

    public partial class PlayerProxy : ICameraFollowTarget, IEnemyEvadeTarget
    {
        public UnityEngine.Vector3 Position
        {
            get
            {
                if (!instance) { instance = ResolveInstance(); }
                return instance.Position;
            }
        }
    }
    */
}