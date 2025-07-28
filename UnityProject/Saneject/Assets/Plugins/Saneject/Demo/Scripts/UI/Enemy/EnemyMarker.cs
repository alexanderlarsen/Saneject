using Plugins.Saneject.Demo.Scripts.Camera;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.Saneject.Demo.Scripts.UI.Enemy
{
    /// <summary>
    /// UI element that points to off-screen enemies by clamping their position to screen bounds.
    /// Note: This class is marked <c>partial</c> because it uses <see cref="SerializeInterfaceAttribute" />.
    /// The Roslyn source generator <c>SerializeInterfaceGenerator.dll</c> generates a matching partial that implements the serialized backing field and assigns it to the interface reference.
    /// </summary>
    public partial class EnemyMarker : MonoBehaviour
    {
        [Inject, SerializeInterface]
        private IMainCamera mainCamera;

        [Inject("MarkerTarget"), SerializeField]
        private Transform markerTarget;

        [Inject("MarkerImage"), SerializeField]
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

    /*
    Roslyn generated partial:

    public partial class EnemyMarker : ISerializationCallbackReceiver
    {
        [SerializeField, InterfaceBackingField(interfaceType: typeof(IMainCamera), isInjected: true, injectId: null)]
        private Object __mainCamera;

        public void OnBeforeSerialize()
        {
    #if UNITY_EDITOR
            __mainCamera = mainCamera as Object;
    #endif
        }

        public void OnAfterDeserialize()
        {
            mainCamera = __mainCamera as IMainCamera;
        }
    }
    */
}