using System;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.SanejectLegacy.Samples.DemoGame.Scripts.UI.MVC;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.UI.HUD
{
    /// <summary>
    /// Serializable view backing the in-game HUD.
    /// Displays the current score and remaining enemy count.
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
        /// Stores the total number of enemies that were spawned for the round.
        /// </summary>
        /// <param name="totalEnemies">The total enemy count for the round.</param>
        public void SetTotalEnemies(int totalEnemies)
        {
            this.totalEnemies = totalEnemies;
        }

        /// <summary>
        /// Updates the score label shown in the HUD.
        /// </summary>
        /// <param name="points">The score to display.</param>
        public void UpdateScore(int points)
        {
            scoreText.text = $"Score: {points}";
        }

        /// <summary>
        /// Updates the remaining-enemies label shown in the HUD.
        /// </summary>
        /// <param name="enemiesLeft">The number of enemies still active.</param>
        public void UpdateEnemiesLeft(int enemiesLeft)
        {
            enemiesLeftText.text = $"Enemies left: {enemiesLeft}/{totalEnemies}";
        }
    }
}
