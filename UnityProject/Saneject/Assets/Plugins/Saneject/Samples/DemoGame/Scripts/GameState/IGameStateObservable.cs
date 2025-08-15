using System;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.GameState
{
    /// <summary>
    /// Exposes game state change events.
    /// </summary>
    public interface IGameStateObservable
    {
        /// <summary>
        /// Invoked once when the game is over (e.g., no enemies remain).
        /// </summary>
        event Action OnGameOver;
    }
}