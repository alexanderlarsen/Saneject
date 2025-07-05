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
            RegisterGlobalComponent<Player>().FromScopeDescendants();
            RegisterGlobalComponent<EnemyManager>().FromScopeDescendants();
            RegisterGlobalComponent<ScoreManager>().FromScopeDescendants();
            RegisterGlobalComponent<CameraController>().FromScopeDescendants();
            RegisterGlobalComponent<GameStateManager>().FromScopeDescendants();

            RegisterComponent<ICameraFollowTarget, Player>().FromScopeDescendants();
            RegisterComponent<IScoreUpdater, ScoreManager>().FromScopeDescendants();
            RegisterComponent<IEnemyObservable, EnemyManager>().FromScopeDescendants();

            RegisterComponent<UnityEngine.Camera>().FromAnywhereInScene();

            RegisterObject<GameObject>().WithId("EnemyPrefab").FromAssetLoad("Assets/Plugins/Saneject/Demo/Prefabs/Enemy.prefab");
        }
    }
}