using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Object = UnityEngine.Object;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class InjectionContext
    {
        private readonly HashSet<BindingNode> usedBindings = new();
        private readonly HashSet<BindingNode> unusedBindingNodes = new();
        private readonly HashSet<BindingNode> validBindingNodes = new();
        private readonly Dictionary<FieldNode, object> fieldNodeResolutionMap = new();
        private readonly Dictionary<MethodNode, IReadOnlyList<object>> methodNodeResolutionMap = new();
        private readonly Dictionary<ScopeNode, IReadOnlyList<object>> scopeNodeGlobalResolutionMap = new();
        private readonly List<(string path, Object instance)> createdProxyAssets = new();
        private readonly List<Error> errors = new();

        public InjectionContext(IReadOnlyCollection<TransformNode> activeTransformNodes)
        {
            ActiveTransformNodes = activeTransformNodes;

            ActiveComponentNodes = ActiveTransformNodes
                .EnumerateAllComponentNodes()
                .ToList();

            ActiveScopeNodes = ActiveTransformNodes
                .EnumerateAllScopeNodes()
                .ToList();

            ActiveBindingNodes = ActiveScopeNodes
                .EnumerateAllBindingNodes()
                .ToList();
        }

        public IReadOnlyCollection<TransformNode> ActiveTransformNodes { get; }
        public IReadOnlyCollection<ComponentNode> ActiveComponentNodes { get; }
        public IReadOnlyCollection<ScopeNode> ActiveScopeNodes { get; }
        public IReadOnlyCollection<BindingNode> ActiveBindingNodes { get; }
        public IReadOnlyCollection<BindingNode> ValidBindingNodes => validBindingNodes;
        public IReadOnlyCollection<BindingNode> UsedBindings => usedBindings;
        public IReadOnlyCollection<BindingNode> UnusedBindingNodes => unusedBindingNodes;
        public IReadOnlyDictionary<FieldNode, object> FieldNodeResolutionMap => fieldNodeResolutionMap;
        public IReadOnlyDictionary<MethodNode, IReadOnlyList<object>> MethodNodeResolutionMap => methodNodeResolutionMap;
        public IReadOnlyDictionary<ScopeNode, IReadOnlyList<object>> ScopeNodeGlobalResolutionMap => scopeNodeGlobalResolutionMap;
        public IReadOnlyCollection<Error> Errors => errors;
        public IReadOnlyCollection<(string path, Object instance)> CreatedProxyAssets => createdProxyAssets;

        public void RegisterValidBinding(BindingNode bindingNode)
        {
            validBindingNodes.Add(bindingNode);
        }

        public void RegisterUsedBinding(BindingNode bindingNode)
        {
            usedBindings.Add(bindingNode);
        }

        public void RegisterError(Error error)
        {
            errors.Add(error);
        }

        public void RegisterErrors(IEnumerable<Error> errors)
        {
            this.errors.AddRange(errors);
        }

        public void RegisterGlobalDependencies(
            ScopeNode scopeNode,
            IEnumerable<object> dependencies)
        {
            scopeNodeGlobalResolutionMap[scopeNode] = dependencies.ToList();
        }

        public void RegisterFieldDependency(
            FieldNode fieldNode,
            object dependency)
        {
            fieldNodeResolutionMap[fieldNode] = dependency;
        }

        public void RegisterMethodDependencies(
            MethodNode methodNode,
            IEnumerable<object> dependencies)
        {
            methodNodeResolutionMap[methodNode] = dependencies.ToList();
        }

        public void RegisterCreatedProxyAsset(
            Object instance,
            string path)
        {
            createdProxyAssets.Add((path, instance));
        }

        public InjectionResults GetResults()
        {
            unusedBindingNodes.Clear();

            foreach (BindingNode bindingNode in ActiveBindingNodes)
                if (!usedBindings.Contains(bindingNode))
                    unusedBindingNodes.Add(bindingNode);

            return new InjectionResults
            (
                errors: errors,
                unusedBindingNodes: unusedBindingNodes,
                createdProxyAssets: createdProxyAssets,
                globalRegistrationCount: scopeNodeGlobalResolutionMap.Count,
                injectedFieldCount: fieldNodeResolutionMap
                    .Keys
                    .Count(field => !field.IsPropertyBackingField),
                injectedPropertyCount: fieldNodeResolutionMap
                    .Keys
                    .Count(field => field.IsPropertyBackingField),
                injectedMethodCount: methodNodeResolutionMap.Count,
                scopesProcessedCount: ActiveScopeNodes.Count
            );
        }
    }
}