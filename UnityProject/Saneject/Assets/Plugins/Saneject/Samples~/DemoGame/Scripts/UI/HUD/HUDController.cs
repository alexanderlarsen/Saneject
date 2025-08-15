using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.GameState;
using Plugins.Saneject.Samples.DemoGame.Scripts.Highscore;
using Plugins.Saneject.Samples.DemoGame.Scripts.UI.MVC;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.UI.HUD
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
        [SerializeField, InterfaceBackingField(interfaceType: typeof(IGameStateObservable), isInjected: true, injectId: null)]
        private Object __gameStateObservable;

        [SerializeField, InterfaceBackingField(interfaceType: typeof(IScoreObservable), isInjected: true, injectId: null)]
        private Object __scoreObservable;

        [SerializeField, InterfaceBackingField(interfaceType: typeof(IEnemyObservable), isInjected: true, injectId: null)]
        private Object __enemyObservable;

        public void OnBeforeSerialize()
        {
    #if UNITY_EDITOR
            __gameStateObservable = gameStateObservable as Object;
            __scoreObservable = scoreObservable as Object;
            __enemyObservable = enemyObservable as Object;
    #endif
        }

        public void OnAfterDeserialize()
        {
            gameStateObservable = __gameStateObservable as IGameStateObservable;
            scoreObservable = __scoreObservable as IScoreObservable;
            enemyObservable = __enemyObservable as IEnemyObservable;
        }
    }
    */
}