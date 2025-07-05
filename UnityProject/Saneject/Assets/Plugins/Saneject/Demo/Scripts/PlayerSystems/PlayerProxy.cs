using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.InterfaceProxy;

namespace Plugins.Saneject.Demo.Scripts.PlayerSystems
{
    /// <summary>
    /// Proxy ScriptableObject for <see cref="Player" />.
    /// Enables cross-scene or cross-prefab serialization of <see cref="Plugins.Saneject.Demo.Scripts.Camera.ICameraFollowTarget" /> and <see cref="Plugins.Saneject.Demo.Scripts.Enemies.IEnemyEvadeTarget" /> references.
    /// The Roslyn generator generates a <c>partial class</c> implementing the interfaces and forwards all calls/events to the resolved <see cref="Player" /> instance at runtime.
    /// </summary>
    [GenerateInterfaceProxy]
    public partial class PlayerProxy : InterfaceProxyObject<Player>
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