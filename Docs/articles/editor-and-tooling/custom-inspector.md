# Custom inspector

## MonoBehaviourInspector

Saneject includes a `MonoBehaviourInspector` that acts as a fallback editor for all `MonoBehaviour` subclasses. It draws:

- The script reference field at the top.
- All serializable fields in declaration order, including fields defined in base classes.
- `[SerializeInterface]` backing fields next to their corresponding interface fields (rather than at the bottom where the Roslyn-generated partial would normally place them).
- Nested `[Serializable]` classes as collapsible foldouts with correct field ordering.
- `[Inject]` fields as read-only when **Show Injected Fields/Properties** is enabled in User Settings.
- Interface field validation — if a manually assigned object doesn't implement the expected interface, it's cleared to `null` with an error log.

## Conflicts with other custom inspectors

If another plugin registers a `[CustomEditor(typeof(MyComponent))]` for a type, that inspector takes priority and Saneject's fallback is bypassed. The result is typically incorrect field ordering — `[SerializeInterface]` backing fields appear at the bottom, and nested class fields may be out of order.

## Restoring Saneject layout in your own editor

If you need a custom editor for a type that also uses Saneject features, construct a `ComponentModel` and delegate drawing to `SanejectInspector`:

```csharp
using Plugins.Saneject.Experimental.Editor.Inspectors;
using Plugins.Saneject.Experimental.Editor.Inspectors.Models;
using UnityEditor;

[CustomEditor(typeof(MyComponent))]
public class MyComponentEditor : Editor
{
    private ComponentModel componentModel;

    private void OnEnable()
    {
        componentModel = new ComponentModel(target, serializedObject);
    }

    public override void OnInspectorGUI()
    {
        SanejectInspector.OnInspectorGUI(componentModel);
    }
}
```

`ComponentModel` collects all drawable fields from the target type (including base class fields) and builds the model that `SanejectInspector` uses for rendering. Construct it once in `OnEnable` and reuse it.

## Drawing individual properties

If you only want Saneject to handle part of your inspector, you can call the lower-level methods directly:

```csharp
// Draw a single property with Saneject's read-only and interface-validation logic
SanejectInspector.DrawProperty(propertyModel);

// Validate a single property (clears invalid interface assignments)
SanejectInspector.ValidateProperty(propertyModel);

// Draw just the script reference field
SanejectInspector.DrawMonoBehaviourScriptField(target);
```

`PropertyModel` instances are available via `componentModel.Properties`.
