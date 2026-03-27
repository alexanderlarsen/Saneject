using System;
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Proxy;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.RuntimeProxy
{
    public class RuntimeProxyTargetComponent : MonoBehaviour,
        IRuntimeProxyTarget,
        IAdvancedRuntimeProxyTarget,
        IRuntimeProxyGenericTarget<int>,
        IHiddenRuntimeProxyTarget,
        ISerializationCallbackReceiver,
        IRuntimeProxySwapTarget
    {
        public event Action OnTriggered;
        public event Action SomethingHappened;

        public string BaseProp => "Base";
        public string ReadOnlyProperty => "Read Only";
        public int ReadOnlyProp => 42;
        public int GenericValue => 99;

        public string ReadWriteProperty { get; set; }
        public string WriteOnlyProperty { private get; set; }
        public int WriteOnlyProp { private get; set; }

        public void BaseMethod()
        {
        }

        public void DoSomething()
        {
        }

        public string Echo(string value)
        {
            return value;
        }

        public int Add(int a, int b)
        {
            return a + b;
        }

        public void NoArgMethod()
        {
        }

        public int Sum(int a, int b)
        {
            return a + b;
        }

        public List<string> GetList()
        {
            return new();
        }

        public void HiddenMethod()
        {
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }

        public void SwapProxiesWithRealInstances()
        {
        }
    }
}
