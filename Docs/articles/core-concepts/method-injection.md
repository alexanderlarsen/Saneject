# Method injection

Saneject supports `[Inject]` on methods as an alternative or complement to field injection. Method injection runs after all `[Inject]` fields are assigned.

This is useful when you want to:

- Perform initialization logic using multiple resolved dependencies at once.
- Inject into third-party components where you can't add serialized fields to the source.
- Configure or cache dependencies according to custom logic before storing them in fields.
- Run any editor-time behavior that should happen after dependency resolution.

## Rules

- The method must be an instance method (not static).
- Access level can be `public`, `protected`, or `private`.
- All parameters must be resolvable by Saneject. If any parameter cannot be resolved, the method is not called.
- `List<T>` and `T[]` parameters are supported.
- Interface parameters resolve from interface bindings.
- `[Inject]` methods inside nested `[Serializable]` classes are supported.
- Methods are called in declaration order.

## Qualifiers

Method injection supports the same qualifiers as field injection. The method name is used as the member name for `ToMember` matching:

```csharp
// Binding side — targets the method named "InjectAudio" on Player specifically
BindComponent<IAudioService, AudioService>()
    .FromAnywhere()
    .ToTarget<Player>()
    .ToMember("InjectAudio");
```

## Example

```csharp
public partial class Player : MonoBehaviour
{
    // Field injection runs first
    [Inject, SerializeField]
    private CharacterController controller;

    // Method injection runs after all fields are assigned
    [Inject]
    private void Inject(PlayerConfig config, IAudioService audio)
    {
        // Called automatically after field injection
    }

    // Collections are supported
    [Inject]
    private void InjectEnemies(IEnemy[] enemies)
    {
        // Receives all bound IEnemy instances
    }
}

// Nested serializable classes are also supported
[Serializable]
public class PlayerStats
{
    [Inject]
    public void Inject(StatsConfig config)
    {
        // Also called during injection
    }
}
```
