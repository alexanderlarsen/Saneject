using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Runtime;

namespace Tests.Editor.SerializeInterface
{
    public class AttributeTests
    {
        [Test]
        public void BackingField_HasNullInjectId()
        {
            AssertBackingField(
                typeof(ComponentRequester),
                "__interfaceComponent",
                expectedType: typeof(IInjectable),
                expectedInjectId: null,
                expectedIsInjected: true);
        }

        [Test]
        public void BackingField_HasInjectId_ComponentA()
        {
            AssertBackingField(
                typeof(ComponentRequesterWithID),
                "__interfaceComponentA",
                expectedType: typeof(IInjectable),
                expectedInjectId: "componentA",
                expectedIsInjected: true);
        }

        [Test]
        public void BackingField_HasInjectId_ComponentB()
        {
            AssertBackingField(
                typeof(ComponentRequesterWithID),
                "__interfaceComponentB",
                expectedType: typeof(IInjectable),
                expectedInjectId: "componentB",
                expectedIsInjected: true);
        }

        private void AssertBackingField(
            Type targetType,
            string backingFieldName,
            Type expectedType,
            string expectedInjectId,
            bool expectedIsInjected)
        {
            FieldInfo field = targetType.GetField(
                backingFieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.NotNull(field, $"Expected backing field '{backingFieldName}' on {targetType.Name}.");

            InterfaceBackingFieldAttribute attr = field.GetCustomAttributes(typeof(InterfaceBackingFieldAttribute), false)
                .FirstOrDefault() as InterfaceBackingFieldAttribute;

            Assert.NotNull(attr, $"Expected InterfaceBackingFieldAttribute on field '{backingFieldName}'.");
            Assert.AreEqual(expectedType, attr.InterfaceType, "Interface type mismatch.");
            Assert.AreEqual(expectedIsInjected, attr.IsInjected, "IsInjected mismatch.");
            Assert.AreEqual(expectedInjectId, attr.InjectId, "InjectId mismatch.");
        }
    }
}