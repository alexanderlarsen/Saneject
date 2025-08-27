using System;
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Highscore;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Enemies
{
    /// <summary>
    /// Manages spawning and tracking of enemies in the scene. Keeps count of total and remaining enemies, and notifies observers on state change.
    /// Implements <see cref="IEnemyObservable" /> to provide enemy counts and change notifications.
    /// Note: Marked as <c>partial</c> since it uses <see cref="SerializeInterfaceAttribute" />.
    /// The Roslyn generator <c>SerializeInterfaceGenerator.dll</c> generates a partial to provide the serialized backing field and assignment logic.
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

    /*
    Roslyn generated partial:

    public partial class EnemyManager : ISerializationCallbackReceiver
    {
        [SerializeField, InterfaceBackingField(typeof(IScoreUpdater), true, null), EditorBrowsable(EditorBrowsableState.Never)]
        private Object __scoreUpdater;

        [SerializeField, InterfaceBackingField(typeof(IEnemy), false, null), EditorBrowsable(EditorBrowsableState.Never)]
        private List<Object> __activeEnemies;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            __scoreUpdater = scoreUpdater as Object;
            __activeEnemies = activeEnemies?.Cast<Object>().ToList();
#endif
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnAfterDeserialize()
        {
            scoreUpdater = __scoreUpdater as IScoreUpdater;
            activeEnemies = (__activeEnemies ?? new List<Object>()).Select(x => x as IEnemy).ToList();
        }
    }
    */
}