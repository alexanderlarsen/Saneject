using System;
using System.Collections.Generic;

namespace Tests.Legacy.Runtime.Proxy
{
    public interface IAdvancedProxyTarget : IBaseInterface
    {
        // Events
        event Action SomethingHappened;

        // Properties
        int ReadOnlyProp { get; }
        string ReadWriteProp { get; set; }
        float WriteOnlyProp { set; }

        // Methods
        void NoArgMethod();
        string Echo(string input);

        int Sum(
            int a,
            int b = 5);

        List<string> GetList();
    }
}