using System;
using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Highscore
{
    /// <summary>
    /// Concrete implementation of both <see cref="IScoreObservable" /> and <see cref="IScoreUpdater" />.
    /// Tracks the current score and raises an event when points are added.
    /// </summary>
    public class ScoreManager : MonoBehaviour, IScoreObservable, IScoreUpdater
    {
        public event Action<int> OnPointsChanged;

        public int Points { get; private set; }

        public void AddPoints(int points)
        {
            Points += points;
            OnPointsChanged?.Invoke(Points);
        }
    }
}