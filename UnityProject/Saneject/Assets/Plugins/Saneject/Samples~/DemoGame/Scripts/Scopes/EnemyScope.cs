using Plugins.Saneject.Runtime.Scopes;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Camera;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.PlayerSystems;
using UnityEngine;
using UnityEngine.UI;
using CameraController = Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Camera.CameraController;
using EnemyMarker = Plugins.SanejectLegacy.Samples.DemoGame.Scripts.UI.Enemy.EnemyMarker;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Scopes
{
    /// <summary>
    /// Prefab-local scope for each enemy instance.
    /// Declares local bindings for enemy components and runtime proxy bindings to scene-owned services.
    /// </summary>
    public class EnemyScope : Scope
    {
        /// <summary>
        /// Declares the bindings used by the enemy prefab and its marker UI.
        /// </summary>
        protected override void DeclareBindings()
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

            // The two bindings below both resolve from the GlobalScope.
            // When no strategy is specified, the binding will default to GlobalScope resolution.
            BindComponent<IEnemyEvadeTarget, Player>()
                .FromRuntimeProxy()
                .FromGlobalScope();

            BindComponent<IMainCamera, CameraController>()
                .FromRuntimeProxy();
        }
    }
}
