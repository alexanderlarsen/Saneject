using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Scopes
{
    /// <summary>
    /// Prefab-local scope for the player character.
    /// </summary>
    public class PlayerScope : Scope
    {
        /// <summary>
        /// Declares the bindings required by components inside the player prefab.
        /// </summary>
        protected override void DeclareBindings()
        {
            BindComponent<CharacterController>()
                .FromScopeSelf();
        }
    }
}
