using System;
using Plugins.Saneject.Demo.Scripts.Enemies;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.Saneject.Demo.Scripts.GameState
{
    /// <summary>
    /// Listens for enemy count changes and broadcasts game over state when all enemies are defeated.
    /// Implements <see cref="IGameStateObservable" />.
    /// Note: This class is marked as <c>partial</c> because it uses <see cref="SerializeInterfaceAttribute" />.
    /// The Roslyn generator <c>SerializeInterfaceGenerator.dll</c> generates a backing field and assigns it to the interface field on deserialize.
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

    /*
    Roslyn generated partial:

    public partial class GameStateManager : UnityEngine.ISerializationCallbackReceiver
    {
       [UnityEngine.SerializeField, Saneject.Runtime.Attributes.InterfaceBackingField(interfaceType: typeof(IEnemyObservable), isInjected: true, injectId: null)]
       private UnityEngine.Object __enemyObservable;

       public void OnBeforeSerialize() { }

       public void OnAfterDeserialize()
       {
           enemyObservable = __enemyObservable as IEnemyObservable;
       }
    }
    */
}