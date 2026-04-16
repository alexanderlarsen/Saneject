namespace Plugins.Saneject.Samples.DemoGame.Scripts.SceneManagement
{
    /// <summary>
    /// Coordinates the sample game's scene transitions.
    /// </summary>
    public interface ISceneManager
    {
        /// <summary>
        /// Starts the game by loading the gameplay and UI scenes, then unloading the start scene.
        /// </summary>
        void StartGame();

        /// <summary>
        /// Restarts the sample by returning to the start scene.
        /// </summary>
        void RestartGame();
    }
}
