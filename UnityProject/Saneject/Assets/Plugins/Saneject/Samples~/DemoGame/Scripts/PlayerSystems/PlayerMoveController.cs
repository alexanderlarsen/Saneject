using System;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.PlayerSystems
{
    /// <summary>
    /// Serializable helper that handles player input and movement.
    /// Saneject injects into this nested object as part of the owning <see cref="Player" /> during editor injection.
    /// </summary>
    [Serializable]
    public class PlayerMoveController
    {
        [Inject, SerializeField]
        private CharacterController characterController;

        [SerializeField]
        private float moveSpeed = 6;

        /// <summary>
        /// Applies one-time platform-specific movement adjustments.
        /// </summary>
        public void Initialize()
        {
            // Reduce moveSpeed because moveInput is higher on touch than keyboard.
            if (Application.platform == RuntimePlatform.Android)
                moveSpeed /= 10f;
        }

        /// <summary>
        /// Reads input and moves the player for the current frame.
        /// </summary>
        public void Move()
        {
            // Use old input system for compatibility with old Unity versions:
            Vector2 moveInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector3 moveDirection = new(moveInput.x, 0, moveInput.y);
            characterController.Move(moveDirection * (moveSpeed * Time.deltaTime));
        }
    }
}
