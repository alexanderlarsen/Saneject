using Plugins.SanejectLegacy.Runtime.Scopes;
using UnityEngine;

namespace Plugins.SanejectLegacy.Samples.DemoGame.Scripts.Scopes
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