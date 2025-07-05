using UnityEngine;

namespace Plugins.Saneject.Demo.Scripts.Enemies
{
    /// <summary>
    /// Interface for a target that enemies will try to evade/catch (usually the player).
    /// </summary>
    public interface IEnemyEvadeTarget
    {
        Vector3 Position { get; }
    }
}