using Plugins.Saneject.Demo.Scripts.Camera;
using Plugins.Saneject.Demo.Scripts.Enemies;
using UnityEngine;

namespace Plugins.Saneject.Demo.Scripts.PlayerSystems
{
    /// <summary>
    /// Represents the player character. Implements both <see cref="ICameraFollowTarget" /> and <see cref="IEnemyEvadeTarget" />.
    /// Provides a position reference for camera and enemy systems, and delegates movement to <see cref="PlayerMoveController" />.
    /// </summary>
    public class Player : MonoBehaviour, ICameraFollowTarget, IEnemyEvadeTarget
    {
        [SerializeField]
        private PlayerMoveController moveController;

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