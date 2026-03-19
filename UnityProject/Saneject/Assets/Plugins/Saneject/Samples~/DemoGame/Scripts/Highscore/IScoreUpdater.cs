namespace Plugins.Saneject.Samples.DemoGame.Scripts.Highscore
{
    /// <summary>
    /// Mutates the score tracked by the demo game's score system.
    /// </summary>
    public interface IScoreUpdater
    {
        /// <summary>
        /// Adds the specified number of points to the current score.
        /// </summary>
        /// <param name="points">The number of points to add.</param>
        void AddPoints(int points);
    }
}
