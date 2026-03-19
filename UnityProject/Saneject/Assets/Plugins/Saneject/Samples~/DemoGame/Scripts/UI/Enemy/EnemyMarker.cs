using Plugins.Saneject.Runtime.Attributes;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.UI.Enemy
{
    /// <summary>
    /// HUD marker that points toward an enemy when it moves off-screen.
    /// Marked <c>partial</c> so Saneject can serialize the injected <see cref="IMainCamera" /> interface reference.
    /// </summary>
    public partial class EnemyMarker : MonoBehaviour
    {
        [Inject, SerializeInterface]
        private IMainCamera mainCamera;

        [Inject, SerializeField]
        private Transform markerTarget;

        [Inject, SerializeField]
        private Image markerImage;

        [SerializeField]
        private float padding = 20f;

        private void LateUpdate()
        {
            Vector3 targetScreenPosition = mainCamera.WorldToScreenPoint(markerTarget.transform.position);

            bool isTargetOutsideScreen = targetScreenPosition.x < 0 || targetScreenPosition.x > Screen.width || targetScreenPosition.y < 0 || targetScreenPosition.y > Screen.height;

            if (!isTargetOutsideScreen)
            {
                markerImage.color = Color.clear;
                return;
            }

            markerImage.color = Color.red;

            Vector3 clampedPosition = new(
                Mathf.Clamp(targetScreenPosition.x, padding, Screen.width - padding),
                Mathf.Clamp(targetScreenPosition.y, padding, Screen.height - padding),
                targetScreenPosition.z
            );

            transform.position = clampedPosition;
        }
    }
}
