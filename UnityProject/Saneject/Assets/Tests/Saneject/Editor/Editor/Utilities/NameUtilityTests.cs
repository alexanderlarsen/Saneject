using NUnit.Framework;
using Plugins.Saneject.Editor.Utilities;

namespace Tests.Saneject.Editor.Editor.Utilities
{
    public class NameUtilityTests
    {
        [Test]
        public void GetLogicalName_GivenBackingField_ReturnsPropertyName()
        {
            // Find logical name
            string logicalName = NameUtility.GetLogicalName("<Dependency>k__BackingField");

            // Assert
            Assert.That(logicalName, Is.EqualTo("Dependency"));
        }

        [Test]
        public void GetLogicalName_GivenNormalField_ReturnsOriginalName()
        {
            // Find logical name
            string logicalName = NameUtility.GetLogicalName("dependency");

            // Assert
            Assert.That(logicalName, Is.EqualTo("dependency"));
        }

        [Test]
        public void GetLogicalName_GivenMalformedBackingField_ReturnsOriginalName()
        {
            // Find logical name
            string logicalName = NameUtility.GetLogicalName("<Dependency>");

            // Assert
            Assert.That(logicalName, Is.EqualTo("<Dependency>"));
        }
    }
}
