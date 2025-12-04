using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Runtime;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;
using ComponentRequesterWithID = Tests.Runtime.Component.ComponentRequesterWithID;

namespace Tests.Editor.SerializeInterface
{
    public class AttributeTests
    {
        // TODO: Update these tests to reflect new InterfaceBackingFieldAttribute
        
        [Test]
        public void BackingField_HasNullInjectId()
        {
            AssertBackingField(
                typeof(ComponentRequester),
                "__interfaceComponent",
                expectedType: typeof(IInjectable));
        }

        [Test]
        public void BackingField_HasInjectId_ComponentA()
        {
            AssertBackingField(
                typeof(ComponentRequesterWithID),
                "__interfaceComponentA",
                expectedType: typeof(IInjectable));
        }

        [Test]
        public void BackingField_HasInjectId_ComponentB()
        {
            AssertBackingField(
                typeof(ComponentRequesterWithID),
                "__interfaceComponentB",
                expectedType: typeof(IInjectable));
        }

        private void AssertBackingField(
            Type targetType,
            string backingFieldName,
            Type expectedType)
        {
            FieldInfo field = targetType.GetField(
                backingFieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.NotNull(field, $"Expected backing field '{backingFieldName}' on {targetType.Name}.");

            InterfaceBackingFieldAttribute attr = field.GetCustomAttributes(typeof(InterfaceBackingFieldAttribute), false)
                .FirstOrDefault() as InterfaceBackingFieldAttribute;

            Assert.NotNull(attr, $"Expected InterfaceBackingFieldAttribute on field '{backingFieldName}'.");
            Assert.AreEqual(expectedType, attr.InterfaceType, "Interface type mismatch.");
        }
    }
}