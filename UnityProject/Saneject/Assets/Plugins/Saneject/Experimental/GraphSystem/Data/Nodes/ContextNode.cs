using System;
using System.Collections.Generic;

namespace Plugins.Saneject.Experimental.GraphSystem.Data.Nodes
{
    [Serializable]
    public class ContextNode : IEqualityComparer<ContextNode>
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public bool Equals(
            ContextNode x,
            ContextNode y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x is null)
                return false;

            if (y is null)
                return false;

            if (x.GetType() != y.GetType())
                return false;

            return x.Id == y.Id;
        }

        public int GetHashCode(ContextNode obj)
        {
            return HashCode.Combine(obj.Id);
        }
    }
}