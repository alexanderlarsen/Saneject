using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Legacy.Runtime.Proxy
{
    public class ProxyTarget : MonoBehaviour, IProxyTarget, IAdvancedProxyTarget
    {
        private float writeOnlyProperty;
        public event Action SomethingHappened;

        public event Action OnTriggered;
        public string BaseProp => "base";

        public int ReadOnlyProp => 42;

        public int ReadOnlyProperty { get; }

        public string ReadWriteProp { get; set; }

        public float WriteOnlyProp
        {
            set
            {
                /* ignored */
            }
        }

        public string ReadWriteProperty { get; set; }

        public float WriteOnlyProperty
        {
            set => writeOnlyProperty = value;
        }

        public void BaseMethod()
        {
        }

        public void NoArgMethod()
        {
        }

        public void DoSomething()
        {
        }

        public string Echo(string input)
        {
            return input;
        }

        public int Add(
            int a,
            int b)
        {
            return a + b;
        }

        public int Sum(
            int a,
            int b = 5)
        {
            return a + b;
        }

        public List<string> GetList()
        {
            return new List<string> { "A", "B" };
        }
    }
}