using System;

namespace Plugins.Saneject.Demo.Scripts.Enemies
{
    /// <summary>
    /// Interface for objects that can be observed for enemy count changes (e.g., for UI updates).
    /// </summary>
    public interface IEnemyObservable
    {
        event Action<int> OnEnemiesLeftChanged;
        int EnemiesLeft { get; }
        int TotalEnemies { get; }
    }
}