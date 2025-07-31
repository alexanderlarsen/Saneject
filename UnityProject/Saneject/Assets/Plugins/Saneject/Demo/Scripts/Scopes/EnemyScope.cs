using Plugins.Saneject.Demo.Scripts.Camera;
using Plugins.Saneject.Demo.Scripts.Enemies;
using Plugins.Saneject.Demo.Scripts.PlayerSystems;
using Plugins.Saneject.Demo.Scripts.UI.Enemy;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.Saneject.Demo.Scripts.Scopes
{
    /// <summary>
    /// DI scope for each enemy prefab.
    /// </summary>
    public class EnemyScope : Scope
    {
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
                .FromAssetLoad("Assets/Plugins/Saneject/Demo/Proxies/PlayerProxy.asset");

            BindAsset<IMainCamera, CameraControllerProxy>()
                .FromAssetLoad("Assets/Plugins/Saneject/Demo/Proxies/CameraControllerProxy.asset");
        }
    }
}