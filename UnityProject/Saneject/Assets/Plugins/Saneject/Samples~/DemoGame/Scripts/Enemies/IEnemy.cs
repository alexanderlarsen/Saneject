using System;
using UnityEngine;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies
{
    /// <summary>
    /// Represents a spawned enemy instance that can be tracked by gameplay and UI systems.
    /// </summary>
    public interface IEnemy
    {
        /// <summary>
        /// Raised when this enemy is caught and is about to be removed from play.
        /// </summary>
        event Action<IEnemy> OnEnemyCaught;

        /// <summary>
        /// Gets the enemy transform used for positioning and marker tracking.
        /// </summary>
        Transform Transform { get; }
    }
}
