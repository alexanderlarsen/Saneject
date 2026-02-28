# Method injection

Saneject supports `[Inject]` on methods as an alternative or complement to field injection. Method injection also supports qualifiers `ToTarget<T>()`, `ToMember(string)`, `ToId(string)`, with member name being the method name.

This is useful when you want to:

- Configure components based on injected data (e.g. calculations using a config `ScriptableObject`).
- Inject into third-party classes where you can't modify the source but they expose public serialized fields or properties.
- Configure external systems (e.g. editor tools, asset processors, analytics) using resolved dependencies.
- Apply complex initialization logic where simple field/property injection isn't enough.
- Configure or cache dependencies internally according to custom logic before storing them in fields.
- …and generally, any editor-time behavior that should happen after dependency resolution.

Method injection runs after all `[Inject]` fields are assigned.

## Rules

- The method must be an instance method (not static).
- Access level can be `public`, `protected`, or `private`.
- All parameters must be resolvable by Saneject bindings:
    - Each parameter type follows the same resolution rules as fields.
    - Arrays and `List<T>` parameters are supported.
    - Interface parameters resolve from interface bindings.
    - Proxy bindings work the same as for fields.
- If any parameter cannot be resolved, the method is not invoked.
- `[Inject]` methods inside nested `[Serializable]` classes are also supported.
- Methods are invoked in declaration order (reflection order in C#).

## Example

```csharp
public partial class RootClass : MonoBehaviour
{
    [SerializeField]
    private NestedClass nestedClass;
    
    [Inject]
    private void Inject(Dependency dependency, IDependency iface)
    {
        // Called automatically after field injection
    }

    [Inject]
    private void InjectCollection(Dependency[] dependencies)
    {
        // Arrays and lists are supported
    }
}

[Serializable]
public class NestedClass
{
    [Inject]
    public void Inject(MyDependency[] dependencies)
    {
        // Nested serializable classes are supported too
    }
}
```