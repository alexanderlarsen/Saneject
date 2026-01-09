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
            Object logContext,
            bool suppressError,
            Exception exception = null)
        {
            ErrorType = errorType;
            ErrorMessage = errorMessage;
            LogContext = logContext;
            SuppressError = suppressError;
            Exception = exception;
        }

        public ErrorType ErrorType { get; }
        public string ErrorMessage { get; }
        public Object LogContext { get; }
        public Exception Exception { get; }
        public bool SuppressError { get; }

        public static Error CreateInvalidBindingError(
            string errorMessage,
            BindingNode bindingNode)
        {
            return new Error
            (
                errorType: ErrorType.InvalidBinding,
                errorMessage: $"{SignatureBuilder.GetBindingSignature(bindingNode)} {errorMessage}",
                logContext: bindingNode.ScopeNode.TransformNode.Transform,
                suppressError: false
            );
        }

        public static Error CreateMissingBindingError(FieldNode fieldNode)
        {
            string expectedBindingSignature = SignatureBuilder.GetHypotheticalBindingSignature
            (
                fieldNode.RequestedType,
                fieldNode.IsCollection,
                fieldNode.ComponentNode.TransformNode.NearestScopeNode
            );

            string fieldSignature = SignatureBuilder.GetFieldSignature(fieldNode);

            return new Error
            (
                errorType: ErrorType.MissingBinding,
                errorMessage: $"{expectedBindingSignature} {fieldSignature}",
                logContext: fieldNode.ComponentNode.TransformNode.Transform,
                suppressError: fieldNode.SuppressMissingErrors
            );
        }

        public static Error CreateMissingBindingError(MethodParameterNode parameterNode)
        {
            string expectedBindingSignature = SignatureBuilder.GetHypotheticalBindingSignature
            (
                parameterNode.RequestedType,
                parameterNode.IsCollection,
                parameterNode.MethodNode.ComponentNode.TransformNode.NearestScopeNode
            );

            string methodParameterSignature = SignatureBuilder.GetMethodParameterSignature(parameterNode);

            return new Error
            (
                errorType: ErrorType.MissingBinding,
                errorMessage: $"{expectedBindingSignature} {methodParameterSignature}",
                logContext: parameterNode.MethodNode.ComponentNode.TransformNode.Transform,
                suppressError: parameterNode.MethodNode.SuppressMissingErrors
            );
        }

        public static Error CreateMissingGlobalDependencyError(
            BindingNode bindingNode,
            HashSet<Type> rejectedTypes)
        {
            StringBuilder msg = new();
            msg.Append(SignatureBuilder.GetBindingSignature(bindingNode));

            if (rejectedTypes is { Count: > 0 })
            {
                string typeList = string.Join(", ", rejectedTypes.Select(t => t.Name));
                msg.AppendLine();
                msg.AppendLine($"Candidates rejected due to scene/prefab or prefab/prefab context mismatch: {typeList}.");
                msg.AppendLine("Use ProxyObjects for proper cross-context references, or disable filtering in User Settings (not recommended).");
            }

            return new Error
            (
                errorType: ErrorType.MissingGlobalObject,
                errorMessage: msg.ToString(),
                logContext: bindingNode.ScopeNode.TransformNode.Transform,
                suppressError: false
            );
        }

        public static Error CreateMissingDependencyError(
            BindingNode bindingNode,
            FieldNode fieldNode,
            HashSet<Type> rejectedTypes)
        {
            StringBuilder msg = new();
            msg.Append($"{SignatureBuilder.GetBindingSignature(bindingNode)} {SignatureBuilder.GetFieldSignature(fieldNode)}");

            if (rejectedTypes is { Count: > 0 })
            {
                string typeList = string.Join(", ", rejectedTypes.Select(t => t.Name));
                msg.AppendLine();
                msg.AppendLine($"Candidates rejected due to scene/prefab or prefab/prefab context mismatch: {typeList}.");
                msg.AppendLine("Use ProxyObjects for proper cross-context references, or disable filtering in User Settings (not recommended).");
            }

            return new Error
            (
                errorType: bindingNode.IsCollectionBinding ? ErrorType.MissingDependencies : ErrorType.MissingDependency,
                errorMessage: msg.ToString(),
                logContext: bindingNode.ScopeNode.TransformNode.Transform,
                suppressError: fieldNode.SuppressMissingErrors
            );
        }

        public static Error CreateMissingDependencyError(
            BindingNode bindingNode,
            MethodParameterNode parameterNode,
            HashSet<Type> rejectedTypes)
        {
            StringBuilder msg = new();
            msg.Append($"{SignatureBuilder.GetBindingSignature(bindingNode)} {SignatureBuilder.GetMethodParameterSignature(parameterNode)}");

            if (rejectedTypes is { Count: > 0 })
            {
                string typeList = string.Join(", ", rejectedTypes.Select(t => t.Name));
                msg.AppendLine();
                msg.AppendLine($"Candidates rejected due to scene/prefab or prefab/prefab context mismatch: {typeList}.");
                msg.AppendLine("Use ProxyObjects for proper cross-context references, or disable filtering in User Settings (not recommended).");
            }

            return new Error
            (
                errorType: bindingNode.IsCollectionBinding ? ErrorType.MissingDependencies : ErrorType.MissingDependency,
                errorMessage: msg.ToString(),
                logContext: bindingNode.ScopeNode.TransformNode.Transform,
                suppressError: parameterNode.MethodNode.SuppressMissingErrors
            );
        }

        public static Error CreateMethodInvocationException(
            MethodNode methodNode,
            Exception exception)
        {
            return new Error
            (
                errorType: ErrorType.MethodInvocationException,
                errorMessage: $"{SignatureBuilder.GetMethodSignature(methodNode)}",
                logContext: methodNode.ComponentNode.TransformNode.Transform,
                suppressError: false,
                exception: exception
            );
        }

        public static Error CreateBindingFilterException(
            BindingNode bindingNode,
            Exception exception)
        {
            return new Error
            (
                errorType: ErrorType.BindingFilterException,
                errorMessage: $"{SignatureBuilder.GetBindingSignature(bindingNode)}",
                logContext: bindingNode.ScopeNode.TransformNode.Transform,
                suppressError: false,
                exception: exception
            );
        }
    }
}