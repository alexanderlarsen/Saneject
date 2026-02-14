using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Data.Logging;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Object = UnityEngine.Object;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.Data.Injection
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
        private readonly List<ComponentNode> proxySwapTargetNodes = new();

        public InjectionContext(
            IReadOnlyCollection<TransformNode> activeTransformNodes,
            InjectionProgressTracker progressTracker)
        {
            progressTracker.BeginSegment(stepCount: 4);
            progressTracker.UpdateInfoText("Creating injection context: Finding transforms");

            ActiveTransformNodes = activeTransformNodes;

            progressTracker.NextStep();
            progressTracker.UpdateInfoText("Creating injection context: Filtering transforms");

            ActiveComponentNodes = ActiveTransformNodes
                .EnumerateAllComponentNodes()
                .ToList();

            progressTracker.NextStep();
            progressTracker.UpdateInfoText("Creating injection context: Filtering scopes");

            ActiveScopeNodes = ActiveTransformNodes
                .EnumerateAllScopeNodes()
                .ToList();

            progressTracker.NextStep();
            progressTracker.UpdateInfoText("Creating injection context: Filtering bindings");

            ActiveBindingNodes = ActiveScopeNodes
                .EnumerateAllBindingNodes()
                .ToList();

            progressTracker.NextStep();
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
        
        public void RegisterProxySwapTarget(ComponentNode componentNode)
        {
            proxySwapTargetNodes.Add(componentNode);
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
                unusedBindingNodes: unusedBindingNodes.Where(b => validBindingNodes.Contains(b)).ToList().AsReadOnly(),
                createdProxyAssets: createdProxyAssets,
                globalRegistrationCount: scopeNodeGlobalResolutionMap.Count,
                proxySwapTargetsCount: proxySwapTargetNodes.Count,
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