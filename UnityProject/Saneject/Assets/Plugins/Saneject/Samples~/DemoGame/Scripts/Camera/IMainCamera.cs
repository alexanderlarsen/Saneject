using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Camera
{
    /// <summary>
    /// Exposes main camera functionality required by other systems.
    /// </summary>
    public interface IMainCamera
    {
        /// <summary>
        /// Converts a world position to screen coordinates.
        /// </summary>
        Vector3 WorldToScreenPoint(Vector3 worldPosition);
    }
}