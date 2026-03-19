using System;
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Highscore;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Enemies
{
    /// <summary>
    /// Scene-level gameplay service that spawns enemies, tracks the active set, and awards score
    /// when enemies are caught.
    /// Marked <c>partial</c> so Saneject can serialize the injected score updater and enemy list interfaces.
    /// </summary>
    public partial class EnemyManager : MonoBehaviour, IEnemyObservable
    {
        [Inject, SerializeInterface]
        private IScoreUpdater scoreUpdater;

        [Inject, SerializeField]
        private GameObject enemyPrefab;

        [ReadOnly, SerializeInterface]
        private List<IEnemy> activeEnemies;

        [SerializeField]
        private int numStartingEnemies = 10;

        public event Action<int> OnEnemiesLeftChanged;

        public int EnemiesLeft => activeEnemies.Count;
        public int TotalEnemies { get; private set; }

        private void Start()
        {
            InitializeEnemies();
        }

        private void OnDestroy()
        {
            foreach (IEnemy enemy in activeEnemies)
                enemy.OnEnemyCaught -= OnEnemyCaught;
        }

        private void OnEnemyCaught(IEnemy enemy)
        {
            activeEnemies.Remove(enemy);
            OnEnemiesLeftChanged?.Invoke(EnemiesLeft);
            scoreUpdater.AddPoints(Random.Range(1, 10));

            if (EnemiesLeft <= 0)
                Debug.Log("All enemies caught. You win!");
        }

        private void InitializeEnemies()
        {
            for (int i = 0; i < numStartingEnemies; i++)
            {
                IEnemy enemy = Instantiate(enemyPrefab, transform).GetComponent<IEnemy>();
                Vector2 randomHorizontal = Random.insideUnitCircle;
                Vector3 spawnPosition = new Vector3(randomHorizontal.x, 0, randomHorizontal.y) * 10;
                enemy.Transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
                enemy.Transform.SetParent(transform);
                enemy.OnEnemyCaught += OnEnemyCaught;
                activeEnemies.Add(enemy);
            }

            TotalEnemies = activeEnemies.Count;
        }
    }
}