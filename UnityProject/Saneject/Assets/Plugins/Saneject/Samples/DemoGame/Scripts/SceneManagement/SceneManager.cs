using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.SceneManagement
{
    /// <summary>
    /// Implements <see cref="ISceneManager" /> to coordinate scene loading and unloading across game states.
    /// Loads the game and UI scenes, and removes the start scene on game start.
    /// Fully resets the game by unloading all scenes and reloading the start scene on restart.
    /// </summary>
    public class SceneManager : MonoBehaviour, ISceneManager
    {
        private const string StartSceneName = "StartScene";
        private const string GameSceneName = "GameScene";
        private const string UISceneName = "UIScene";

        public void StartGame()
        {
            StopAllCoroutines();
            StartCoroutine(StartGameRoutine());
        }

        public void RestartGame()
        {
            StopAllCoroutines();
            StartCoroutine(RestartGameRoutine());
        }

        private IEnumerator StartGameRoutine()
        {
            AsyncOperation gameSceneLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GameSceneName, LoadSceneMode.Additive);

            while (gameSceneLoad is { isDone: false })
                yield return null;

            AsyncOperation uiSceneLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UISceneName, LoadSceneMode.Additive);

            while (uiSceneLoad is { isDone: false })
                yield return null;

            AsyncOperation startSceneUnload = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(StartSceneName);

            while (startSceneUnload is { isDone: false })
                yield return null;
        }

        private IEnumerator RestartGameRoutine()
        {
            AsyncOperation startSceneLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(StartSceneName, LoadSceneMode.Single);

            while (startSceneLoad is { isDone: false })
                yield return null;
        }
    }
}