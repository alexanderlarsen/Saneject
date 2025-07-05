using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.Saneject.Demo.Scripts.SceneManagement
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