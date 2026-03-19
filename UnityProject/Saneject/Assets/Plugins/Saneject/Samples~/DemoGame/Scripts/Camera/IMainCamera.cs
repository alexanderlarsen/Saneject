using UnityEngine;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Camera
{
    /// <summary>
    /// Exposes the camera functionality that other gameplay and UI systems depend on.
    /// </summary>
    public interface IMainCamera
    {
        /// <summary>
        /// Converts a world position to screen coordinates.
        /// </summary>
        /// <param name="worldPosition">The world position to convert.</param>
        /// <returns>The converted screen-space position.</returns>
        Vector3 WorldToScreenPoint(Vector3 worldPosition);
    }
}
