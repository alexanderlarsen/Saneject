namespace Plugins.Saneject.Demo.Scripts.Enemies
{
    /// <summary>
    /// Interface for objects that notify when they are "caught" (removed from the scene by the player).
    /// </summary>
    public interface IEnemyCatchNotifiable
    {
        void NotifyEnemyCaught();
    }
}