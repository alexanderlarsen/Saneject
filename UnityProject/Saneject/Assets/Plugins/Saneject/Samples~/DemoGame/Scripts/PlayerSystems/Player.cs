using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Camera;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Enemies;
using UnityEngine;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.PlayerSystems
{
    /// <summary>
    /// Player character component for the demo game.
    /// Exposes its position through <see cref="ICameraFollowTarget" /> and <see cref="IEnemyEvadeTarget" />,
    /// while delegating movement details to <see cref="PlayerMoveController" />.
    /// </summary>
    public class Player : MonoBehaviour, ICameraFollowTarget, IEnemyEvadeTarget
    {
        [SerializeField]
        private PlayerMoveController moveController;

        /// <summary>
        /// Gets the player's current world position.
        /// </summary>
        public Vector3 Position => transform.position;

        private void Awake()
        {
            moveController.Initialize();
        }

        private void Update()
        {
            moveController.Move();
        }
    }
}
