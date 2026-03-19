using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.GameState
{
    /// <summary>
    /// Runtime proxy stub for <see cref="GameStateManager" />.
    /// Used when <see cref="IGameStateObservable" /> must cross a Saneject context boundary and
    /// be swapped to the real scene instance during scope startup.
    /// </summary>
    [GenerateRuntimeProxy]
    public partial class GameStateManagerProxy : RuntimeProxy<GameStateManager>
    {
    }
}
