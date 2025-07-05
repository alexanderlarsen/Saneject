using UnityEngine;

namespace Plugins.Saneject.Demo.Scripts.Camera
{
    /// <summary>
    /// Represents a target that the camera can follow.
    /// </summary>
    public interface ICameraFollowTarget
    {
        /// <summary>
        /// The world position of the target.
        /// </summary>
        Vector3 Position { get; }
    }
}