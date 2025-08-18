using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Camera
{
    /// <summary>
    /// Proxy ScriptableObject for <see cref="Samples.DemoGame.Scripts.Camera.CameraController" />.
    /// Enables cross-scene or cross-prefab serialization of <see cref="IMainCamera" /> references.
    /// The Roslyn generator generates a <c>partial class</c> implementing the interfaces and forwards all calls/events to the resolved <see cref="Samples.DemoGame.Scripts.Camera.CameraController" /> instance at runtime.
    /// </summary>
    [GenerateProxyObject]
    public partial class CameraControllerProxy : ProxyObject<CameraController>
    {
    }

    /*
    Roslyn generated partial:

    using UnityEngine;
    using System.Collections.Generic;

    // If CameraController implements e.g. IMainCamera:
    public partial class CameraControllerProxy : IMainCamera
    {
        public Vector3 WorldToScreenPoint(Vector3 worldPosition)
        {
            if (!instance) { instance = ResolveInstance(); }
            return instance.WorldToScreenPoint(worldPosition);
        }
    }

    // For each additional public interface on CameraController, all methods/properties/events are also forwarded.
    // Event forwarding uses subscription lists and proper add/remove forwarding as seen in the generator.
    */
}