using System;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Samples.DemoGame.Scripts.UI.MVC;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.UI.GameOver
{
    /// <summary>
    /// Serializable view backing the game-over panel.
    /// Displays the final score, the number of enemies caught, and exposes the restart button.
    /// </summary>
    [Serializable]
    public class GameOverView : ViewBase
    {
        [Inject("enemiesLeftText"), SerializeField]
        private Text enemiesLeftText;

        [Inject("scoreText"), SerializeField]
        private Text scoreText;

        [Inject, SerializeField]
        private Button restartButton;

        /// <summary>
        /// Registers a callback for the restart button.
        /// </summary>
        /// <param name="action">The callback to invoke when the restart button is clicked.</param>
        public void OnRestartButtonClick(UnityAction action)
        {
            restartButton.onClick.AddListener(action);
        }

        /// <summary>
        /// Updates the score label shown on the game-over panel.
        /// </summary>
        /// <param name="points">The final score to display.</param>
        public void UpdateScore(int points)
        {
            scoreText.text = $"Score: {points}";
        }

        /// <summary>
        /// Updates the enemy summary shown on the game-over panel.
        /// </summary>
        /// <param name="enemiesCaught">The number of enemies caught during the round.</param>
        public void UpdateEnemiesCaught(int enemiesCaught)
        {
            enemiesLeftText.text = $"Enemies caught: {enemiesCaught}";
        }

        /// <summary>
        /// Clears the registered button listeners owned by this view.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            restartButton.onClick.RemoveAllListeners();
        }
    }
}
