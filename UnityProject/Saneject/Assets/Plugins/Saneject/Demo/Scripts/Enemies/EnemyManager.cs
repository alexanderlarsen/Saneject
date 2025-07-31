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
    public partial class EnemyManager : MonoBehaviour, IEnemyObservable
    {
        [Inject, SerializeInterface]
        private IScoreUpdater scoreUpdater; 

        [Inject, SerializeInterface]
        private IEnemy[] enemies;

        [ReadOnly, SerializeField]
        private int enemiesLeft;

        public event Action<int> OnEnemiesLeftChanged;

        public int EnemiesLeft => enemiesLeft;
        public int TotalEnemies { get; private set; }

        private void Start()
        {
            InitializeEnemies();
            enemiesLeft = enemies.Length;
            TotalEnemies = enemiesLeft;
        }

        private void OnDestroy()
        {
            foreach (IEnemy enemy in enemies)
                enemy.OnEnemyCaught -= OnEnemyCaught;
        }

        private void OnEnemyCaught()
        {
            enemiesLeft--;
            OnEnemiesLeftChanged?.Invoke(enemiesLeft);
            scoreUpdater.AddPoints(Random.Range(1, 10));

            if (enemiesLeft <= 0)
                Debug.Log("All enemies caught. You win!");
        }

        private void InitializeEnemies()
        {
            foreach (IEnemy enemy in enemies)
            {
                Vector2 randomHorizontal = Random.insideUnitCircle;
                Vector3 spawnPosition = new Vector3(randomHorizontal.x, 0, randomHorizontal.y) * 10;
                enemy.Transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
                enemy.Transform.SetParent(transform);
                enemy.OnEnemyCaught += OnEnemyCaught;
            }
        }
    }
    
    /*
    Roslyn generated partial:

    public partial class EnemyManager : ISerializationCallbackReceiver
    {
        [SerializeField, InterfaceBackingField(interfaceType: typeof(IScoreUpdater), isInjected: true, injectId: null)]
         private Object __scoreUpdater;

        [SerializeField, InterfaceBackingField(interfaceType: typeof(IEnemy), isInjected: true, injectId: null)]
        private Object[] __enemies;

        public void OnBeforeSerialize()
        {
    #if UNITY_EDITOR
            __scoreUpdater = scoreUpdater as UnityEngine.Object;
            __enemies = enemies?.Cast<UnityEngine.Object>().ToArray();
    #endif
        }

        public void OnAfterDeserialize()
        {
            scoreUpdater = __scoreUpdater as Plugins.Saneject.Demo.Scripts.Highscore.IScoreUpdater;
            enemies = (__enemies ?? Array.Empty<UnityEngine.Object>())
                    .Select(x => x as Plugins.Saneject.Demo.Scripts.Enemies.IEnemy)
                    .ToArray();
        }
    }
    */
}