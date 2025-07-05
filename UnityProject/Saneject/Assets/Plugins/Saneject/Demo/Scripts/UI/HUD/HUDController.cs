using Plugins.Saneject.Demo.Scripts.Enemies;
using Plugins.Saneject.Demo.Scripts.GameState;
using Plugins.Saneject.Demo.Scripts.Highscore;
using Plugins.Saneject.Demo.Scripts.UI.MVC;
using Plugins.Saneject.Runtime.Attributes;

namespace Plugins.Saneject.Demo.Scripts.UI.HUD
{
    /// <summary>
    /// UI controller for the in-game HUD.
    /// Subscribes to game state, score, and enemy events to update the view.
    /// Note: This class is marked as <c>partial</c> because it uses <see cref="SerializeInterfaceAttribute" />.
    /// The Roslyn generator <c>SerializeInterfaceGenerator.dll</c> generates a matching partial that adds serialized backing fields and assigns them to the interface references.
    /// </summary>
    public partial class HUDController : ControllerBase<HUDView>
    {
        [Inject, SerializeInterface]
        private IGameStateObservable gameStateObservable;

        [Inject, SerializeInterface]
        private IScoreObservable scoreObservable;

        [Inject, SerializeInterface]
        private IEnemyObservable enemyObservable;

        private void Awake()
        {
            scoreObservable.OnPointsChanged += view.UpdateScore;
            enemyObservable.OnEnemiesLeftChanged += view.UpdateEnemiesLeft;
            gameStateObservable.OnGameOver += view.Hide;
        }

        private void Start()
        {
            view.SetTotalEnemies(enemyObservable.TotalEnemies);
            view.UpdateEnemiesLeft(enemyObservable.EnemiesLeft);
            view.UpdateScore(scoreObservable.Points);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            scoreObservable.OnPointsChanged -= view.UpdateScore;
            enemyObservable.OnEnemiesLeftChanged -= view.UpdateEnemiesLeft;
            gameStateObservable.OnGameOver -= view.Hide;
        }
    }

    /*
    Roslyn generated partial:

    public partial class HUDController : ISerializationCallbackReceiver
    {
        [UnityEngine.SerializeField, Saneject.Runtime.Attributes.InterfaceBackingField(interfaceType: typeof(IGameStateObservable), isInjected: true, injectId: null)]
        private UnityEngine.Object __gameStateObservable;

        [UnityEngine.SerializeField, Saneject.Runtime.Attributes.InterfaceBackingField(interfaceType: typeof(IScoreObservable), isInjected: true, injectId: null)]
        private UnityEngine.Object __scoreObservable;

        [UnityEngine.SerializeField, Saneject.Runtime.Attributes.InterfaceBackingField(interfaceType: typeof(IEnemyObservable), isInjected: true, injectId: null)]
        private UnityEngine.Object __enemyObservable;

        public void OnBeforeSerialize() {}

        public void OnAfterDeserialize()
        {
            gameStateObservable = __gameStateObservable as IGameStateObservable;
            scoreObservable = __scoreObservable as IScoreObservable;
            enemyObservable = __enemyObservable as IEnemyObservable;
        }
    }
    */
}