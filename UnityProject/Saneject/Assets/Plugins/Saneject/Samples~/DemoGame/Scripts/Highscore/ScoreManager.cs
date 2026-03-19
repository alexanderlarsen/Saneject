using System;
using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Highscore
{
    /// <summary>
    /// Scene-level score service that exposes read and write interfaces for the current score.
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
