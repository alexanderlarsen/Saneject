using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.GameState;
using Plugins.Saneject.Samples.DemoGame.Scripts.Highscore;
using Plugins.Saneject.Samples.DemoGame.Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using EnemyManagerProxy = Plugins.Saneject.Samples.DemoGame.Scripts.Enemies.EnemyManagerProxy;
using GameStateManagerProxy = Plugins.Saneject.Samples.DemoGame.Scripts.GameState.GameStateManagerProxy;
using SceneManagerProxy = Plugins.Saneject.Samples.DemoGame.Scripts.SceneManagement.SceneManagerProxy;
using ScoreManagerProxy = Plugins.Saneject.Samples.DemoGame.Scripts.Highscore.ScoreManagerProxy;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Scopes
{
    /// <summary>
    /// Scene-level DI scope for the UI scene.
    /// </summary>
    public class UISceneScope : Scope
    {
        [SerializeField]
        private GameStateManagerProxy gameStateManagerProxy;

        [SerializeField]
        private ScoreManagerProxy scoreManagerProxy;

        [SerializeField]
        private EnemyManagerProxy enemyManagerProxy;

        [SerializeField]
        private SceneManagerProxy sceneManagerProxy;

        public override void ConfigureBindings()
        {
            BindComponent<Text>()
                .WithId("enemiesLeftText")
                .FromTargetDescendants()
                .WhereNameIs("EnemiesText");

            BindComponent<Text>()
                .WithId("scoreText")
                .FromTargetDescendants()
                .WhereNameIs("ScoreText");

            BindComponent<Button>()
                .WithId("restartButton")
                .FromTargetDescendants()
                .WhereNameIs("RestartButton");

            BindAsset<IGameStateObservable, GameStateManagerProxy>()
                .FromInstance(gameStateManagerProxy);

            BindAsset<IScoreObservable, ScoreManagerProxy>()
                .FromInstance(scoreManagerProxy);

            BindAsset<IEnemyObservable, EnemyManagerProxy>()
                .FromInstance(enemyManagerProxy);

            BindAsset<ISceneManager, SceneManagerProxy>()
                .FromInstance(sceneManagerProxy);
        }
    }
}