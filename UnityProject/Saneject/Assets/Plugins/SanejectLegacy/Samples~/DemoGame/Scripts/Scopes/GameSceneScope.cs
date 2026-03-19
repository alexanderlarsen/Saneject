using Plugins.SanejectLegacy.Runtime.Scopes;
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
                .ToTarget<EnemyManager>()
                .ToMember("enemyPrefab")
                .FromAssetLoad("Assets/Plugins/Saneject/Legacy/Samples/DemoGame/Prefabs/Enemy.prefab");
        }
    }
}