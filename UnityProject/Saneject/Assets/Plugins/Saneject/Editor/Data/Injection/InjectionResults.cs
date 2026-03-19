using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Data.BatchInjection;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Data.Logging;
using UnityEngine;

namespace Plugins.Saneject.Editor.Data.Injection
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class InjectionResults
    {
        private readonly List<Error> errors = new();
        private readonly List<BindingNode> unusedBindingNodes = new();
        private readonly List<(string path, Object instance)> createdProxyAssets = new();

        public InjectionResults()
        {
        }

        public InjectionResults(
            IReadOnlyCollection<Error> errors,
            IReadOnlyCollection<BindingNode> unusedBindingNodes,
            IReadOnlyCollection<(string path, Object instance)> createdProxyAssets,
            int globalRegistrationCount,
            int proxySwapTargetsCount,
            int injectedFieldCount,
            int injectedPropertyCount,
            int injectedMethodCount,
            int scopesProcessedCount)
        {
            this.errors.AddRange(errors);
            this.unusedBindingNodes.AddRange(unusedBindingNodes);
            this.createdProxyAssets.AddRange(createdProxyAssets);
            GlobalRegistrationCount = globalRegistrationCount;
            ProxySwapTargetsCount = proxySwapTargetsCount;
            InjectedFieldCount = injectedFieldCount;
            InjectedPropertyCount = injectedPropertyCount;
            InjectedMethodCount = injectedMethodCount;
            ScopesProcessedCount = scopesProcessedCount;
        }

        public IReadOnlyCollection<Error> Errors => errors;
        public IReadOnlyCollection<BindingNode> UnusedBindingNodes => unusedBindingNodes;
        public IReadOnlyCollection<(string path, Object instance)> CreatedProxyAssets => createdProxyAssets;

        public int GlobalRegistrationCount { get; private set; }
        public int ProxySwapTargetsCount { get; private set; }
        public int InjectedFieldCount { get; private set; }
        public int InjectedPropertyCount { get; private set; }
        public int InjectedMethodCount { get; private set; }
        public int ScopesProcessedCount { get; private set; }

        public void AddToResults(InjectionResults results)
        {
            errors.AddRange(results.Errors);
            unusedBindingNodes.AddRange(results.UnusedBindingNodes);
            createdProxyAssets.AddRange(results.CreatedProxyAssets);
            GlobalRegistrationCount += results.GlobalRegistrationCount;
            ProxySwapTargetsCount += results.ProxySwapTargetsCount;
            InjectedFieldCount += results.InjectedFieldCount;
            InjectedPropertyCount += results.InjectedPropertyCount;
            InjectedMethodCount += results.InjectedMethodCount;
            ScopesProcessedCount += results.ScopesProcessedCount;
        }

        public InjectionStatus GetStatus()
        {
            if (errors.Count(error => !error.SuppressError) > 0)
                return InjectionStatus.Error;

            return unusedBindingNodes.Count > 0
                ? InjectionStatus.Warning
                : InjectionStatus.Success;
        }
    }
} 