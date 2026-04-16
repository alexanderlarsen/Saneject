using System;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Highscore
{
    /// <summary>
    /// Exposes read-only score state for systems that display points.
    /// </summary>
    public interface IScoreObservable
    {
        /// <summary>
        /// Raised whenever the current score changes.
        /// </summary>
        event Action<int> OnPointsChanged;

        /// <summary>
        /// Gets the current score.
        /// </summary>
        int Points { get; }
    }
}
