using Plugins.Saneject.Legacy.Runtime.Scopes;
using Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Camera;
using Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.PlayerSystems;
using UnityEngine;
using UnityEngine.UI;
using CameraController = Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Camera.CameraController;
using EnemyMarker = Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.UI.Enemy.EnemyMarker;

namespace Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Scopes
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