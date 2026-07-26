using System.Collections.Generic;
using NUnit.Framework;
using Plugins.Saneject.Editor.Extensions;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;

namespace Tests.Saneject.Editor.Extensions
{
    public class TypeExtensionsTests
    {
        // Regression: generic collection types (e.g. Dictionary<,>, List<>) carry [Serializable] and would
        // otherwise be treated as nested serializable, causing traversal to recurse into their internal
        // fields. A Dictionary property's cached ValueCollection holds a back-reference to the dictionary,
        // producing a reference cycle and an uncatchable StackOverflowException during injection.
        [Test]
        public void IsNestedSerializable_ReturnsFalse_ForGenericCollectionTypes()
        {
            Assert.That(typeof(Dictionary<Vector3, NestedChildTarget>).IsNestedSerializable(), Is.False);
            Assert.That(typeof(List<NestedChildTarget>).IsNestedSerializable(), Is.False);
        }

        // Guard rail: the generic exclusion above must not stop plain [Serializable] user classes from
        // being traversed for nested injection.
        [Test]
        public void IsNestedSerializable_ReturnsTrue_ForPlainSerializableClass()
        {
            Assert.That(typeof(NestedChildTarget).IsNestedSerializable(), Is.True);
        }
    }
}
