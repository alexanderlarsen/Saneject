using UnityEngine;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies
{
    /// <summary>
    /// Provides the world-space position that enemies should steer away from.
    /// </summary>
    public interface IEnemyEvadeTarget
    {
        /// <summary>
        /// Gets the target's current world position.
        /// </summary>
        Vector3 Position { get; }
    }
}
