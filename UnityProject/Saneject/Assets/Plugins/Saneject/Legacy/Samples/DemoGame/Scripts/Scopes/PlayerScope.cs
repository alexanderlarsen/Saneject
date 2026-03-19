using Plugins.Saneject.Legacy.Runtime.Scopes;
using UnityEngine;

namespace Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Scopes
{
    /// <summary>
    /// DI scope for the player.
    /// </summary>
    public class PlayerScope : Scope
    {
        public override void ConfigureBindings()
        {
            BindComponent<CharacterController>()
                .FromScopeSelf();
        }
    }
}