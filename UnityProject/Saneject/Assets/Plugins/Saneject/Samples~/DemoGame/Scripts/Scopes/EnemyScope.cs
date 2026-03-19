using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Camera;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.PlayerSystems;
using Plugins.Saneject.Samples.DemoGame.Scripts.UI.Enemy;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Scopes
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
