using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Camera;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.Highscore;
using Plugins.Saneject.Samples.DemoGame.Scripts.PlayerSystems;
using UnityEngine;
using CameraController = Plugins.Saneject.Samples.DemoGame.Scripts.Camera.CameraController;
using EnemyManager = Plugins.Saneject.Samples.DemoGame.Scripts.Enemies.EnemyManager;
using GameStateManager = Plugins.Saneject.Samples.DemoGame.Scripts.GameState.GameStateManager;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Scopes
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

            BindAsset<GameObject>()
                .FromAssetLoad("Assets/Plugins/Saneject/Samples/DemoGame/Prefabs/Enemy.prefab")
                .WhereTargetIs<EnemyManager>()
                .WhereMemberNameIs("enemyPrefab");
        }
    }
}