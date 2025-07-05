namespace Plugins.Saneject.Demo.Scripts.Highscore
{
    /// <summary>
    /// Interface for modifying the current score.
    /// </summary>
    public interface IScoreUpdater
    {
        /// <summary>
        /// Adds the specified number of points to the current score.
        /// </summary>
        void AddPoints(int points);
    }
}