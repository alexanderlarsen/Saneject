using Plugins.SanejectLegacy.Runtime.Attributes;
using UnityEngine;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.SceneManagement
{
    /// <summary>
    /// Bootstraps the game and UI scenes.
    /// </summary>
    public partial class Bootstrapper : MonoBehaviour
    {
        [SerializeInterface]
        private ISceneManager sceneManager;

        private void Start()
        {
            sceneManager.StartGame();
        }
    }
}