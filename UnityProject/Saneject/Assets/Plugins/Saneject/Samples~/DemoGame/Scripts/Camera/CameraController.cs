using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Camera
{
    /// <summary>
    /// Scene camera component that follows an injected <see cref="ICameraFollowTarget" />
    /// and exposes screen-space conversion through <see cref="IMainCamera" />.
    /// Marked <c>partial</c> so Saneject can generate serialized interface support for the target reference.
    /// </summary>
    public partial class CameraController : MonoBehaviour, IMainCamera
    {
        [Inject, SerializeInterface]
        private ICameraFollowTarget target;

        [Inject, SerializeField]
        private UnityEngine.Camera cam;

        [SerializeField]
        private Vector3 offset = new(0, 20, -10);

        private void LateUpdate()
        {
            transform.position = target.Position + offset;
            transform.LookAt(target.Position);
        }

        /// <summary>
        /// Converts a world position into screen coordinates using the injected Unity camera.
        /// </summary>
        /// <param name="worldPosition">The world position to convert.</param>
        /// <returns>The screen-space position for <paramref name="worldPosition" />.</returns>
        public Vector3 WorldToScreenPoint(Vector3 worldPosition)
        {
            return cam.WorldToScreenPoint(worldPosition);
        }
    }
}
