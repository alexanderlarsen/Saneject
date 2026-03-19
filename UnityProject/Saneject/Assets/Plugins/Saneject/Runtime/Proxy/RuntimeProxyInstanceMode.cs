using System.ComponentModel;

namespace Plugins.Saneject.Runtime.Proxy
{
    /// <summary>
    /// Determines how a runtime proxy manages its resolved instance lifecycle.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum RuntimeProxyInstanceMode
    {
        /// <summary>
        /// Creates a new instance each time the proxy is resolved.
        /// </summary>
        Transient,

        /// <summary>
        /// Creates the instance once and caches it. The cached instance is registered in <see cref="Scopes.GlobalScope"/> to survive scene transitions.
        /// </summary>
        Singleton
    }
}