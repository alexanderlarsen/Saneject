using System;
using Plugins.Saneject.Demo.Scripts.UI.MVC;
using Plugins.Saneject.Runtime.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Plugins.Saneject.Demo.Scripts.UI.GameOver
{
    /// <summary>
    /// View logic for the Game Over screen.
    /// Displays score, enemies caught, and handles the restart button.
    /// Uses IDs in <c>[Inject]</c> attributes to distinguish between multiple dependencies of the same type.
    /// </summary>
    [Serializable]
    public class GameOverView : ViewBase
    {
        [Inject("enemiesTmp"), SerializeField]
        private TextMeshProUGUI enemiesLeftTmp;

        [Inject("scoreTmp"), SerializeField]
        private TextMeshProUGUI scoreTmp;

        [Inject("restartButton"), SerializeField]
        private Button restartButton;

        /// <summary>
        /// Registers a callback for the restart button.
        /// </summary>
        public void OnRestartButtonClick(UnityAction action)
        {
            restartButton.onClick.AddListener(action);
        }

        /// <summary>
        /// Updates the score text.
        /// </summary>
        public void UpdateScore(int points)
        {
            scoreTmp.text = $"Score: {points}";
        }

        /// <summary>
        /// Updates the enemies caught text.
        /// </summary>
        public void UpdateEnemiesCaught(int enemiesLeft)
        {
            enemiesLeftTmp.text = $"Enemies caught: {enemiesLeft}";
        }

        public override void Dispose()
        {
            base.Dispose();
            restartButton.onClick.RemoveAllListeners();
        }
    }
}