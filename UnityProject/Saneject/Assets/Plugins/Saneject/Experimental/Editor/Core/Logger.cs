using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class Logger
    {
        public static void LogBindingErrors(IReadOnlyList<BindingError> errors)
        {
            foreach (BindingError error in errors)
                Debug.LogError(error.ErrorMessage, error.PingObject);
        }
    }
}