using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.GameState;
using Plugins.Saneject.Samples.DemoGame.Scripts.Highscore;
using Plugins.Saneject.Samples.DemoGame.Scripts.SceneManagement;
using Plugins.Saneject.Samples.DemoGame.Scripts.UI.GameOver;
using UnityEngine.UI;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Scopes
{
    /// <summary>
    /// Scene-level scope for the UI scene.
    /// Declares UI element bindings and runtime proxy bindings back to gameplay services in the game scene.
    /// </summary>
    public class UISceneScope : Scope
    {
        /// <summary>
        /// Declares the bindings used by the HUD and game-over UI.
        /// </summary>
        protected override void DeclareBindings()
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
                .FromRuntimeProxy();

            BindComponent<IScoreObservable, ScoreManager>()
                .FromRuntimeProxy();

            BindComponent<IEnemyObservable, EnemyManager>()
                .FromRuntimeProxy();

            BindComponent<ISceneManager, SceneManager>()
                .FromRuntimeProxy();
        }
    }
}