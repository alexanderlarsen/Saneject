using System;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies;
using UnityEngine;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.GameState
{
    /// <summary>
    /// Monitors the enemy count and raises the game-over signal once all enemies have been caught.
    /// Marked <c>partial</c> so Saneject can serialize the injected <see cref="IEnemyObservable" /> reference.
    /// </summary>
    public partial class GameStateManager : MonoBehaviour, IGameStateObservable
    {
        [Inject, SerializeInterface]
        private IEnemyObservable enemyObservable;

        private bool isGameOver;

        public event Action OnGameOver;

        private void Awake()
        {
            enemyObservable.OnEnemiesLeftChanged += OnEnemiesLeftChanged;
        }

        private void OnEnemiesLeftChanged(int enemiesLeft)
        {
            if (isGameOver || enemiesLeft > 0)
                return;

            isGameOver = true;
            OnGameOver?.Invoke();
        }
    }
}
