using System;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.GameState
{
    /// <summary>
    /// Exposes the game-state events that UI systems react to.
    /// </summary>
    public interface IGameStateObservable
    {
        /// <summary>
        /// Raised once when the round ends because no enemies remain.
        /// </summary>
        event Action OnGameOver;
    }
}
