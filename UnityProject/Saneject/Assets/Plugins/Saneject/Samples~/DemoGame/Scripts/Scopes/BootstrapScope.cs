using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.SceneManagement;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Scopes
{
    /// <summary>
    /// Bootstrap scope for the start scene.
    /// Declares the runtime proxy binding that creates the sample's shared <see cref="ISceneManager" /> instance.
    /// </summary>
    public class BootstrapScope : Scope
    {
        /// <summary>
        /// Declares the bindings needed before the gameplay and UI scenes are loaded.
        /// </summary>
        protected override void DeclareBindings()
        {
            // When the runtime proxy is created, it will create a new instance of the
            // SceneManager class on a new GameObject, and register it to the GlobalScope (.AsSingleton)
            BindComponent<ISceneManager, SceneManager>()
                .FromRuntimeProxy()
                .FromNewComponentOnNewGameObject()
                .AsSingleton();
        }
    }
}