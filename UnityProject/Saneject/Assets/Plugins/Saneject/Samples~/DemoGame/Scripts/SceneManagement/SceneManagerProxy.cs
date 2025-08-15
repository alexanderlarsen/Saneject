using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.InterfaceProxy;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.SceneManagement
{
    /// <summary>
    /// Proxy ScriptableObject for <see cref="SceneManager" />.
    /// Enables cross-scene or cross-prefab serialization of <see cref="ISceneManager" /> references.
    /// The Roslyn generator generates a <c>partial class</c> implementing the interfaces and forwards all calls/events to the resolved <see cref="SceneManager" /> instance at runtime.
    /// </summary>
    [GenerateInterfaceProxy]
    public partial class SceneManagerProxy : InterfaceProxyObject<SceneManager>
    {
    }

    /*
    Roslyn generated partial:

    public partial class SceneManagerProxy : ISceneManager
    {
       public void StartGame()
       {
           if (!instance) { instance = ResolveInstance(); }
           instance.StartGame();
       }

       public void RestartGame()
       {
           if (!instance) { instance = ResolveInstance(); }
           instance.RestartGame();
       }
    }
    */
}