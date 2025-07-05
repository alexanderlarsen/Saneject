using System;

namespace Plugins.Saneject.Demo.Scripts.Highscore
{
    /// <summary>
    /// Exposes read-only score data and a change event.
    /// </summary>
    public interface IScoreObservable
    {
        /// <summary>
        /// Invoked when score points change.
        /// </summary>
        event Action<int> OnPointsChanged;

        /// <summary>
        /// Current score points.
        /// </summary>
        int Points { get; }
    }
}