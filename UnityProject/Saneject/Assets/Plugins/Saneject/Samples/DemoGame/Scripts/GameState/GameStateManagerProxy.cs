using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.InterfaceProxy;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.GameState
{
    /// <summary>
    /// Proxy ScriptableObject for <see cref="GameStateManager" />.
    /// Enables cross-scene or cross-prefab serialization of <see cref="IGameStateObservable" /> references.
    /// The Roslyn generator generates a <c>partial class</c> implementing the interfaces and forwards all calls/events to the resolved <see cref="GameStateManager" /> instance at runtime.
    /// </summary>
    [GenerateInterfaceProxy]
    public partial class GameStateManagerProxy : InterfaceProxyObject<GameStateManager>
    {
    }

    /*
    Roslyn generated partial:

    public partial class GameStateManagerProxy : IGameStateObservable
    {
        private readonly System.Collections.Generic.List<(GameStateManager target, System.Action handler)> __OnGameOverSubscriptions = new();

        public event System.Action OnGameOver
        {
            add
            {
                if (!instance) { instance = ResolveInstance(); }
                var target = instance;
                target.OnGameOver += value;
                __OnGameOverSubscriptions.Add((target, value));
            }
            remove
            {
                var sub = __OnGameOverSubscriptions.Find(x => x.handler == value);
                if (sub.target != null && !sub.target.Equals(null))
                    sub.target.OnGameOver -= value;
                __OnGameOverSubscriptions.RemoveAll(x => x.handler == value);
            }
        }
    }
    */
}