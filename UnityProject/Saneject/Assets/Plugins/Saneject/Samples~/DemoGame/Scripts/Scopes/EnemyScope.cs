using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Camera;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using UnityEngine;
using UnityEngine.UI;
using CameraControllerProxy = Plugins.Saneject.Samples.DemoGame.Scripts.Camera.CameraControllerProxy;
using EnemyMarker = Plugins.Saneject.Samples.DemoGame.Scripts.UI.Enemy.EnemyMarker;
using PlayerProxy = Plugins.Saneject.Samples.DemoGame.Scripts.PlayerSystems.PlayerProxy;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Scopes
{
    /// <summary>
    /// DI scope for each enemy prefab.
    /// </summary>
    public class EnemyScope : Scope
    {
        [SerializeField]
        private PlayerProxy playerProxy;

        [SerializeField]
        private CameraControllerProxy cameraControllerProxy;

        public override void ConfigureBindings()
        {
            BindComponent<CharacterController>()
                .FromScopeSelf();

            BindComponent<Transform>()
                .WithId("MarkerTarget")
                .FromRootSelf();

            BindComponent<Image>()
                .WithId("MarkerImage")
                .FromTargetSelf()
                .WhereTargetIs<EnemyMarker>();

            BindAsset<IEnemyEvadeTarget, PlayerProxy>()
                .FromInstance(playerProxy);

            BindAsset<IMainCamera, CameraControllerProxy>()
                .FromInstance(cameraControllerProxy);
        }
    }
}