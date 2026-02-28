# Custom inspector

Unity’s default inspector draws fields in declaration order, but Roslyn generated interface backing fields live in a partial class, which normally causes them to appear at the bottom of the Inspector. This breaks expected grouping and makes injected interfaces harder to interpret.

`MonoBehaviourFallbackInspector` is a global fallback custom `Editor` for all `MonoBehaviour`s.  
It is only used when no more specific inspector exists for the target type, ensuring Saneject's intended UI/UX is always applied by default.

Instead of custom per field drawing, the fallback inspector delegates entirely to `SanejectInspector.DrawDefault`, which uses a structured, data driven pipeline that closely mirrors Unity’s native inspector behavior.

Specifically, it:

- Draws the non-editable script field at the top of the inspector.
- Discovers all serializable fields and interfaces in the correct declaration order, including base types.
- Renders fields using Unity’s built in `PropertyField`, so standard attributes like `Header`, `Tooltip`, spacing, etc. behave exactly like the default inspector.
- Shows `[Inject]` and `[ReadOnly]` fields as read-only.
- Displays `[SerializeInterface]` fields with interface type labels and validation.
- Supports arrays and lists using Unity’s native collection UI.
- Recursively renders nested `[Serializable]` reference types.
- Validates assigned interface values and resolves components from assigned `GameObject`s when possible.
- Omits non-serialized, backing only, or hidden fields automatically.

If you create your own inspector for a specific type, or a catch all `Editor` for `MonoBehaviour`, Unity will use that instead of Saneject’s fallback inspector. In that case, Saneject’s inspector behavior will not apply unless you explicitly call back into it.

You can fully restore Saneject’s inspector behavior inside a custom editor by calling:

```csharp
SanejectInspector.DrawDefault(target, serializedObject);
```

You can also restore the behavior more granularly by using individual static methods from `SanejectInspector` (explained below).

## Saneject inspector API

`SanejectInspector` contains the full inspector rendering system used by Saneject.  
It is built around a data collection phase followed by a deterministic rendering phase that relies on Unity’s native `PropertyField` UI.

You can use the full system:

```csharp
SanejectInspector.DrawDefault(target, serializedObject);
```

Or integrate individual parts for advanced tooling or custom inspectors.

### Data collection

| Method                | Description                                                                                                                          |
|-----------------------|--------------------------------------------------------------------------------------------------------------------------------------|
| `CollectPropertyData` | Collects `PropertyData` for all serializable fields of the given type and its base classes, including `[SerializeInterface]` fields. |

### Drawing & validation

| Method                         | Description                                                                                                                                                                                                                                                                                |
|--------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `DrawDefault`                  | Draws the complete default Saneject MonoBehaviour inspector, including the script field, all serializable fields in declaration order, injection aware read only handling, custom UI and validation for `[SerializeInterface]` fields, and recursive drawing of nested serializable types. |
| `DrawMonoBehaviourScriptField` | Draws the default script field at the top of a MonoBehaviour inspector.                                                                                                                                                                                                                    |
| `DrawAndValidateProperties`    | Draws a collection of `PropertyData` with the given display names, read only flags and validates interface fields.                                                                                                                                                                         |
| `DrawAndValidateProperty`      | Draws a single `PropertyData` with the given display name, read only flag and validates interface fields, including nested serializable types.                                                                                                                                             |

### Helpers & extensions

| Method                         | Description                                                                                                                         |
|--------------------------------|-------------------------------------------------------------------------------------------------------------------------------------|
| `ValidateProperty`             | Validates that the property is assigned to an object that implements the expected type.                                             |
| `IsReadOnly`                   | Returns true if the field should be drawn as read only due to `[Inject]` or `[ReadOnly]`.                                           |
| `HasReadOnlyAttribute`         | Returns true if the field is marked with `[ReadOnly]`.                                                                              |
| `ShouldDraw`                   | Returns true if the field is valid for drawing in the inspector.                                                                    |
| `HasInjectAttribute`           | Returns true if the field is marked with `[Inject]`.                                                                                |
| `IsSerializeInterface`         | Returns true if the field is marked with `[SerializeInterface]`.                                                                    |
| `ResolveType`                  | Returns the element type of a single or collection type (array or list).                                                            |
| `GetInterfaceBackingFieldName` | Returns the logical name of a field, stripping compiler auto property backing syntax when present and adds two leading underscores. |
| `IsNestedSerializable`         | Returns true if the type is a non Unity serializable reference type suitable for custom nested drawing.                             |
| `GetAllFields`                 | Collects all `FieldInfo` of the type, including base classes.                                                                       |

These APIs are intended for integrating Saneject’s inspector behavior into custom editors, building editor tooling, or selectively reusing parts of the inspector pipeline while preserving correct ordering, validation, and visibility rules.