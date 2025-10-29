using System;
using UnityEngine;

namespace Tests.Runtime.MethodInjection
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