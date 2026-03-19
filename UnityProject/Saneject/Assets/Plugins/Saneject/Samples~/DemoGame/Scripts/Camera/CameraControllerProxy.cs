using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Camera
{
    /// <summary>
    /// Runtime proxy stub for <see cref="CameraController" />.
    /// Used when an <see cref="IMainCamera" /> dependency crosses a Saneject context boundary
    /// and must be swapped to the real scene instance during scope startup.
    /// </summary>
    [GenerateRuntimeProxy]
    public partial class CameraControllerProxy : RuntimeProxy<CameraController>
    {
    }
}
