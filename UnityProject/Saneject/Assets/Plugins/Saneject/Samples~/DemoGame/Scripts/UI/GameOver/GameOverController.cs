using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.GameState;
using Plugins.Saneject.Samples.DemoGame.Scripts.Highscore;
using Plugins.Saneject.Samples.DemoGame.Scripts.SceneManagement;
using Plugins.Saneject.Samples.DemoGame.Scripts.UI.MVC;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.UI.GameOver
{
    /// <summary>
    /// Controller for the game-over UI.
    /// Listens to gameplay observables and updates the view when the round ends.
    /// Marked <c>partial</c> so Saneject can serialize the injected interface references.
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
}
