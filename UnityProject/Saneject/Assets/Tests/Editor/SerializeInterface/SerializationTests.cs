using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.SerializeInterface
{
    public class SerializationTests : BaseTest
    {
        private GameObject root;
        private ComponentRequester requester;
        private InjectableComponent component;

        [Test]
        public void OnBeforeSerialize_SetsBackingObject()
        {
            // Add components
            component = root.AddComponent<InjectableComponent>();
            requester = root.AddComponent<ComponentRequester>();
            requester.interfaceComponent = component;

            // Simulate serialization
            requester.OnBeforeSerialize();

            // Assert backing field holds serialized reference
            Object backingField = GetBackingObjectField(requester);
            Assert.AreEqual(component, backingField, "Expected backing Object field to hold serialized value.");
        }

        [Test]
        public void OnAfterDeserialize_RestoresInterfaceFromBackingObject()
        {
            // Add components
            component = root.AddComponent<InjectableComponent>();
            requester = root.AddComponent<ComponentRequester>();
            requester.interfaceComponent = component;
            requester.OnBeforeSerialize();

            // Null out field to simulate deserialization
            requester.interfaceComponent = null;

            // Simulate deserialization
            requester.OnAfterDeserialize();

            // Assert deserialized reference is restored
            Assert.AreEqual(component, requester.interfaceComponent, "Expected interfaceComponent to be restored from serialized Object.");
        }

        [Test]
        public void Class_Implements_ISerializationCallbackReceiver()
        {
            bool implements = typeof(ComponentRequester).GetInterfaces()
                .Contains(typeof(ISerializationCallbackReceiver));

            Assert.IsTrue(implements, "ComponentRequester must implement ISerializationCallbackReceiver to support SerializeInterface generation.");
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }

        private Object GetBackingObjectField(ComponentRequester requester)
        {
            FieldInfo field = typeof(ComponentRequester).GetField(
                "__interfaceComponent",
                BindingFlags.NonPublic | BindingFlags.Instance);

            return (Object)field?.GetValue(requester);
        }
    }
}