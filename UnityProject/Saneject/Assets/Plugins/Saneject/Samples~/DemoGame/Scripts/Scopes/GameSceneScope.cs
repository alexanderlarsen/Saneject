using Plugins.Saneject.Runtime.Scopes;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Camera;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Highscore;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.PlayerSystems;
using UnityEngine;
using CameraController = Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Camera.CameraController;
using EnemyManager = Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies.EnemyManager;
using GameStateManager = Plugins.SanejectLegacy.Samples.DemoGame.Scripts.GameState.GameStateManager;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Scopes
{
    /// <summary>
    /// Scene-level scope for the gameplay scene.
    /// Declares gameplay bindings and global registrations that other contexts consume through runtime proxies.
    /// </summary>
    public class GameSceneScope : Scope
    {
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
                .FromAssetLoad("Assets/Plugins/Saneject/Samples/DemoGame/Prefabs/Enemy.prefab");
        }
    }
}
