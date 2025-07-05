using Plugins.Saneject.Demo.Scripts.Enemies;
using Plugins.Saneject.Demo.Scripts.GameState;
using Plugins.Saneject.Demo.Scripts.Highscore;
using Plugins.Saneject.Demo.Scripts.SceneManagement;
using Plugins.Saneject.Demo.Scripts.UI.MVC;
using Plugins.Saneject.Runtime.Attributes;

namespace Plugins.Saneject.Demo.Scripts.UI.GameOver
{
    /// <summary>
    /// Controller for the Game Over screen.
    /// Subscribes to game state events and updates the view accordingly.
    /// Note: This class is marked as <c>partial</c> because it uses <see cref="SerializeInterfaceAttribute" />.
    /// The Roslyn generator <c>SerializeInterfaceGenerator.dll</c> generates a matching partial that adds serialized backing fields and assigns them to the interface references.
    /// </summary>
    public partial class GameOverController : ControllerBase<GameOverView>
    {
        [Inject, SerializeInterface]
        private IGameStateObservable gameStateObservable;

        [Inject, SerializeInterface]
        private IScoreObservable scoreObservable;

        [Inject, SerializeInterface]
        private IEnemyObservable enemyObservable;

        [Inject, SerializeInterface]
        private ISceneManager sceneManager;

        private void Awake()
        {
            view.OnRestartButtonClick(RestartGame);
            gameStateObservable.OnGameOver += OnGameOver;
        }

        private void Start()
        {
            view.Hide();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            gameStateObservable.OnGameOver -= OnGameOver;
        }

        private void OnGameOver()
        {
            view.UpdateScore(scoreObservable.Points);
            view.UpdateEnemiesCaught(enemyObservable.TotalEnemies);
            view.Show();
        }

        private void RestartGame()
        {
            sceneManager.RestartGame();
        }
    }

    /*
    Roslyn generated partial:

    public partial class GameOverController : ISerializationCallbackReceiver
    {
        [UnityEngine.SerializeField, Saneject.Runtime.Attributes.InterfaceBackingField(interfaceType: typeof(IGameStateObservable), isInjected: true, injectId: null)]
        private UnityEngine.Object __gameStateObservable;

        [UnityEngine.SerializeField, Saneject.Runtime.Attributes.InterfaceBackingField(interfaceType: typeof(IScoreObservable), isInjected: true, injectId: null)]
        private UnityEngine.Object __scoreObservable;

        [UnityEngine.SerializeField, Saneject.Runtime.Attributes.InterfaceBackingField(interfaceType: typeof(IEnemyObservable), isInjected: true, injectId: null)]
        private UnityEngine.Object __enemyObservable;

        [UnityEngine.SerializeField, Saneject.Runtime.Attributes.InterfaceBackingField(interfaceType: typeof(ISceneManager), isInjected: true, injectId: null)]
        private UnityEngine.Object __sceneManager;

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            gameStateObservable = __gameStateObservable as IGameStateObservable;
            scoreObservable = __scoreObservable as IScoreObservable;
            enemyObservable = __enemyObservable as IEnemyObservable;
            sceneManager = __sceneManager as ISceneManager;
        }
    }
    */
}