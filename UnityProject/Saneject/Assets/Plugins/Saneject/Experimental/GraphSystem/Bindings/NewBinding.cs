using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.GraphSystem.Bindings
{
    public abstract class NewBinding
    {
        private readonly List<object> directInstancesToResolveFrom = new();
        private readonly List<Func<Object, bool>> filters = new();
        private readonly List<string> idQualifiers = new();
        private readonly List<string> injectionTargetMemberNameQualifiers = new();
        private readonly List<Type> injectionTargetTypeQualifiers = new();

        public Type InterfaceType { get; private set; }
        public Type ConcreteType { get; private set; }
        public bool IsCollectionBinding { get; private set; }

        public IReadOnlyList<object> DirectInstancesToResolveFrom => directInstancesToResolveFrom;
        public IReadOnlyList<string> IdQualifiers => idQualifiers;
        public IReadOnlyList<Type> InjectionTargetTypeQualifiers => injectionTargetTypeQualifiers;
        public IReadOnlyList<string> InjectionTargetMemberNameQualifiers => injectionTargetMemberNameQualifiers;
        public IReadOnlyList<Func<Object, bool>> Filters => filters;

        public void SetTargetType(Type targetType)
        {
            if (targetType.IsInterface)
                InterfaceType = targetType;
            else
                ConcreteType = targetType;
        }

        public void SetTargetTypes(
            Type interfaceType,
            Type concreteType)
        {
            InterfaceType = interfaceType;
            ConcreteType = concreteType;
        }

        public void ResolveFromInstances(params object[] instances)
        {
            directInstancesToResolveFrom.AddRange(instances);
        }

        public void AddIdQualifier(string id)
        {
            idQualifiers.Add(id);
        }

        public void AddInjectionTargetTypeQualifier(Type type)
        {
            injectionTargetTypeQualifiers.Add(type);
        }

        public void AddInjectionTargetMemberNameQualifier(string memberName)
        {
            injectionTargetMemberNameQualifiers.Add(memberName);
        }

        /// <summary>
        /// Add a filter for candidate <see cref="UnityEngine.Object" />s. Only objects passing all filters will be considered for resolution.
        /// </summary>
        /// <param name="filter">Predicate to evaluate each <see cref="UnityEngine.Object" />.</param>
        public void AddFilter(Func<Object, bool> filter)
        {
            filters.Add(filter);
        }

        public void MarkCollectionBinding()
        {
            IsCollectionBinding = true;
        }
    }
}