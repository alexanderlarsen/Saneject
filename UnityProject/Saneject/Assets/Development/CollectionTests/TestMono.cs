using UnityEngine;

namespace Development.ReadOnlyCollectionDrawer
{
    public class TestMono : MonoBehaviour, ITest
    {
        public void SaySomething()
        {
            Debug.Log($"{gameObject.name} says something");
        }
    }
}