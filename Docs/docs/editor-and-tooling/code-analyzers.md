---
title: Code analyzers
---

# Code analyzers

Saneject ships with [Roslyn analyzers](../reference/glossary.md#roslyn-analyzer) for common injection attribute mistakes.
These checks run during C# analysis, so issues are reported in the editor and during compilation instead of appearing later during an [injection run](../reference/glossary.md#injection-run).

## Why this exists

[Editor-time injection](../reference/glossary.md#editor-time-injection) writes values into serialized members.
If an injected member is not serializable in Unity, or if an interface serialization attribute is used on the wrong type, the injection result can be missing or misleading.

The analyzer rules catch those mistakes early and report clear, fixable diagnostics.

## Analyzer rules

| Diagnostic ID | Rule                                                                                                                                                                                                       | Severity | Category    |
|---------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------|-------------|
| `INJ001`      | `[Inject]` fields must have either `[SerializeField]`, `[SerializeInterface]`, or be `public`.                                                                                                             | Error    | `Injection` |
| `INJ002`      | `[SerializeInterface]` fields must be interface-typed members. Supported shapes are interface, interface array, and one-parameter generic collections of an interface type (for example `List<IService>`). | Error    | `Injection` |

## Rule details

### `INJ001`: `[Inject]` must be serializable or public

This rule reports when a field has `[Inject]` but is neither:

- `public`
- marked with Unity's `[SerializeField]`
- marked with Saneject's `[SerializeInterface]`

```csharp
using Plugins.Saneject.Experimental.Runtime.Attributes;
using UnityEngine;

public partial class PlayerHud : MonoBehaviour
{
    [Inject]
    private Camera mainCamera; // INJ001

    [Inject, SerializeField]
    private Camera minimapCamera; // OK

    [Inject, SerializeInterface]
    private IHealthService healthService; // OK
}
```

### `INJ002`: `[SerializeInterface]` requires interface types

This rule reports when `[SerializeInterface]` is placed on a non-interface shape.

```csharp
using System.Collections.Generic;
using Plugins.Saneject.Experimental.Runtime.Attributes;
using UnityEngine;

public partial class EnemyHud : MonoBehaviour
{
    [SerializeInterface]
    private IEnemyService enemyService; // OK

    [SerializeInterface]
    private List<IEnemyService> allEnemyServices; // OK

    [SerializeInterface]
    private Camera worldCamera; // INJ002
}
```

## Code fixes

Saneject also includes [Roslyn code fixes](../reference/glossary.md#roslyn-code-fix) for both diagnostics.
When your IDE supports Roslyn quick actions, the fixes appear directly in the lightbulb menu.

### Fixes for `INJ001`

- `Add [SerializeField]`
- `Add [SerializeInterface]` (offered when the field type is an interface)
- `Make field public`

### Fixes for `INJ002`

- `Remove [SerializeInterface]`
- `Replace [SerializeInterface] with [SerializeField]`

> [screenshot containing Rider or Visual Studio showing an `INJ001` error on an `[Inject]` field and a quick-fix menu with `Add [SerializeField]`, `Add [SerializeInterface]`, and `Make field public`]

## Where the analyzer is loaded from

In the Unity project, Saneject's analyzer assembly is included at:

- `Assets/Plugins/Saneject/Experimental/Roslyn/Saneject.Analyzers.dll`

It is imported with the `RoslynAnalyzer` asset label, so Unity treats it as analyzer tooling instead of runtime gameplay code.

## Related pages

- [Field, property & method injection](../core-concepts/field-property-and-method-injection.md)
- [Serialized interface](../core-concepts/serialized-interface.md)
- [Injection menus](injection-menus.md)
- [Logging & validation](logging-and-validation.md)
- [Settings](settings.md)
- [Glossary](../reference/glossary.md)
