using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.SceneManagement
{
    /// <summary>
    /// Start-scene entry point that kicks off the sample's additive scene flow.
    /// Marked <c>partial</c> so Saneject can serialize the injected <see cref="ISceneManager" /> reference.
    /// </summary>
    public partial class Bootstrapper : MonoBehaviour
    {
        [Inject, SerializeInterface]
        private ISceneManager sceneManager;

        private void Start()
        {
            sceneManager.StartGame();
        }
    }
}
