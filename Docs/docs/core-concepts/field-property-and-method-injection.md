---
title: Field, property & method injection
---

# Field, property & method injection

Field, property, and method injection is how Saneject writes resolved dependencies into components during an editor [injection run](../reference/glossary.md#injection-run).
 
For [binding](../reference/glossary.md#binding) setup details, see [Binding](binding.md) and [Scope](scope.md).

## How it works

At a high level, Saneject performs these steps:

1. Scans active [injection targets](../reference/glossary.md#injection-target) for `[Inject]` fields and `[Inject]` methods.
2. Also scans nested `[Serializable]` class instances inside those components.
3. For each [injection site](../reference/glossary.md#injection-site), finds a matching [binding](../reference/glossary.md#binding) by walking from the nearest [scope](../reference/glossary.md#scope) upward.
4. Matches on requested type, collection shape (single vs array/list), and optional [binding qualifiers](../reference/glossary.md#binding-qualifier) (ID, target type, member name).
5. Locates dependency objects from the selected [binding](../reference/glossary.md#binding) and applies [context isolation](../reference/glossary.md#context-isolation) rules.
6. Injects values in this order: [global registrations](../reference/glossary.md#global-registration), fields and auto-properties, then methods.

Important behavior:

- For single-value sites, Saneject assigns the first located candidate.
- For `T[]` and `List<T>`, Saneject assigns all located candidates.
- If no match is found, Saneject records missing [binding](../reference/glossary.md#binding) or missing dependency errors and injects `null`.
- `suppressMissingErrors` only suppresses missing [binding](../reference/glossary.md#binding) and missing dependency logs. It does not suppress method invocation exceptions.

## Inject attribute options

`InjectAttribute` supports these forms:

| Syntax                                        | Meaning                                                              |
|-----------------------------------------------|----------------------------------------------------------------------|
| `[Inject]`                                    | Type-only matching.                                                  |
| `[Inject("id")]`                              | Type + ID matching.                                                  |
| `[Inject(suppressMissingErrors: true)]`       | Type matching, but suppress missing binding/dependency logs.         |
| `[Inject("id", suppressMissingErrors: true)]` | Type + ID matching, with suppressed missing binding/dependency logs. |

The `ID` value must match a [binding qualifier](../reference/glossary.md#binding-qualifier) declared with `ToID(...)`.

## Field injection

Annotate a field with `[Inject]`. Field injection is persisted through Unity serialization, so injected fields should be serializable.

In practice:

- Concrete fields should be `public` or `[SerializeField] private`.
- Interface fields should use `[SerializeInterface]`, so Unity can serialize and show them in the inspector. Access modifier does not matter for this case.
- Arrays and `List<>` require [collection bindings](../reference/glossary.md#collection-binding) (`BindComponents`, `BindAssets`, or equivalent collection forms).

```csharp
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

public partial class HUDView : MonoBehaviour
{
    // Concrete component dependency.
    [Inject, SerializeField]
    private Camera mainCamera;

    // Same type can be disambiguated by ID.
    [Inject("scoreText"), SerializeField]
    private Text scoreText;

    // Interface dependency.
    [Inject, SerializeInterface]
    private IScoreService scoreService;

    // Interface collection dependency.
    [Inject, SerializeInterface]
    private List<IEnemyObservable> enemyObservables;

    // Collection dependencies.
    [Inject, SerializeField]
    private AudioClip[] hitSfx;

    [Inject, SerializeField]
    private List<AudioClip> uiSfx;

    // Optional dependency. Missing logs are suppressed.
    [Inject(suppressMissingErrors: true), SerializeField]
    private Text optionalDebugText;
}
```

```csharp
using Plugins.Saneject.Runtime.Scopes;

public class HUDScope : Scope
{
    protected override void DeclareBindings()
    {
        BindComponent<Camera>()
            .ToMember("mainCamera")
            .FromAnywhere();

        BindComponent<Text>()
            .ToID("scoreText")
            .FromScopeDescendants(includeSelf: true);

        BindComponent<IScoreService, ScoreService>()
            .FromRuntimeProxy()
            .FromGlobalScope();
        
        BindComponents<IEnemyObservable>()
            .FromAnywhere();

        BindAssets<AudioClip>()
            .ToMember("hitSfx")
            .FromFolder("Assets/Game/Audio/Sfx/Hit");

        BindAssets<AudioClip>()
            .ToMember("uiSfx")
            .FromFolder("Assets/Game/Audio/Sfx/UI");
    }
}
```

## Property injection

`InjectAttribute` targets fields and methods, but not normal properties.  
For properties, use auto-properties with a field target:

- `[field: Inject]`
- `[field: SerializeField]` for concrete types
- `[field: SerializeInterface]` for interface types

```csharp
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;

public partial class CombatHud : MonoBehaviour
{
    [field: Inject, SerializeField]
    public Camera MainCamera { get; private set; }

    [field: Inject("playerRoot"), SerializeField]
    public Transform PlayerRoot { get; private set; }

    [field: Inject, SerializeInterface]
    public ICombatService CombatService { get; private set; }

    [field: Inject, SerializeInterface]
    public IEnemyObservable[] EnemyObservables { get; private set; }

    [field: Inject, SerializeField]
    public List<AudioClip> UISounds { get; private set; }
}
```

Notes:

- This works for auto-properties because the compiler generates a backing field that Saneject can inject.
- [Binding qualifier](../reference/glossary.md#binding-qualifier) `ToMember("MainCamera")` matches the logical property name, not the compiler-generated backing field name.

## Method injection

Annotate a method with `[Inject]`. Saneject resolves each parameter from [bindings](../reference/glossary.md#binding) and then invokes the method.

Because this invocation happens at editor-time, method injection is mostly useful for setup/configuration logic. Typical examples include configuring components whose source you cannot modify and add `[Inject]` to, applying values to built-in Unity components from injected config assets, validating setup, or running custom wiring logic inside your type.

Key rules:

- Qualifiers (`ID`, `ToTarget`, `ToMember`) are matched against the method as a whole.
- Method-level `ID` and `suppressMissingErrors` settings apply to all parameters in that method.
- Each parameter is resolved by its own requested type and shape.
- Methods are invoked after field and property injection.
- If dependencies are missing, parameters resolve to `null` and invocation is still attempted.
- Method exceptions are caught and logged. They do not break the overall [injection run](../reference/glossary.md#injection-run).

```csharp
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class CombatController : MonoBehaviour
{
    [Inject("combatInit")]
    private void Initialize(
        ICombatService combatService,
        Camera mainCamera,
        IEnemyObservable[] enemyObservables,
        List<EnemyController> enemies)
    {
        // Use injected parameters.
    }

    [Inject("optionalUI", suppressMissingErrors: true)]
    private void TryWireOptionalUI(Text debugText, Button retryButton)
    {
        if (debugText == null || retryButton == null)
            return;
    }
}
```

Example [bindings](../reference/glossary.md#binding) for `Initialize`:

```csharp
protected override void DeclareBindings()
{
    BindComponent<ICombatService, CombatService>()
        .ToID("combatInit")
        .ToMember("Initialize")
        .FromScopeSelf();

    BindComponent<Camera>()
        .ToID("combatInit")
        .ToMember("Initialize")
        .FromAnywhere();

    BindComponents<IEnemyObservable, EnemyManager>()
        .ToID("combatInit")
        .ToMember("Initialize")
        .FromScopeDescendants(includeSelf: true);

    BindComponents<EnemyController>()
        .ToID("combatInit")
        .ToMember("Initialize")
        .FromScopeDescendants(includeSelf: true);
}
```

## Supported dependency types

Saneject resolves dependencies from [component bindings](../reference/glossary.md#component-binding) and [asset bindings](../reference/glossary.md#asset-binding). That means injected objects are Unity objects, such as:

- `UnityEngine.Component` instances
- `UnityEngine.Object` assets, e.g., `ScriptableObject`, `AudioClip`, `Texture2D`, etc.

Plain C# object (POCO) services are not supported.

For service-like patterns, use [runtime proxies](../reference/glossary.md#runtime-proxy) with component implementations. [Runtime proxy bindings](../reference/glossary.md#runtime-proxy-binding) can resolve from [global scope](../reference/glossary.md#global-scope), loaded scenes, prefabs, or newly created `GameObject`s, with transient or singleton instance modes. See [Runtime proxy](runtime-proxy.md) for details.

## Interface injection notes

Interface injection is supported for fields, auto-properties, and method parameters.

For serialized fields and auto-properties, use `[SerializeInterface]` so references are visible and persisted in Unity's inspector/serialization workflow. See [Serialized interface](serialized-interface.md).

## Related pages

- [Scope](scope.md)
- [Binding](binding.md)
- [Context](context.md)
- [Serialized interface](serialized-interface.md)
- [Runtime proxy](runtime-proxy.md)
- [Global scope](global-scope.md)
- [Glossary](../reference/glossary.md)