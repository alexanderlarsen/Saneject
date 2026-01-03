using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Utils;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class Error
    {
        private Error(
            ErrorType errorType,
            string errorMessage,
            Object logContext)
        {
            ErrorType = errorType;
            ErrorMessage = errorMessage;
            LogContext = logContext;
        }

        public ErrorType ErrorType { get; }
        public string ErrorMessage { get; }
        public Object LogContext { get; }

        public static Error CreateInvalidBindingError(
            string errorMessage,
            BindingNode bindingNode)
        {
            return new Error
            (
                ErrorType.InvalidBinding,
                $"Invalid binding {SignatureBuilder.GetBindingSignature(bindingNode)} {errorMessage}",
                bindingNode.ScopeNode.TransformNode.Transform
            );
        }

        public static Error CreateMissingBindingError(FieldNode fieldNode)
        {
            string expectedBindingSignature = SignatureBuilder.GetPartialBindingSignature(fieldNode.RequestedType, fieldNode.IsCollection, fieldNode.ComponentNode.TransformNode.NearestScopeNode);

            return new Error
            (
                ErrorType.MissingBinding,
                $"Missing binding {expectedBindingSignature} {SignatureBuilder.GetFieldSignature(fieldNode)}",
                fieldNode.ComponentNode.TransformNode.Transform
            );
        }
        
        public static Error CreateMissingGlobalDependencyError(
            BindingNode bindingNode,
            HashSet<Type> rejectedTypes)
        {
            StringBuilder msg = new();

            msg.Append($"Missing dependency {SignatureBuilder.GetBindingSignature(bindingNode)}");

            if (rejectedTypes is { Count: > 0 })
            {
                string typeList = string.Join(", ", rejectedTypes.Select(t => t.Name));
                msg.AppendLine();
                msg.AppendLine($"Candidates rejected due to scene/prefab or prefab/prefab context mismatch: {typeList}.");
                msg.AppendLine("Use ProxyObjects for proper cross-context references, or disable filtering in User Settings (not recommended).");
            }

            return new Error
            (
                ErrorType.MissingGlobalDependency,
                msg.ToString(),
                bindingNode.ScopeNode.TransformNode.Transform
            );
        }

        public static Error CreateMissingDependencyError(
            BindingNode bindingNode,
            FieldNode fieldNode,
            HashSet<Type> rejectedTypes)
        {
            StringBuilder msg = new();

            msg.Append($"Missing dependency {SignatureBuilder.GetBindingSignature(bindingNode)} {SignatureBuilder.GetFieldSignature(fieldNode)}");

            if (rejectedTypes is { Count: > 0 })
            {
                string typeList = string.Join(", ", rejectedTypes.Select(t => t.Name));
                msg.AppendLine();
                msg.AppendLine($"Candidates rejected due to scene/prefab or prefab/prefab context mismatch: {typeList}.");
                msg.AppendLine("Use ProxyObjects for proper cross-context references, or disable filtering in User Settings (not recommended).");
            }

            return new Error
            (
                ErrorType.MissingDependency,
                msg.ToString(),
                bindingNode.ScopeNode.TransformNode.Transform
            );
        }
    }
}