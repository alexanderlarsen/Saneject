using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class InjectionSession
    {
        private readonly Dictionary<FieldNode, IEnumerable<Object>> fieldResolutionMap = new();
        private readonly HashSet<BindingNode> usedBindings = new();
        private readonly HashSet<BindingNode> validBindings = new();
        private readonly List<(string path, Object instance)> createdProxyAssets = new();
        private readonly List<Error> errors = new();
        private readonly Stopwatch stopwatch = new();
        private readonly Dictionary<ScopeNode, List<Object>> globalResolutionMap = new();

        private InjectionSession(IEnumerable<Transform> startTransforms)
        {
            stopwatch.Start();
            IsSessionActive = true;
            Id = Guid.NewGuid().ToString();
            Graph = new InjectionGraph(startTransforms);
        }

        public string Id { get; }
        public InjectionGraph Graph { get; }

        public IReadOnlyCollection<BindingNode> UsedBindings => usedBindings;
        public IReadOnlyCollection<BindingNode> ValidBindings => validBindings;
        public IReadOnlyCollection<Error> Errors => errors;
        public IReadOnlyDictionary<FieldNode, IEnumerable<Object>> FieldResolutionMap => fieldResolutionMap;
        public IReadOnlyCollection<(string path, Object instance)> CreatedProxyAssets => createdProxyAssets;

        public IReadOnlyDictionary<ScopeNode, IReadOnlyList<Object>> GlobalResolutionMap => globalResolutionMap.ToDictionary
        (
            kvp => kvp.Key,
            kvp => (IReadOnlyList<Object>)kvp.Value
        );

        public long DurationMilliseconds => stopwatch.ElapsedMilliseconds;
        public bool IsSessionActive { get; private set; }

        public void EndSession()
        {
            stopwatch.Stop();
            IsSessionActive = false;
        }

        public void RegisterGlobalDependency(
            ScopeNode scopeNode,
            Object globalObject)
        {
            EnsureActiveSession();

            if (!globalResolutionMap.ContainsKey(scopeNode))
                globalResolutionMap[scopeNode] = new List<Object>();

            globalResolutionMap[scopeNode].Add(globalObject);
        }

        public void RegisterValidBinding(BindingNode bindingNode)
        {
            EnsureActiveSession();
            validBindings.Add(bindingNode);
        }

        public void RegisterUsedBinding(BindingNode bindingNode)
        {
            EnsureActiveSession();
            usedBindings.Add(bindingNode);
        }

        public void RegisterError(Error error)
        {
            EnsureActiveSession();
            errors.Add(error);
        }

        public void RegisterErrors(IEnumerable<Error> errors)
        {
            EnsureActiveSession();
            this.errors.AddRange(errors);
        }

        public void RegisterFieldDependencies(
            FieldNode fieldNode,
            IEnumerable<Object> dependencies)
        {
            EnsureActiveSession();
            fieldResolutionMap[fieldNode] = dependencies;
        }

        public void RegisterCreatedProxyAsset(
            Object instance,
            string path)
        {
            EnsureActiveSession();
            createdProxyAssets.Add((path, instance));
        }

        private void EnsureActiveSession()
        {
            if (!IsSessionActive)
                throw new InvalidOperationException("Saneject: The session is immutable after it has been stopped and cannot be modified anymore.");
        }

        #region Factory methods

        public static InjectionSession StartSession(params GameObject[] startObjects)
        {
            return new InjectionSession(startObjects.Select(gameObject => gameObject.transform));
        }

        public static InjectionSession StartSession(params Transform[] startTransforms)
        {
            return new InjectionSession(startTransforms);
        }

        public static InjectionSession StartSession(params Component[] startComponents)
        {
            return new InjectionSession(startComponents.Select(component => component.transform));
        }

        #endregion
    }
}