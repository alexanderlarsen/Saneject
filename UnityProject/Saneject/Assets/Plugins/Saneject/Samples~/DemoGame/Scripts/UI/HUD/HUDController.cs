using Plugins.Saneject.Runtime.Attributes;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.GameState;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Highscore;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.UI.MVC;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.UI.HUD
{
    /// <summary>
    /// Controller for the in-game HUD.
    /// Subscribes to gameplay observables and pushes the latest state into the view.
    /// Marked <c>partial</c> so Saneject can serialize the injected interface references.
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
}
