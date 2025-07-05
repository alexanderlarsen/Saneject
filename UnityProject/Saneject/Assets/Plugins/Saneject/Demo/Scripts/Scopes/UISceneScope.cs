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
            RegisterObject<IGameStateObservable, GameStateManagerProxy>().FromInstance(gameStateManagerProxy);
            RegisterObject<IScoreObservable, ScoreManagerProxy>().FromInstance(scoreManagerProxy);
            RegisterObject<IEnemyObservable, EnemyManagerProxy>().FromInstance(enemyManagerProxy);
            RegisterObject<ISceneManager, SceneManagerProxy>().FromInstance(sceneManagerProxy);

            RegisterComponent<TextMeshProUGUI>().WithId("enemiesTmp").FromTargetDescendants().WhereNameIs("EnemiesTmp");
            RegisterComponent<TextMeshProUGUI>().WithId("scoreTmp").FromTargetDescendants().WhereNameIs("ScoreTmp");
            RegisterComponent<Button>().WithId("restartButton").FromTargetDescendants().WhereNameIs("RestartButton");
        }
    }
}