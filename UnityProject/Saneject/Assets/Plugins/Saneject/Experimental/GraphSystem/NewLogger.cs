using System.Collections.Generic;
using Plugins.Saneject.Experimental.GraphSystem.Data;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem
{
    public static class NewLogger
    {
        public static void LogBindingErrors(IReadOnlyList<BindingError> errors)
        {
            foreach (BindingError error in errors)
                Debug.LogError(error.ErrorMessage, error.PingObject);
        }
    }
}