using Plugins.Saneject.Demo.Scripts.Camera;
using Plugins.Saneject.Demo.Scripts.Enemies;
using Plugins.Saneject.Demo.Scripts.GameState;
using Plugins.Saneject.Demo.Scripts.Highscore;
using Plugins.Saneject.Demo.Scripts.PlayerSystems;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;

namespace Plugins.Saneject.Demo.Scripts.Scopes
{
    /// <summary>
    /// Scene-level DI scope for the main game scene.
    /// </summary>
    public class GameSceneScope : Scope
    {
        public override void Configure()
        {
            Bind<Player>().AsGlobal().FromScopeDescendants();
            Bind<EnemyManager>().AsGlobal().FromScopeDescendants();
            Bind<ScoreManager>().AsGlobal().FromScopeDescendants();
            Bind<CameraController>().AsGlobal().FromScopeDescendants();
            Bind<GameStateManager>().AsGlobal().FromScopeDescendants();

            Bind<ICameraFollowTarget, Player>().FromScopeDescendants();
            Bind<IScoreUpdater, ScoreManager>().FromScopeDescendants();
            Bind<IEnemyObservable, EnemyManager>().FromScopeDescendants();

            Bind<UnityEngine.Camera>().FromAnywhereInScene();

            Bind<GameObject>().WithId("EnemyPrefab").FromAssetLoad("Assets/Plugins/Saneject/Demo/Prefabs/Enemy.prefab");
        }
    }
}