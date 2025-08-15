using System;
using UnityEngine;

namespace Plugins.Saneject.Samples.DemoGame.Scripts.Enemies
{
    public interface IEnemy
    {
        event Action OnEnemyCaught;
        Transform Transform { get; }
    }
}