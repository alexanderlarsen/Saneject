using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Highscore
{
    /// <summary>
    /// Proxy ScriptableObject for <see cref="ScoreManager" />.
    /// Enables cross-scene or cross-prefab serialization of <see cref="IScoreObservable" /> and <see cref="IScoreUpdater" /> references.
    /// The Roslyn generator generates a <c>partial class</c> implementing the interfaces and forwards all calls/events to the resolved <see cref="ScoreManager" /> instance at runtime.
    /// </summary>
    [GenerateProxyObject]
    public partial class ScoreManagerProxy : ProxyObject<ScoreManager>
    {
    }

    /*
    Roslyn generated partial:

    public partial class ScoreManagerProxy : IScoreObservable, IScoreUpdater
    {
        public int Points
        {
            get
            {
                if (!instance)
                    instance = ResolveInstance();

                return instance.Points;
            }
        }

        private readonly System.Collections.Generic.List<(ScoreManager target, System.Action<int> handler)> __OnPointsChangedSubscriptions = new();

        public event System.Action<int> OnPointsChanged
        {
            add
            {
                if (!instance) { instance = ResolveInstance(); }
                var target = instance;
                target.OnPointsChanged += value;
                __OnPointsChangedSubscriptions.Add((target, value));
            }
            remove
            {
                var sub = __OnPointsChangedSubscriptions.Find(x => x.handler == value);
                if (sub.target != null && !sub.target.Equals(null))
                    sub.target.OnPointsChanged -= value;
                __OnPointsChangedSubscriptions.RemoveAll(x => x.handler == value);
            }
        }

        public void AddPoints(int points)
        {
            if (!instance)
                instance = ResolveInstance();

            instance.AddPoints(points);
        }
    }
    */
}