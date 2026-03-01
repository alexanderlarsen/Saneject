using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Editor.Data.Logging
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum ErrorType
    {
        InvalidBinding,
        MissingBinding,
        MissingGlobalObject,
        MissingDependency,
        MissingDependencies,
        MethodInvocationException,
        BindingFilterException
    }
}