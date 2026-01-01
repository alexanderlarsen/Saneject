using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class InjectionSession
    {
        private readonly HashSet<BindingNode> usedBindings = new();
        private readonly HashSet<BindingNode> validBindings = new();
        private readonly List<Error> errors = new();
        private readonly Stopwatch stopwatch = new();

        private InjectionSession(IEnumerable<Transform> startTransforms)
        {
            stopwatch.Start();
            Id = Guid.NewGuid().ToString();
            Graph = new InjectionGraph(startTransforms);
        }

        public string Id { get; }
        public InjectionGraph Graph { get; }

        public IReadOnlyCollection<BindingNode> UsedBindings => usedBindings;
        public IReadOnlyCollection<BindingNode> ValidBindings => validBindings;
        public IReadOnlyCollection<Error> Errors => errors;

        public long InjectionDurationMilliseconds => stopwatch.ElapsedMilliseconds;

        public void StopTimer()
        {
            stopwatch.Stop();
        }

        public void MarkBindingValid(BindingNode bindingNode)
        {
            validBindings.Add(bindingNode);
        }

        public void MarkBindingUsed(BindingNode bindingNode)
        {
            usedBindings.Add(bindingNode);
        }

        public void AddError(Error error)
        {
            errors.Add(error);
        }

        public void AddErrors(IEnumerable<Error> errors)
        {
            this.errors.AddRange(errors);
        }

        #region Factory methods

        public static InjectionSession Create(params GameObject[] startObjects)
        {
            return new InjectionSession(startObjects.Select(gameObject => gameObject.transform));
        }

        public static InjectionSession Create(params Transform[] startTransforms)
        {
            return new InjectionSession(startTransforms);
        }

        public static InjectionSession Create(params Component[] startComponents)
        {
            return new InjectionSession(startComponents.Select(component => component.transform));
        }

        #endregion
    }
}