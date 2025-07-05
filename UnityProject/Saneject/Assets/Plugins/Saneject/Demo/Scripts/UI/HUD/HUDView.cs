using System;
using Plugins.Saneject.Demo.Scripts.UI.MVC;
using Plugins.Saneject.Runtime.Attributes;
using TMPro;
using UnityEngine;

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
        [Inject("enemiesTmp"), SerializeField]
        private TextMeshProUGUI enemiesLeftTmp;

        [Inject("scoreTmp"), SerializeField]
        private TextMeshProUGUI scoreTmp;

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
            scoreTmp.text = $"Score: {points}";
        }

        /// <summary>
        /// Updates the enemies left text.
        /// </summary>
        public void UpdateEnemiesLeft(int enemiesLeft)
        {
            enemiesLeftTmp.text = $"Enemies left: {enemiesLeft}/{totalEnemies}";
        }
    }
}