# SerializeInterface

## Why Unity can't "serialize an interface"

Unity's serializer only supports a restricted set of types:

- Primitive value types (`int`, `float`, `bool`, etc.)
- Enums and strings
- Arrays and `List<T>` where `T` is serializable
- Structs marked `[Serializable]`
- References to `UnityEngine.Object` subclasses (`Component`, `ScriptableObject`, etc.)

Interfaces don't fit into any of these categories. They're just contracts, not concrete data or Unity objects, so Unity has nothing it can actually write. As a result, fields typed as an interface are skipped entirely by the serializer.

## What the Saneject Roslyn generator adds

For every `[SerializeInterface]` field, the generator emits a hidden, serializable `UnityEngine.Object` backing field in a partial class that implements `ISerializationCallbackReceiver`.

At serialization time (Editor only), the interface reference is synced into this backing field so it shows up in the Inspector and persists in the scene/prefab.

At deserialization time, the backing field is copied back into the real interface field so the component can use it normally in code.

```csharp
// User written class. 
// Needs to be partial for source generator to extend it with another partial.
public partial class Requester : MonoBehaviour
{
    [Inject, SerializeInterface]
    private IService service;

    [Inject, SerializeInterface]
    private IService[] servicesArray;
}
```

```csharp
// Auto-generated partial.
public partial class Requester : ISerializationCallbackReceiver
{
    [SerializeField, InterfaceBackingField(interfaceType: typeof(IService), isInjected: true, injectId: null)]
    private Object __service; // Real serialized field
    
    [SerializeField, InterfaceBackingField(interfaceType: typeof(IService), isInjected: true, injectId: null)]
    private Object[] __servicesArray; // Real serialized field

    public void OnBeforeSerialize()
    {
        // Sync interface fields into their Object-backed fields so they serialize and show up in the Inspector.
        #if UNITY_EDITOR
        __service = service as Object;
        __servicesArray = servicesArray?.Cast<UnityEngine.Object>().ToArray();
        #endif
    }
    
    public void OnAfterDeserialize()
    {
        // When Unity deserialization occurs, the Object is assigned to the actual interface field.
        service = __service as IService;
        
        servicesArray = (__servicesArray ?? Array.Empty<UnityEngine.Object>())
                    .Select(x => x as IService)
                    .ToArray();
    }
}
```

The generated backing fields make interface members appear in the Inspector like any other serialized reference.

With `[Inject, SerializeInterface]`, fields are grayed out and auto-managed by Saneject.

Without `[Inject]`, they behave as regular `Object` fields you can assign manually. If you assign an object that doesn't implement the required interface, the field is cleared to `null`.

![Interface field visible in the Inspector](../images/serialize-interface-inspector-example.webp)