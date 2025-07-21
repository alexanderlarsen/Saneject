using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Plugins.Saneject.Demo.Scripts.Scopes
{
    /// <summary>
    /// DI scope for the player.
    /// </summary>
    public class PlayerScope : Scope
    {
        public override void Configure()
        {
            Bind<CharacterController>().FromScopeSelf();
            Bind<PlayerInput>().FromScopeSelf();
        }
    }
}