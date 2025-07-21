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

        public override void Configure()
        {
            Bind<IGameStateObservable, GameStateManagerProxy>().FromInstance(gameStateManagerProxy);
            Bind<IScoreObservable, ScoreManagerProxy>().FromInstance(scoreManagerProxy);
            Bind<IEnemyObservable, EnemyManagerProxy>().FromInstance(enemyManagerProxy);
            Bind<ISceneManager, SceneManagerProxy>().FromInstance(sceneManagerProxy);

            Bind<TextMeshProUGUI>().WithId("enemiesTmp").FromTargetDescendants().WhereNameIs("EnemiesTmp");
            Bind<TextMeshProUGUI>().WithId("scoreTmp").FromTargetDescendants().WhereNameIs("ScoreTmp");
            Bind<Button>().WithId("restartButton").FromTargetDescendants().WhereNameIs("RestartButton");
        }
    }
}