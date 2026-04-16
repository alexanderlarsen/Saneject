using System;
using System.Collections.Generic;

namespace Tests.Saneject.Fixtures.Scripts.RuntimeProxy
{
    public interface IBaseRuntimeProxyTarget
    {
        string BaseProp { get; }
        void BaseMethod();
    }

    public interface IRuntimeProxyTarget
    {
        event Action OnTriggered;

        string ReadOnlyProperty { get; }
        string ReadWriteProperty { get; set; }
        string WriteOnlyProperty { set; }

        void DoSomething();
        string Echo(string value);
        int Add(int a, int b);
    }

    public interface IAdvancedRuntimeProxyTarget : IBaseRuntimeProxyTarget
    {
        event Action SomethingHappened;

        int ReadOnlyProp { get; }
        string ReadWriteProperty { get; set; }
        int WriteOnlyProp { set; }

        void NoArgMethod();
        string Echo(string value);
        int Sum(int a, int b);
        List<string> GetList();
    }

    public interface IRuntimeProxyGenericTarget<T>
    {
        T GenericValue { get; }
    }

    internal interface IHiddenRuntimeProxyTarget
    {
        void HiddenMethod();
    }
}
