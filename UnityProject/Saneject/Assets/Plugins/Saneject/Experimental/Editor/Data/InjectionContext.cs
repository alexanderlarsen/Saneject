using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class InjectionContext
    {
        private readonly HashSet<BindingNode> usedBindings = new();
        private readonly HashSet<BindingNode> unusedBindings = new();
        private readonly HashSet<BindingNode> validBindings = new();
        private readonly Dictionary<FieldNode, object> fieldResolutionMap = new();
        private readonly Dictionary<MethodNode, IReadOnlyList<object>> methodResolutionMap = new();
        private readonly Dictionary<ScopeNode, IReadOnlyList<object>> scopeGlobalResolutionMap = new();
        private readonly List<(string path, Object instance)> createdProxyAssets = new();
        private readonly List<Error> errors = new();
        private readonly Stopwatch stopwatch = new();

        private InjectionContext(IEnumerable<Transform> startTransforms)
        {
            stopwatch.Start();
            Id = Guid.NewGuid().ToString();
            Graph = new InjectionGraph(startTransforms);
        }

        public InjectionGraph Graph { get; }
        public string Id { get; }
        public long ElapsedMilliseconds => stopwatch.ElapsedMilliseconds;

        public IReadOnlyCollection<BindingNode> UsedBindings => usedBindings;
        public IReadOnlyCollection<BindingNode> UnusedBindings => unusedBindings;
        public IReadOnlyCollection<BindingNode> ValidBindings => validBindings;
        public IReadOnlyDictionary<FieldNode, object> FieldResolutionMap => fieldResolutionMap;
        public IReadOnlyDictionary<MethodNode, IReadOnlyList<object>> MethodResolutionMap => methodResolutionMap;
        public IReadOnlyDictionary<ScopeNode, IReadOnlyList<object>> ScopeGlobalResolutionMap => scopeGlobalResolutionMap;
        public IReadOnlyCollection<Error> Errors => errors;
        public IReadOnlyCollection<(string path, Object instance)> CreatedProxyAssets => createdProxyAssets;

        public void StopTimer()
        {
            stopwatch.Stop();
        }

        public void CacheUnusedBindings()
        {
            unusedBindings.Clear();

            foreach (BindingNode bindingNode in Graph
                         .EnumerateAllBindingNodes()
                         .Where(binding => !UsedBindings.Contains(binding)))
                unusedBindings.Add(bindingNode);
        }

        public void RegisterValidBinding(BindingNode bindingNode)
        {
            validBindings.Add(bindingNode);
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
            scopeGlobalResolutionMap[scopeNode] = dependencies.ToList();
        }

        public void RegisterFieldDependency(
            FieldNode fieldNode,
            object dependency)
        {
            fieldResolutionMap[fieldNode] = dependency;
        }

        public void RegisterMethodDependencies(
            MethodNode methodNode,
            IEnumerable<object> dependencies)
        {
            methodResolutionMap[methodNode] = dependencies.ToList();
        }

        public void RegisterCreatedProxyAsset(
            Object instance,
            string path)
        {
            createdProxyAssets.Add((path, instance));
        }

        #region Factory methods

        public static InjectionContext Create(params GameObject[] startObjects)
        {
            return new InjectionContext(startObjects.Select(gameObject => gameObject.transform));
        }

        public static InjectionContext Create(params Transform[] startTransforms)
        {
            return new InjectionContext(startTransforms);
        }

        public static InjectionContext Create(params Component[] startComponents)
        {
            return new InjectionContext(startComponents.Select(component => component.transform));
        }

        #endregion
    }
}