using System;
using UnityEngine;

namespace Plugins.Saneject.Demo.Scripts.Enemies
{
    public interface IEnemy
    {
        event Action OnEnemyCaught; 
        Transform Transform { get; }
    }
}