using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.Saneject.Demo.Scripts.Camera
{
    /// <summary>
    /// Controls the main camera movement to follow a target.
    /// Implements <see cref="IMainCamera" /> for screen space conversion.
    /// Note: This class is marked as <c>partial</c> because it uses <see cref="SerializeInterfaceAttribute" />.
    /// The Roslyn generator <c>SerializeInterfaceGenerator.dll</c> automatically generates a matching partial that implements the serialized backing field and assigns it to the interface reference after deserialization.
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

        public Vector3 WorldToScreenPoint(Vector3 worldPosition)
        {
            return cam.WorldToScreenPoint(worldPosition);
        }
    }

    /*
    Roslyn generated partial:

    public partial class CameraController : ISerializationCallbackReceiver
    {
        [SerializeField, InterfaceBackingField(interfaceType: typeof(ICameraFollowTarget), isInjected: true, injectId: null)]
        private UnityEngine.Object __target;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            target = __target as ICameraFollowTarget;
        }
    }
    */
}