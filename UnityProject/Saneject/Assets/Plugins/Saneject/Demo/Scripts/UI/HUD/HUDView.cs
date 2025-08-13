using System;
using Plugins.Saneject.Demo.Scripts.UI.MVC;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.Saneject.Demo.Scripts.UI.HUD
{
    /// <summary>
    /// View logic for displaying the player's HUD.
    /// Shows current score and remaining enemies.
    /// Uses IDs in <c>[Inject]</c> attributes to distinguish between multiple dependencies of the same type.
    /// </summary>
    [Serializable]
    public class HUDView : ViewBase
    {
        [Inject("enemiesLeftText"), SerializeField]
        private Text enemiesLeftText;

        [Inject("scoreText"), SerializeField]
        private Text scoreText;

        private int totalEnemies;

        /// <summary>
        /// Set number of total enemies.
        /// </summary>
        public void SetTotalEnemies(int totalEnemies)
        {
            this.totalEnemies = totalEnemies;
        }

        /// <summary>
        /// Updates the score text.
        /// </summary>
        public void UpdateScore(int points)
        {
            scoreText.text = $"Score: {points}";
        }

        /// <summary>
        /// Updates the enemies left text.
        /// </summary>
        public void UpdateEnemiesLeft(int enemiesLeft)
        {
            enemiesLeftText.text = $"Enemies left: {enemiesLeft}/{totalEnemies}";
        }
    }
}