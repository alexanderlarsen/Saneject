using System;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.PlayerSystems
{
    /// <summary>
    /// Handles input and movement logic for the player.
    /// Note that <see cref="PlayerMoveController" /> is a serializable class nested inside <see cref="Player" /> and dependency injection also works here.
    /// </summary>
    [Serializable]
    public class PlayerMoveController
    {
        [Inject, SerializeField]
        private CharacterController characterController;

        [SerializeField]
        private float moveSpeed = 6;

        public void Initialize()
        {
            // Reduce moveSpeed because moveInput is higher on touch than keyboard.
            if (Application.platform == RuntimePlatform.Android)
                moveSpeed /= 10f;
        }

        public void Move()
        {
            // Use old input system for compatibility with old Unity versions:
            Vector2 moveInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector3 moveDirection = new(moveInput.x, 0, moveInput.y);
            characterController.Move(moveDirection * (moveSpeed * Time.deltaTime));
        }
    }
}