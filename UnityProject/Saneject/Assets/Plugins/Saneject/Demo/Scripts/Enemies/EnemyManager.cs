using System;
using Plugins.Saneject.Demo.Scripts.Highscore;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Plugins.Saneject.Demo.Scripts.Enemies
{
    /// <summary>
    /// Manages spawning and tracking of enemies in the scene. Keeps count of total and remaining enemies, and notifies observers on state change.
    /// Implements <see cref="IEnemyObservable" /> to provide enemy counts and change notifications.
    /// Note: Marked as <c>partial</c> since it uses <see cref="SerializeInterfaceAttribute" />.
    /// The Roslyn generator <c>SerializeInterfaceGenerator.dll</c> generates a partial to provide the serialized backing field and assignment logic.
    /// </summary>
    public partial class EnemyManager : MonoBehaviour, IEnemyObservable, IEnemyCatchNotifiable
    {
        [Inject, SerializeInterface]
        private IScoreUpdater scoreUpdater;

        [Inject("EnemyPrefab"), SerializeField]
        private GameObject enemyPrefab;

        [ReadOnly, SerializeField]
        private int enemiesLeft;

        [SerializeField]
        private int numEnemies = 6;

        public event Action<int> OnEnemiesLeftChanged;

        public int EnemiesLeft => enemiesLeft;
        public int TotalEnemies { get; private set; }

        private void Start()
        {
            SpawnEnemies();
        }

        public void NotifyEnemyCaught()
        {
            enemiesLeft--;
            OnEnemiesLeftChanged?.Invoke(enemiesLeft);
            scoreUpdater.AddPoints(Random.Range(1, 10));

            if (enemiesLeft <= 0)
                Debug.Log("All enemies caught. You win!");
        }

        private void SpawnEnemies()
        {
            for (int i = 0; i < numEnemies; i++)
            {
                Vector2 randomHorizontal = Random.insideUnitCircle;
                Vector3 spawnPosition = new Vector3(randomHorizontal.x, 0, randomHorizontal.y) * 10;
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
            }

            enemiesLeft = transform.childCount;
            TotalEnemies = enemiesLeft;
        }

        /*
        Roslyn generated partial:

        public partial class EnemyManager : UnityEngine.ISerializationCallbackReceiver
        {
            [UnityEngine.SerializeField, Saneject.Runtime.Attributes.InterfaceBackingField(interfaceType: typeof(IScoreUpdater), isInjected: true, injectId: null)]
            private UnityEngine.Object __scoreUpdater;

            public void OnBeforeSerialize() { }

            public void OnAfterDeserialize()
            {
                scoreUpdater = __scoreUpdater as IScoreUpdater;
            }
        }
        */
    }
}