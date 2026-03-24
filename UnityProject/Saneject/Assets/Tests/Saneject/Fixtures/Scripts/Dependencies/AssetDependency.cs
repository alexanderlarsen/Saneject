using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts
{
    [CreateAssetMenu(menuName = "SanejectTests/Fixtures/AssetDependency")]
    public class AssetDependency : ScriptableObject, IDependency
    {
    }
}