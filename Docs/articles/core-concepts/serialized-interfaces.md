# Serialized interfaces

## Why Unity can't serialize an interface

Unity's serializer supports a restricted set of types: primitive value types, enums, strings, arrays and `List<T>` of serializable types, `[Serializable]` structs, and references to `UnityEngine.Object` subclasses.

Interfaces don't fit any of these categories. They're contracts, not concrete data or Unity objects, so the serializer has nothing to write. Interface-typed fields are skipped entirely.

## The `[SerializeInterface]` attribute

Marking an interface field with `[SerializeInterface]` tells the Roslyn source generator to emit a hidden `UnityEngine.Object` backing field in a matching partial class. At serialization time (Editor only), the interface reference is synced into the backing field so it appears in the Inspector and persists in the scene or prefab. At deserialization time, the backing field is copied back into the interface field for use in code.

Because the generator emits code into a partial class, your class must be declared `partial`:

```csharp
// User-written class — must be partial
public partial class Enemy : MonoBehaviour
{
    [Inject, SerializeInterface]
    private IAudioService audioService;

    [Inject, SerializeInterface]
    private IEnemyConfig[] configs; // Arrays of interfaces are supported
}
```

The generated partial contains the backing fields and `ISerializationCallbackReceiver` implementation:

```csharp
// Auto-generated partial
public partial class Enemy : ISerializationCallbackReceiver
{
    [SerializeField, InterfaceBackingField(...)]
    private Object __audioService;

    [SerializeField, InterfaceBackingField(...)]
    private Object[] __configs;

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        __audioService = audioService as Object;
        __configs = configs?.Cast<Object>().ToArray();
#endif
    }

    public void OnAfterDeserialize()
    {
        audioService = __audioService as IAudioService;
        configs = (__configs ?? Array.Empty<Object>())
            .Select(x => x as IEnemyConfig)
            .ToArray();
    }
}
```

## With vs without `[Inject]`

| Usage | Behaviour |
|---|---|
| `[Inject, SerializeInterface]` | Field is injected and managed by Saneject. Shown grayed out in the Inspector. |
| `[SerializeInterface]` (no `[Inject]`) | Field appears in the Inspector as a manual drag-and-drop slot. If you assign an object that doesn't implement the required interface, the assignment is cleared to `null`. |

## Inspector appearance

The backing field is drawn next to its corresponding interface field — not at the bottom of the Inspector where auto-generated fields normally appear. This is handled by Saneject's `MonoBehaviourInspector`. See [Custom inspector](../editor-and-tooling/custom-inspector.md) if you need to integrate this with your own editor class.


