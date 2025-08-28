using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.GameState;
using Plugins.Saneject.Samples.DemoGame.Scripts.Highscore;
using Plugins.Saneject.Samples.DemoGame.Scripts.SceneManagement;
using UnityEngine.UI;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Scopes
{
    /// <summary>
    /// Scene-level DI scope for the UI scene.
    /// </summary>
    public class UISceneScope : Scope
    {
        public override void ConfigureBindings()
        {
            BindComponent<Text>()
                .ToID("enemiesLeftText")
                .FromTargetDescendants()
                .WhereNameIs("EnemiesText");

            BindComponent<Text>()
                .ToID("scoreText")
                .FromTargetDescendants()
                .WhereNameIs("ScoreText");

            BindComponent<Button>()
                .ToID("restartButton")
                .FromTargetDescendants()
                .WhereNameIs("RestartButton");

            BindComponent<IGameStateObservable, GameStateManager>()
                .FromProxy();

            BindComponent<IScoreObservable, ScoreManager>()
                .FromProxy();

            BindComponent<IEnemyObservable, EnemyManager>()
                .FromProxy();

            BindComponent<ISceneManager, SceneManager>()
                .FromProxy();
        }
    }
}