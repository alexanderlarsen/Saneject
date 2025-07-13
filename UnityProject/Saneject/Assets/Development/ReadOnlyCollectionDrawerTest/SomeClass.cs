using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

namespace Development.ReadOnlyCollectionDrawer
{
    public class SomeClass : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private List<string> stringListReadOnly = new() { "1", "2", "4" };

        [SerializeField, Inject]
        private string[] stringArrayReadOnly = { "1", "2", "3" };

        [SerializeField]
        private List<string> stringList = new() { "1", "2", "3" };

        [SerializeField]
        private string[] stringArray = { "1", "2", "3" };
    }
}