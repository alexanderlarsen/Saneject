using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Camera;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.PlayerSystems;
using UnityEngine;
using UnityEngine.UI;
using EnemyMarker = Plugins.Saneject.Samples.DemoGame.Scripts.UI.Enemy.EnemyMarker;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Scopes
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
                .ToTarget<EnemyMarker>()
                .ToMember("markerTarget")
                .FromRootSelf();

            BindComponent<Image>()
                .ToTarget<EnemyMarker>()
                .ToMember("markerImage")
                .FromTargetSelf();

            BindComponent<IEnemyEvadeTarget, Player>()
                .FromProxy();

            BindComponent<IMainCamera, CameraController>()
                .FromProxy();
        }
    }
}