using Plugins.Saneject.Demo.Scripts.Enemies;
using Plugins.Saneject.Demo.Scripts.GameState;
using Plugins.Saneject.Demo.Scripts.Highscore;
using Plugins.Saneject.Demo.Scripts.SceneManagement;
using Plugins.Saneject.Runtime.Scopes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.Saneject.Demo.Scripts.Scopes
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

        protected override void ConfigureBindings()
        {
            BindComponent<TextMeshProUGUI>()
                .WithId("enemiesTmp")
                .FromTargetDescendants()
                .WhereNameIs("EnemiesTmp");

            BindComponent<TextMeshProUGUI>()
                .WithId("scoreTmp")
                .FromTargetDescendants()
                .WhereNameIs("ScoreTmp");

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