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
        public override void Configure()
        {
            RegisterComponent<CharacterController>().FromScopeSelf();
            RegisterComponent<Transform>().WithId("MarkerTarget").FromRootSelf();
            RegisterComponent<Image>().WithId("MarkerImage").FromTargetSelf().WhereTargetIs<EnemyMarker>();

            RegisterObject<IEnemyCatchNotifiable, EnemyManagerProxy>().FromAssetLoad("Assets/Plugins/Saneject/Demo/Proxies/EnemyManagerProxy.asset");
            RegisterObject<IEnemyEvadeTarget, PlayerProxy>().FromAssetLoad("Assets/Plugins/Saneject/Demo/Proxies/PlayerProxy.asset");
            RegisterObject<IMainCamera, CameraControllerProxy>().FromAssetLoad("Assets/Plugins/Saneject/Demo/Proxies/CameraControllerProxy.asset");
        }
    }
}  