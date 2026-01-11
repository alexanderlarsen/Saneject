using System;

namespace Tests.Legacy.Runtime.Proxy
{
    public interface IProxyTarget
    {
        event Action OnTriggered;
        int ReadOnlyProperty { get; }
        string ReadWriteProperty { get; set; }
        float WriteOnlyProperty { set; }

        void DoSomething();
        string Echo(string input);

        int Add(
            int a,
            int b);
    }
}