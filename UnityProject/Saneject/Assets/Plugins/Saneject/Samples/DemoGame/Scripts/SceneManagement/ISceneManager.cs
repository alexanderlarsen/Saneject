namespace Plugins.Saneject.Samples.DemoGame.Scripts.SceneManagement
{
    /// <summary>
    /// Interface for scene transition logic.
    /// Implemented by the in-game scene manager.
    /// </summary>
    public interface ISceneManager
    {
        /// <summary>
        /// Starts the game by loading game and UI scenes, and unloading the start scene.
        /// </summary>
        void StartGame();

        /// <summary>
        /// Restarts the game by unloading all current scenes and reloading the start scene.
        /// </summary>
        void RestartGame();
    }
}