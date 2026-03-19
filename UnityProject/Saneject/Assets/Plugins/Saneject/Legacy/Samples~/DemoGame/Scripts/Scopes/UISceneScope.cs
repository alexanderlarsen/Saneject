using Plugins.Saneject.Legacy.Runtime.Scopes;
using Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.GameState;
using Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Highscore;
using Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.SceneManagement;
using Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.UI.GameOver;
using UnityEngine.UI;
using EnemyManager = Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Enemies.EnemyManager;
using GameStateManager = Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.GameState.GameStateManager;

namespace Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Scopes
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
                .WhereGameObject(go => go.name == "EnemiesText");

            BindComponent<Text>()
                .ToID("scoreText")
                .FromTargetDescendants()
                .WhereGameObject(go => go.name == "ScoreText");

            BindComponent<Button>()
                .ToTarget<GameOverView>()
                .ToMember("restartButton")
                .FromTargetDescendants()
                .WhereGameObject(go => go.name == "RestartButton");

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