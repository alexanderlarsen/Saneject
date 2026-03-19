using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Camera
{
    /// <summary>
    /// Provides the world-space position that the demo game's camera should follow.
    /// </summary>
    public interface ICameraFollowTarget
    {
        /// <summary>
        /// Gets the target's current world position.
        /// </summary>
        Vector3 Position { get; }
    }
}
