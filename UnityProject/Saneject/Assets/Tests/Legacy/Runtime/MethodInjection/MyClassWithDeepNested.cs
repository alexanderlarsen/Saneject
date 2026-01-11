using System;
using UnityEngine;

namespace Tests.Legacy.Runtime.MethodInjection
{
    public class MyClassWithDeepNested : MonoBehaviour
    {
        [SerializeField]
        private MyNestedForDeep nestedClass;
    }

    [Serializable]
    public class MyNestedForDeep
    {
        [SerializeField]
        private MyDeepNestedClass deepNested;
    }
}