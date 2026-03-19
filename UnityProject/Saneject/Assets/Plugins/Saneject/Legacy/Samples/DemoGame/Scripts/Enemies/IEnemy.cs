using System;
using UnityEngine;

namespace Plugins.Saneject.Legacy.Samples.DemoGame.Scripts.Enemies
{
    public interface IEnemy
    {
        event Action<IEnemy> OnEnemyCaught;
        Transform Transform { get; }
    }
}