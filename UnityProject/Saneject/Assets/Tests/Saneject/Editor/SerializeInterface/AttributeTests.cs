using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Runtime.Attributes;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.SerializeInterface
{
    public class AttributeTests
    {
        [Test]
        public void BackingField_GivenSingleInterfaceField_GeneratesObjectBackingField()
        {
            // Find backing field
            FieldInfo backingField = typeof(SingleInterfaceTarget).GetField("__dependency", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            Assert.That(backingField, Is.Not.Null);
            Assert.That(backingField.FieldType, Is.EqualTo(typeof(Object)));
        }

        [Test]
        public void BackingField_GivenInterfaceArrayField_GeneratesObjectArrayBackingField()
        {
            // Find backing field
            FieldInfo backingField = typeof(MultiInterfaceTarget).GetField("__array", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            Assert.That(backingField, Is.Not.Null);
            Assert.That(backingField.FieldType, Is.EqualTo(typeof(Object[])));
        }

        [Test]
        public void BackingField_GivenInterfaceListField_GeneratesObjectListBackingField()
        {
            // Find backing field
            FieldInfo backingField = typeof(MultiInterfaceTarget).GetField("__list", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            Assert.That(backingField, Is.Not.Null);
            Assert.That(backingField.FieldType, Is.EqualTo(typeof(List<Object>)));
        }

        [Test]
        public void BackingField_GivenSingleInterfaceProperty_GeneratesObjectBackingField()
        {
            // Find backing field
            FieldInfo backingField = typeof(SingleInterfacePropertyTarget).GetField("__Dependency", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            Assert.That(backingField, Is.Not.Null);
            Assert.That(backingField.FieldType, Is.EqualTo(typeof(Object)));
        }

        [Test]
        public void BackingField_GivenInterfaceArrayProperty_GeneratesObjectArrayBackingField()
        {
            // Find backing field
            FieldInfo backingField = typeof(MultiInterfacePropertyTarget).GetField("__Array", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            Assert.That(backingField, Is.Not.Null);
            Assert.That(backingField.FieldType, Is.EqualTo(typeof(Object[])));
        }

        [Test]
        public void BackingField_GivenInterfaceListProperty_GeneratesObjectListBackingField()
        {
            // Find backing field
            FieldInfo backingField = typeof(MultiInterfacePropertyTarget).GetField("__List", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            Assert.That(backingField, Is.Not.Null);
            Assert.That(backingField.FieldType, Is.EqualTo(typeof(List<Object>)));
        }

        [Test]
        public void BackingField_GivenInterfaceField_MirrorsSupportedAttributes()
        {
            // Find backing field and attributes
            FieldInfo backingField = typeof(SingleInterfaceTargetWithAttributes).GetField("__dependency", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(backingField, Is.Not.Null);

            object[] attributes = backingField.GetCustomAttributes(inherit: false);
            HeaderAttribute header = attributes.OfType<HeaderAttribute>().SingleOrDefault();
            TooltipAttribute tooltip = attributes.OfType<TooltipAttribute>().SingleOrDefault();
            SerializeField serializeField = attributes.OfType<SerializeField>().SingleOrDefault();
            HideInInspector hideInInspector = attributes.OfType<HideInInspector>().SingleOrDefault();
            EditorBrowsableAttribute editorBrowsable = attributes.OfType<EditorBrowsableAttribute>().SingleOrDefault();
            InjectAttribute inject = attributes.OfType<InjectAttribute>().SingleOrDefault();
            SerializeInterfaceAttribute serializeInterface = attributes.OfType<SerializeInterfaceAttribute>().SingleOrDefault();

            // Assert
            Assert.That(header, Is.Not.Null);
            Assert.That(header.header, Is.EqualTo("Field Header"));
            Assert.That(tooltip, Is.Not.Null);
            Assert.That(tooltip.tooltip, Is.EqualTo("Field Tooltip"));
            Assert.That(serializeField, Is.Not.Null);
            Assert.That(hideInInspector, Is.Not.Null);
            Assert.That(editorBrowsable, Is.Not.Null);
            Assert.That(editorBrowsable.State, Is.EqualTo(EditorBrowsableState.Never));
            Assert.That(inject, Is.Null);
            Assert.That(serializeInterface, Is.Null);
        }

        [Test]
        public void BackingField_GivenInterfaceProperty_MirrorsSupportedAttributes()
        {
            // Find backing field and attributes
            FieldInfo backingField = typeof(SingleInterfacePropertyTargetWithAttributes).GetField("__Dependency", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(backingField, Is.Not.Null);

            object[] attributes = backingField.GetCustomAttributes(inherit: false);
            HeaderAttribute header = attributes.OfType<HeaderAttribute>().SingleOrDefault();
            TooltipAttribute tooltip = attributes.OfType<TooltipAttribute>().SingleOrDefault();
            SerializeField serializeField = attributes.OfType<SerializeField>().SingleOrDefault();
            HideInInspector hideInInspector = attributes.OfType<HideInInspector>().SingleOrDefault();
            EditorBrowsableAttribute editorBrowsable = attributes.OfType<EditorBrowsableAttribute>().SingleOrDefault();
            InjectAttribute inject = attributes.OfType<InjectAttribute>().SingleOrDefault();
            SerializeInterfaceAttribute serializeInterface = attributes.OfType<SerializeInterfaceAttribute>().SingleOrDefault();

            // Assert
            Assert.That(header, Is.Not.Null);
            Assert.That(header.header, Is.EqualTo("Property Header"));
            Assert.That(tooltip, Is.Not.Null);
            Assert.That(tooltip.tooltip, Is.EqualTo("Property Tooltip"));
            Assert.That(serializeField, Is.Not.Null);
            Assert.That(hideInInspector, Is.Not.Null);
            Assert.That(editorBrowsable, Is.Not.Null);
            Assert.That(editorBrowsable.State, Is.EqualTo(EditorBrowsableState.Never));
            Assert.That(inject, Is.Null);
            Assert.That(serializeInterface, Is.Null);
        }
    }
}