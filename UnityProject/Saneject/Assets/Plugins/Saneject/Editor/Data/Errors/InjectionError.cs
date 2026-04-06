using System;
using System.ComponentModel;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Data.Errors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class InjectionError
    {
        protected InjectionError(
            Object logContext,
            bool suppressError,
            Exception exception)
        {
            LogContext = logContext;
            Exception = exception;
            SuppressError = exception == null && suppressError;
        }

        public Object LogContext { get; }
        public bool SuppressError { get; }
        public Exception Exception { get; }
    }
}