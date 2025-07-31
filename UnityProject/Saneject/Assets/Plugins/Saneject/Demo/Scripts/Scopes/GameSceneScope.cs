using Plugins.Saneject.Demo.Scripts.Camera;
using Plugins.Saneject.Demo.Scripts.Enemies;
using Plugins.Saneject.Demo.Scripts.GameState;
using Plugins.Saneject.Demo.Scripts.Highscore;
using Plugins.Saneject.Demo.Scripts.PlayerSystems;
using Plugins.Saneject.Runtime.Scopes;

namespace Plugins.Saneject.Demo.Scripts.Scopes
{
    /// <summary>
    /// Scene-level DI scope for the main game scene.
    /// </summary>
    public class GameSceneScope : Scope
    {
        public override void ConfigureBindings()
        {
            BindGlobal<Player>()
                .FromScopeDescendants();

            BindGlobal<EnemyManager>()
                .FromScopeDescendants();

            BindGlobal<ScoreManager>()
                .FromScopeDescendants();

            BindGlobal<CameraController>()
                .FromScopeDescendants();

            BindGlobal<GameStateManager>()
                .FromScopeDescendants();

            BindComponent<ICameraFollowTarget, Player>()
                .FromScopeDescendants();

            BindComponent<IScoreUpdater, ScoreManager>()
                .FromScopeDescendants();

            BindComponent<IEnemyObservable, EnemyManager>()
                .FromScopeDescendants();

            BindComponent<UnityEngine.Camera>()
                .FromAnywhereInScene();

            BindMultipleComponents<IEnemy, Enemy>()
                .FromTargetDescendants();
        }
    }
}