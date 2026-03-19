using System;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies
{
    /// <summary>
    /// Exposes enemy-count state for systems that react to round progress.
    /// </summary>
    public interface IEnemyObservable
    {
        /// <summary>
        /// Raised whenever the number of active enemies changes.
        /// </summary>
        event Action<int> OnEnemiesLeftChanged;

        /// <summary>
        /// Gets the number of enemies that are still active in the round.
        /// </summary>
        int EnemiesLeft { get; }

        /// <summary>
        /// Gets the number of enemies that were spawned at the start of the round.
        /// </summary>
        int TotalEnemies { get; }
    }
}
