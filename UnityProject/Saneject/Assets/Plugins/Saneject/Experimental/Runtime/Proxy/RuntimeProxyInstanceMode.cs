using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum RuntimeProxyInstanceMode
    {
        Transient,
        Singleton
    }
}