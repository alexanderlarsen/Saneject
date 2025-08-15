using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.InterfaceProxy;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Enemies
{
    /// <summary>
    /// Proxy ScriptableObject for <see cref="EnemyManager" />.
    /// Enables cross-scene or cross-prefab serialization of <see cref="IEnemyObservable" /> and <see cref="IEnemyCatchNotifiable" /> references.
    /// The Roslyn generator generates a <c>partial class</c> implementing the interfaces and forwards all calls/events to the resolved <see cref="EnemyManager" /> instance at runtime.
    /// </summary>
    [GenerateInterfaceProxy]
    public partial class EnemyManagerProxy : InterfaceProxyObject<EnemyManager>
    {
    }

    /*
    Roslyn generated partial:

    public partial class EnemyManagerProxy : IEnemyObservable, IEnemyCatchNotifiable
    {
        private readonly System.Collections.Generic.List<(EnemyManager target, System.Action<int> handler)> __OnEnemiesLeftChangedSubscriptions = new();

        public int EnemiesLeft
        {
            get
            {
                if (!instance) { instance = ResolveInstance(); }
                return instance.EnemiesLeft;
            }
        }

        public int TotalEnemies
        {
            get
            {
                if (!instance) { instance = ResolveInstance(); }
                return instance.TotalEnemies;
            }
        }

        public event System.Action<int> OnEnemiesLeftChanged
        {
            add
            {
                if (!instance) { instance = ResolveInstance(); }
                var target = instance;
                target.OnEnemiesLeftChanged += value;
                __OnEnemiesLeftChangedSubscriptions.Add((target, value));
            }
            remove
            {
                var sub = __OnEnemiesLeftChangedSubscriptions.Find(x => x.handler == value);
                if (sub.target != null && !sub.target.Equals(null))
                    sub.target.OnEnemiesLeftChanged -= value;
                __OnEnemiesLeftChangedSubscriptions.RemoveAll(x => x.handler == value);
            }
        }

        public void NotifyEnemyCaught()
        {
            if (!instance) { instance = ResolveInstance(); }
            instance.NotifyEnemyCaught();
        }
    }
    */
}