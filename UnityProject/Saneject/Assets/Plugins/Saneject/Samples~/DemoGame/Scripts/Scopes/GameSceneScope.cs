using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Camera;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.GameState;
using Plugins.Saneject.Samples.DemoGame.Scripts.Highscore;
using Plugins.Saneject.Samples.DemoGame.Scripts.PlayerSystems;
using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Scopes
{
    /// <summary>
    /// Scene-level scope for the gameplay scene.
    /// Declares gameplay bindings and global registrations that other contexts consume through runtime proxies.
    /// </summary>
    public class GameSceneScope : Scope
    {
        [SerializeField]
        private GameObject enemyPrefab;
        
        /// <summary>
        /// Declares the gameplay bindings and global registrations used throughout the sample.
        /// </summary>
        protected override void DeclareBindings()
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
                .FromAnywhere();

            BindAsset<GameObject>()
                .ToTarget<EnemyManager>()
                .ToMember("enemyPrefab")
                .FromInstance(enemyPrefab);
        }
    }
}
