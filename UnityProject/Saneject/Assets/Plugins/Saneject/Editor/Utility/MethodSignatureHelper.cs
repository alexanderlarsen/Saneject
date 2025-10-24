using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Plugins.Saneject.Editor.Utility
{
    public static class MethodSignatureHelper
    {
        /// <summary>
        /// Builds a concise, human-readable signature for an injected method,
        /// showing its declaring type, method name, parameter types, and optional Inject ID.
        /// Example: [Injected method: MyClass.Inject(MyDependency, IDependency)]
        /// </summary>
        public static string GetInjectedMethodSignature(
            MethodInfo methodInfo,
            object declaringInstance,
            string injectId)
        {
            if (methodInfo == null)
                return "[Injected method: <null>]";

            Type declaringType = declaringInstance?.GetType() ?? methodInfo.DeclaringType;
            StringBuilder sb = new();

            sb.Append("[Injected method: ");
            sb.Append(declaringType?.Name ?? "<unknown>");
            sb.Append(".");
            sb.Append(methodInfo.Name);
            sb.Append("(");

            ParameterInfo[] parameters = methodInfo.GetParameters();

            if (parameters.Length > 0)
            {
                string paramList = string.Join(", ",
                    parameters.Select(p =>
                    {
                        Type pt = p.ParameterType;
                        string displayType;

                        if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            string inner = pt.GetGenericArguments()[0].Name;
                            displayType = $"List<{inner}>";
                        }
                        else if (pt.IsArray)
                        {
                            displayType = $"{pt.GetElementType()?.Name}[]";
                        }
                        else
                        {
                            displayType = pt.Name;
                        }

                        return displayType;
                    }));

                sb.Append(paramList);
            }

            sb.Append(")");

            if (!string.IsNullOrWhiteSpace(injectId))
            {
                sb.Append(" | ID: ");
                sb.Append(injectId);
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}